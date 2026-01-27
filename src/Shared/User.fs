module Shared.User

open System
open System.Collections.Generic
open FSharp.Control
open Microsoft.AspNetCore.Http

open Giraffe.Core

[<Literal>]
let userStr = "user"
[<Literal>]
let permissionsStr = "userPermissions"
let GetUserFromCtx (ctx: HttpContext) =
    match ctx.Items.TryGetValue userStr with
    | true, o -> Some (o :?> DBContext.Users.User)
    | _ -> None
let getPermissions (ctx: HttpContext) =
    match ctx.Items.TryGetValue permissionsStr with
    | true, o -> Some (o :?> DBContext.Permissions.Permission seq)
    | false, _ -> None
    

let WithUser (dbcontext: DBContext.Data) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let authcookie = (ctx.Request.Cookies.Item "user-auth")
        if authcookie <> null then
            match! dbcontext.Users.FindUserBySession(Guid.Parse authcookie) with
            | Some u ->
                ctx.Items.Add(userStr, u)
                let! a = TaskSeq.toListAsync (dbcontext.Permissions.GetUserPermissions u.Id)
                ctx.Items.Add(permissionsStr, a)
            | _ -> ()

        return! next ctx
    }
    

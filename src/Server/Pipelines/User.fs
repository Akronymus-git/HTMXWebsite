module Server.Pipelines.User

open System
open System.Globalization
open ClassLibrary1.Logging
open FSharp.Control
open Giraffe.Core
open Microsoft.AspNetCore.Http
open Shared.User


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
    

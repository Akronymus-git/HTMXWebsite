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
    
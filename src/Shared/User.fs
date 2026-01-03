module Shared.User

open System
open Microsoft.AspNetCore.Http

open Giraffe.Core


let WithUser (dbcontext: DBContext.Data) (next:HttpFunc) (ctx:HttpContext) =
    let authcookie = (ctx.Request.Cookies.Item "user-auth")
    let user = dbcontext.Users.FindUserBySession (Guid.Parse authcookie) |> Async.RunSynchronously
    match user with
    | Some u ->
        ctx.Items.Add ("userName", u.Name)
        ctx.Items.Add ("userEmail", u.Email)
    | _ -> ()
    next ctx


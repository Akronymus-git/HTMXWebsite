module Client.Index

open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Shared.Permission

let Page (ctx: HttpContext) =
    html [] [
        a [_href "/discord"] [Text "Discord";br []]
        a [_href "/login"] [Text "Login";br []]
        a [_href "/login/register"] [Text "Register";br []]
        if isAdmin ctx then
            a [_href "/admin"] [Text "Admin";br []]
    ]
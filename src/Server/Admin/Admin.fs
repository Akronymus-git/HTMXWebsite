module Server.Admin.Admin

open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Server.Admin.Logs
open Server.Admin.Users
open Shared.Permission


let adminAuth next ctx =
    match isAdmin ctx with
    | true -> next ctx
    | false -> redirectTo false "/login" earlyReturn ctx



let Router (dbcontext: DBContext.Data) =
    router {
        pipe_through (adminAuth)
        forward "/Logs" (Logs.Router dbcontext)
        forward "/Users" (Users.Router dbcontext)
    }

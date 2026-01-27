module Server.Admin.Logs

open System
open Giraffe.Core
open Microsoft.AspNetCore.Http
open Saturn
open Saturn.Router
open Giraffe.Core


let returnTable (context: DBContext.Data) next (ctx: HttpContext) =
    let offset =
        (string
         >> fun x ->
             match Int32.TryParse x with
             | false, _ -> 0
             | true, y -> y)
            ctx.Request.Query["offset"]

    let limit =
        (string
         >> fun x ->
             match Int32.TryParse x with
             | false, _ -> 10
             | true, y -> y)
            ctx.Request.Query["limit"]

    task {
        let! results = context.Logs.getLogs limit offset

        return!
            ((pipeline {
                render_html (Client.Log.DisplayLogs results limit offset)
                set_status_code 200
            })
                next
                ctx)
    }




let Router (dbcontext: DBContext.Data) =
    router { get "" (returnTable dbcontext) }

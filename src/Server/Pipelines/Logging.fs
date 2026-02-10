module Server.Pipelines.Logging

open System
open System.Globalization
open ClassLibrary1.Logging
open Giraffe.Core
open Microsoft.AspNetCore.Http
open Saturn

let WithLogging (dbcontext: DBContext.Data) (filepath: string) (next: HttpFunc) (ctx: HttpContext) =
    task {

        let requestInfo =
            Obj
                [ "request",
                  Obj
                      [ "path", Str (ctx.Request.Path.ToString())
                        "method", Str ctx.Request.Method
                        "userAgent", Str(ctx.Request.Headers["User-Agent"].ToString())
                        "time", Str(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture))
                        match ctx.Request.Headers.TryGetValue "X-Forwarded-For" with
                        | true, source -> "ip", Str(source.ToString())
                        | false, _ -> "ip", Str(ctx.Request.HttpContext.Connection.RemoteIpAddress.ToString())
                        ] ]

        ctx.Items.Add("LoggingData", requestInfo)

        for param in ctx.Request.Query do
            for value in param.Value do
                AddLoggingData ctx $"request.queryparams.{param.Key}" (Str value)

        try
            let res = next ctx

            AddLoggingData
                ctx
                "response"
                (Obj
                    [ "statusCode", (Num ctx.Response.StatusCode)
                      "time", (Str(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture))) ])

            do! logRequest dbcontext.Logs filepath true ctx
            return! res
        with e ->
            AddLoggingData ctx "Errors" (Str <| e.Message)
            do! logRequest dbcontext.Logs filepath false ctx
            let mutable ie = e.InnerException

            while ie <> null do
                Console.WriteLine e.Message
                ie <- ie.InnerException

            return! Response.internalError ctx e.Message
    }

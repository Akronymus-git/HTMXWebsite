module Server.Pipelines.Security


open System
open System.Globalization
open ClassLibrary1.Logging
open Giraffe.Core
open Microsoft.AspNetCore.Http
open Saturn
open Shared.User

let RejectUserAgent (next: HttpFunc) (ctx: HttpContext) =
    task {
        match GetUserFromCtx ctx with
        | Some _ -> return! next ctx
        | None ->
            match ctx.Request.Headers.UserAgent |> List.ofSeq with
            | [ ] ->
                let pl =
                    pipeline {
                       set_status_code 403
                       set_body_from_string "Invalid User Agent"
                    }
                AddLoggingData ctx "response.rejectReason" (Str "Invalid User Agent")
                return! pl earlyReturn ctx
            | [ "" ] ->
                let pl =
                    pipeline {
                       set_status_code 403
                       set_body_from_string "Invalid User Agent"
                    }
                AddLoggingData ctx "response.rejectReason" (Str "Invalid User Agent")
                return! pl earlyReturn ctx
            | _ -> return! next ctx

    }

module Client.Log

open Giraffe.ViewEngine
let DisplayLogs (results: DBContext.Logs.Log seq) limit offset=
    html [] [
        for log in results do
            Text log.Data
        if offset >= 10 then
            a [_href $"?limit={limit}&offset={offset-10}" ] []
        a [_href $"?limit={limit}&offset={offset+10}" ] [Text "Next page"]
    ]

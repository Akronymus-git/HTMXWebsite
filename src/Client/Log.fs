module Client.Log

open System.Globalization
open Giraffe.ViewEngine
let logtable = tag "log-table"
let DisplayLogs (results: DBContext.Logs.Log seq) limit offset=
    html [
        
    ] [
        link [ _rel "stylesheet"; _href "/style.css" ]
        logtable [] [
            span [] [Text "Data"]
            span [] [Text "Timestamp"]
            span [] [Text "UserId"]
            for log in results do
                span [] [Text log.Data]
                span [] [Text (log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))]
                span [] [Text (match log.UserId with Some x -> x.ToString() | _ -> "---")]
        ]
        
        if offset >= 10 then
            a [_href $"?limit={limit}&offset={offset-10}" ] []
        a [_href $"?limit={limit}&offset={offset+10}" ] [Text "Next page"]
    ]

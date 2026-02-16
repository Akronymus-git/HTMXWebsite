module Client.Admin.Logs

open System.Globalization
open Giraffe.ViewEngine
let logtable = tag "log-table"
let Index (results: DBContext.Logs.Log seq) (limit: int) offset=
    html [
        
    ] [
        link [ _rel "stylesheet"; _href "/style.css" ]
        logtable [] [
            span [] [Text "Number"]
            span [] [Text "Success"]
            span [] [Text "Timestamp"]
            span [] [Text "UserId"]
            span [] [Text "StatusCode"]
            span [] [Text "UserAgent"]
            span [] [Text "IP"]
            span [] [ Text "Method" ]
            span [] [Text "Path"]
            span [] [Text "Data"]
            for i, log in Seq.mapi (fun idx x -> idx, x) results do
                span [] [Text (string (offset + i + 1))]
                span [] [Text (match log.Success with Some x -> x | _ -> "---")]
                span [] [Text (log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))]
                span [] [Text (match log.UserId with Some x -> x.ToString() | _ -> "---")]
                span [] [Text (match log.StatusCode with Some x -> x.ToString() | _ -> "---")]
                span [] [Text (match log.UserAgent with Some x -> x | _ -> "---")]
                span [] [Text (match log.IP with Some x -> x | _ -> "---")]
                span [] [Text (match log.Method with Some x -> x | _ -> "---")]
                span [] [Text (match log.Path with Some x -> x | _ -> "---")]
                span [] [Text log.Data]
        ]
        
        if offset >= 10 then
            a [_href $"?limit={limit}&offset={offset-10}" ] []
        a [_href $"?limit={limit}&offset={offset+10}" ] [Text "Next page"]
    ]

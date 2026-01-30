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
            span [] [Text "Timestamp"]
            span [] [Text "Data"]
            span [] [Text "UserId"]
            for i, log in Seq.mapi (fun idx x -> idx, x) results do
                span [] [Text (string (offset + i + 1))]
                span [] [Text (log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))]
                span [] [Text log.Data]
                span [] [Text (match log.UserId with Some x -> x.ToString() | _ -> "---")]
        ]
        
        if offset >= 10 then
            a [_href $"?limit={limit}&offset={offset-10}" ] []
        a [_href $"?limit={limit}&offset={offset+10}" ] [Text "Next page"]
    ]

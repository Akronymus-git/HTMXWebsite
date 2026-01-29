module Client.Admin.Index

open Giraffe.ViewEngine

let Page =
    html [] [
        a [_href "/admin/logs?limit=100&offset=0"] [Text "Logs"]
        br []
        a [_href "/admin/users?limit=100&offset=0"] [Text "Users"]
    ]
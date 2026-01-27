module Client.CapSim

open Client
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Giraffe.ViewEngine.Htmx

let infobar = tag "infobar-area"
let mainArea = tag "main-area"

let Index (ctx: HttpContext) =
    html
        []
        [ head
              []
              [ title [] [ Text "CapSim" ]
                HtmlElements.link [ _rel "stylesheet"; _href "capSimStyle.css" ]
                script [ _src "htmx.min.js" ] [] ] ]

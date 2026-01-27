module Client.Status_404

open System.Net.Http
open Giraffe.ViewEngine
open Giraffe.Core
open Microsoft.AspNetCore.Http

let Page =
    html [] [Text "Not found"]

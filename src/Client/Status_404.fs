module Client.Status_404
open System.Net.Http
open Giraffe.ViewEngine
open Giraffe.Core
open Microsoft.AspNetCore.Http
let Page (next:HttpFunc) (ctx:HttpContext):HttpFuncResult =
    let page =
        html [] []
        
        
    htmlView page next ctx
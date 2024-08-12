module Server.AdminPipeline
open Giraffe.Core
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Saturn.Router
open Client.Admin
open Saturn.Pipeline
open Giraffe.Htmx

let viewWithContext (stuff: bool -> XmlNode) f (ctx:HttpContext) =
    
    let body = (stuff ctx.Request.IsHtmx).ToString()
    (setBodyFromString body >=> setContentType "text/html; charset=utf-8") f ctx
    
    
let AuthorizedAdminPipeline: HttpHandler =
    router {
        
        get "/token" (viewWithContext Index)
        post "/token" (viewWithContext Index)
        deletef "/token/%i" (fun id -> viewWithContext Index)
        get ""  (viewWithContext Index)
    } 
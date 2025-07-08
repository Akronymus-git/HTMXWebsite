module Server.Server

open System
open System.IO
open System.Net.Http
open Client
open Giraffe
open Giraffe.ViewEngine.HtmlElements
open Microsoft.AspNetCore.Http
open Microsoft.Data.Sqlite
open Saturn
open Client.IsoGridRandomizer
open Client.Index
open Client.RandomStuff
open Client.About
open System.Collections.Generic
open Server.AdminPipeline
open Giraffe.Htmx
open Client.MurderBingo

let viewWithContext (stuff: bool -> XmlNode) f (ctx:HttpContext) =
    
    let body =  htmlView (stuff ctx.Request.IsHtmx)
    body f ctx
    
let getIsoGrid seed : HttpHandler =
    fun f ctx ->
        let x = ctx.GetQueryStringValue "x" |> Result.map int
        let y = ctx.GetQueryStringValue "y" |> Result.map int

        match x, y with
        | Ok x1, Ok y1 -> viewWithContext (IsoGridRandomizer seed x1 y1) f ctx
        | _ -> viewWithContext IsoGridForm f ctx

let gridRandomizerMiddleware seed = router { get "" (getIsoGrid seed) }

let notFoundPipeline =
    pipeline {
        set_status_code 404
        plug (fun f ctx -> htmlView (Client.NotFound.NotFound false) f ctx)
    }

let forbiddenPipeline = pipeline { set_status_code 403 }
let webApp =
    router {
        get "/discord" (redirectTo true "https://discord.gg/yK4WsfV5zy")
        get "/" (viewWithContext Index)
        get "/about" (viewWithContext About)
        get "/randomStuff" (viewWithContext RandomStuff)
        forward "/isometricGridRandomizer" (fun x ->
            gridRandomizerMiddleware
                ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % (Int32.MaxValue |> int64))
                 |> int)
                x)
        get "/rpg" (htmlView Incremental.Incremental)
        get "/tools" (htmlView Tools.Tools)
        get "" notFoundPipeline
        post "" forbiddenPipeline
        patch "" forbiddenPipeline
        put "" forbiddenPipeline
        delete "" forbiddenPipeline

    }

let configServices services = services

let app =
    application { 
        url "http://localhost:5000"
        use_router webApp 
        memory_cache 
        use_static "public/"
        use_gzip
        service_config configServices 

    }
run app

module Server

open System
open Giraffe
open Saturn
open Client.IsoGridRandomizer
open Client.Index
open Client.RandomStuff
open Client.About

let murderBingoRouter =
    router {
        get "" (htmlString "")
    }
    
    
let getIsoGrid seed : HttpHandler =
    fun f ctx -> 
        let x = ctx.GetQueryStringValue "x" |> Result.map int
        let y = ctx.GetQueryStringValue "y" |> Result.map int
        match x,y with
        | Ok x1, Ok y1 ->
            htmlView (IsoGridRandomizer seed x1 y1) f ctx
        | _ ->
            htmlView (IsoGridForm) f ctx
            
let gridRandomizerMiddleware seed =
    router {
        get "" (getIsoGrid seed)
    }
let notFoundPipeline =
    pipeline {
        set_status_code 404
        plug (htmlView Client.NotFound.NotFound)
    }
let forbiddenPipeline =
    pipeline {
        set_status_code 403
    }
     
let webApp =
    router {
        get "/" (htmlView Index)
        get "/about" (htmlView About)
        get "/randomStuff" (htmlView RandomStuff)
        forward "/murderBingo" murderBingoRouter
        forward "/isometricGridRandomizer" (fun x -> gridRandomizerMiddleware ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % (Int32.MaxValue |> int64)) |> int) x)
        get "" (notFoundPipeline)
        post "" forbiddenPipeline
        patch "" forbiddenPipeline
        put "" forbiddenPipeline
        delete "" forbiddenPipeline
        
    }
let configServices services =
    services
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

module Server

open Giraffe
open Saturn
open Client.IsoGridRandomizer
open Client.Index
open Client.RandomStuff

let murderBingoRouter =
    router {
        get "" (htmlString "")
    }
    
let gridRandomizerMiddleware () =
    pipeline {
        plug (htmlView (IsoGridRandomizer ()))
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
        get "/about" (htmlView Index)
        get "/randomStuff" (htmlView RandomStuff)
        forward "/murderBingo" murderBingoRouter
        forward "/isometricGridRandomizer" (fun x -> gridRandomizerMiddleware () x)
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
        use_router webApp
        memory_cache
        use_static "public/"
        use_gzip
        service_config configServices
        
    }

run app

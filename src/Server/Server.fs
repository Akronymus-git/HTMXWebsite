module Server

open System.Net.Mime
open Giraffe
open Saturn

        
let webApp =
    router {
        get "/" (htmlView Client.Index.Index)
        
        
    }
let configServices services =
    services
let app =
    application {
        use_router webApp
        memory_cache
        use_static ".././public"
        use_gzip
        service_config configServices
    }

run app

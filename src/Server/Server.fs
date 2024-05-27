module Server

open Giraffe
open Saturn


let webApp =
    router {
        get "/" (text "Hello from SAFE!")
    }

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app

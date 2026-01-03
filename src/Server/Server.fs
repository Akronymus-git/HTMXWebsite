module Server.Server

open System.Net
open ClassLibrary1
open Logging
open Giraffe
open Microsoft.Data.Sqlite
open Saturn

    

    
CredentialCache.DefaultNetworkCredentials.UserName <- "noreply@akronymus.net"
CredentialCache.DefaultNetworkCredentials.Password <- ""//System.Environment.GetCommandLineArgs()[1]



let notFoundPipeline =
    pipeline {
        set_status_code 404
        plug Client.Status_404.Page
    }

let context = DBContext.Data (new SqliteConnection("Data Source=data.db"))
context.Open()
let webApp =
    router {
        get "/discord" (redirectTo true "https://discord.gg/yK4WsfV5zy")
        forward "/login" (Login.Router context)
        forward "/capSim" CapSim.Router
        not_found_handler notFoundPipeline
    }


let app =
    application { 
        url "http://localhost:5000"
        use_router
            (pipeline {
                plug (withLogger "log.txt")
                plug (Shared.User.WithUser context)
                plug webApp
            })
        memory_cache 
        use_static "public/"
        use_gzip
        service_config id 

    }

run app

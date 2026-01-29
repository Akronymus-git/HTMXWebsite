module Server.Server

open System.Net
open ClassLibrary1
open Logging
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Data.Sqlite
open Saturn
open DbUp
open DbUp.Engine
open System
open System.Reflection

open Microsoft.Extensions.DependencyInjection
open Microsoft.Data.Sqlite

[<Literal>]
let connectionString = "Data Source=data.db"

CredentialCache.DefaultNetworkCredentials.UserName <- "noreply@akronymus.net"
CredentialCache.DefaultNetworkCredentials.Password <- "" //System.Environment.GetCommandLineArgs()[1]


let runMigrations (connectionString: string) =
    let upgrader =
        DbUp.DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof<DBContext.Data>.Assembly) // Or move scripts to DBContext project
            .LogToConsole()
            .WithVariablesDisabled()
            .Build()

    upgrader.PerformUpgrade()

runMigrations connectionString |> ignore

let notFoundPipeline =
    pipeline {
        set_status_code 404
        plug (htmlView Client.Status_404.Page)
    }

let context = DBContext.Data(new SqliteConnection(connectionString))
context.Open()

let withContext handler next ctx  =
    handler ctx next ctx
let webApp =
    router {
        get "/discord" (redirectTo true "https://discord.gg/yK4WsfV5zy")
        forward "/login" (Login.Router context)
        forward "/capSim" CapSim.Router
        forward "/admin" (Admin.Admin.Router context)
        forward "/bingo" (Bingo.Router context)
        get "/" (withContext (Client.Index.Page >> htmlView))
        not_found_handler notFoundPipeline
    }

let NormalizePath next (ctx: HttpContext) =
    ctx.Request.Path <- ctx.Request.Path.Value.ToLowerInvariant()
    next ctx


let app =
    application {
        url "http://localhost:5000"

        use_router (
            pipeline {
                plug (NormalizePath)
                plug (withLogger context "log.txt")
                plug (Shared.User.WithUser context)
                plug webApp
            }
        )
        use_static "public/"

        memory_cache
        use_gzip
        service_config id

    }

run app

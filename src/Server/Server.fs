module Server.Server

open System
open System.IO
open System.Net.Http
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
[<Literal>]
let sqlConnection = "Data Source=data.db"
let getQuery (func: SqliteCommand -> unit) (parseRow: SqliteDataReader -> 'a) =
    use conn = new SqliteConnection(sqlConnection)
    conn.Open()
    use command = conn.CreateCommand()
    func command
    use reader = command.ExecuteReader()
    let res = new List<'a>()
    while reader.Read() do
        res.Add(parseRow reader)
    res
    
let checkIfTokenExists token =
    let f (x:SqliteCommand) =
        x.CommandText <- """Select true from tokens where token = $token"""
        x.Parameters.AddWithValue("$token", token) |> ignore
    let read (x: SqliteDataReader) =
        x.GetBoolean(0)
    getQuery f read |> Seq.length |> (<>) 0

let murderBingoRouter =
    let decider: HttpHandler =
        fun (f: HttpFunc) (ctx: HttpContext) ->
            let cookie = ctx.GetCookieValue "auth"

            match cookie with
            | None -> htmlString "" f ctx
            | Some x ->
                match checkIfTokenExists x with
                | true -> htmlString "auth" f ctx
                | _ -> htmlString "not auth" f ctx

    
    router { get "" decider }


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
let adminRouter: HttpHandler =
     fun func ctx ->
         match ctx.GetCookieValue "auth" with
         | None -> forbiddenPipeline func ctx
         | Some auth ->
             let comm (c:SqliteCommand) =
                 c.CommandText <- """
select 1 from Tokens 
inner join TokenRoles on Tokens.tokenId = TokenRoles.tokenId  
inner join Roles on TokenRoles.roleId = Roles.roleId 
where Tokens.token = $token and Roles.roleId = 0"""
                 c.Parameters.AddWithValue("$token", auth) |> ignore
             let parse (x: SqliteDataReader) =
                 x.GetBoolean 0
             let res = (getQuery comm parse).Count > 0
             match res with
             | false -> forbiddenPipeline func ctx
             | true -> AuthorizedAdminPipeline func ctx
         
    
let webApp =
    router {
        get "/" (viewWithContext Index)
        get "/about" (viewWithContext About)
        get "/randomStuff" (viewWithContext RandomStuff)
        get "/admin" adminRouter
        forward "/murderBingo" murderBingoRouter

        forward "/isometricGridRandomizer" (fun x ->
            gridRandomizerMiddleware
                ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % (Int32.MaxValue |> int64))
                 |> int)
                x)

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
let init() =
    try  
        use connect = new SqliteConnection(sqlConnection)
        connect.Open()
        let cmd =  connect.CreateCommand()
        cmd.CommandText <- ((File.ReadAllText "dbInit.sql") + (File.ReadAllText "init.sql"))
        cmd.ExecuteNonQuery() |> ignore
    with
    | ex -> Console.WriteLine ex.Message
        
    
init() 
run app

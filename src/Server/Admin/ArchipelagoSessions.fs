module Server.Admin.ArchipelagoSessions

open System
open System.Collections.Generic
open System.IO
open Archipelago.MultiClient.Net
open Archipelago.MultiClient.Net.BounceFeatures.DeathLink
open Archipelago.MultiClient.Net.Enums
open Archipelago.MultiClient.Net.MessageLog.Messages
open DBContext.ArchipelagoSessions
open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json.Linq
open Saturn

let memorySessions: IDictionary<string, ArchipelagoSession * (LogMessage -> unit) * DeathLinkService * (DeathLink -> unit)> =
    Dictionary<string, ArchipelagoSession * (LogMessage -> unit) * DeathLinkService  * (DeathLink -> unit)> ()

let logging gamename (message: LogMessage) =
    Console.WriteLine(gamename + (message.ToString()))
    File.AppendAllText (gamename, (DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " --- " + (message.ToString()) + "\r\n"))
let loggingSocket gamename (message: DeathLink) =
    
    Console.WriteLine(gamename + "Cause" + (message.Source.ToString()))
    Console.WriteLine(gamename + "Source" + (match message.Cause with | null -> "" | cause -> cause.ToString()))
    File.AppendAllText (gamename, (DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " --- Deathlink source: " + (message.Source.ToString()) +  " Deathlink cause: " + (match message.Cause with | null -> "" | cause -> cause.ToString()) + "\r\n"))

let addSession gamename uri game name password =
    let newSession = ArchipelagoSessionFactory.CreateSession uri

    
    let result = newSession.TryConnectAndLogin(
        game,
        name,
        ItemsHandlingFlags.AllItems,
        null,
        [| "DeathLink"; "TextOnly"; "Tracker" |],
        null,
        (match password with "" -> null | _ -> password),
        true
    )
    match result.Successful with
    | true ->
        let logger = (logging gamename)
        let logger_socket = (loggingSocket gamename)
        newSession.MessageLog.add_OnMessageReceived logger
        let deathlinkservice = newSession.CreateDeathLinkService()
        deathlinkservice.add_OnDeathLinkReceived logger_socket
            
        Some (newSession, logger, deathlinkservice, logger_socket)
    | false -> None
    
    
    

let removeSession (dbcontext: DBContext.Data) gamename =
    match memorySessions.TryGetValue gamename with
    | true, (session, logger,deathLinkService,  deathlinkLogger) ->
        match session.Socket.Connected with
        | true -> session.Socket.DisconnectAsync() |> Async.AwaitTask |> Async.RunSynchronously
        | false -> ()

        session.MessageLog.remove_OnMessageReceived logger
        deathLinkService.remove_OnDeathLinkReceived deathlinkLogger
        memorySessions.Remove gamename |> ignore
        dbcontext.ArchipelagoSessions.DeleteSession gamename |> ignore
        failwith "todo"
    | _ -> failwith "todo"

let extractFields (formdata: IFormCollection) =
    string formdata["gamename"],
    Uri ("wss://" + (string formdata["uri"])),
    string formdata["game"],
    string formdata["name"],
    string formdata["password"]

let SessionAdd (dbcontext: DBContext.Data) (next: HttpFunc) (ctx: HttpContext) =
    task {
        let! formdata = ctx.Request.ReadFormAsync()
        let gamename, uri, game, name, password = extractFields formdata
        match addSession gamename uri game name password with
        | Some (newSession, logger, deathlinkservice, deathlinklogger) ->
            memorySessions.Add(KeyValuePair(gamename, (newSession, logger, deathlinkservice, deathlinklogger)))
            dbcontext.ArchipelagoSessions.InsertSession (gamename, (string uri), game, name, Some password) |> ignore
        | None -> failwith "todo"
        return! (next ctx)
    }

let reconnectAll (dbcontext: DBContext.Data)  =
    task {
        let! dbSessions = dbcontext.ArchipelagoSessions.GetAllSessions ()
                       |> FSharp.Control.TaskSeq.toListAsync
        let addsession (sess: ArchipelagoSessionRow) =
            match addSession sess.GameName (Uri sess.Uri) sess.Game sess.Name sess.Password with
            | None -> ()
            | Some x -> memorySessions.Add(KeyValuePair(sess.GameName, x))

        List.iter addsession dbSessions 
    }

let overviewPage (dbcontext:DBContext.Data) next ctx=
    task {
        let! dbSessions = dbcontext.ArchipelagoSessions.GetAllSessions ()
                       |> FSharp.Control.TaskSeq.toListAsync
        return! htmlView (Client.Admin.ArchipelagoSessions.Index.Page dbSessions) next ctx
    }
    

let detailsPage (dbcontext:DBContext.Data) gamename next ctx=
    task {
        let! session = dbcontext.ArchipelagoSessions.GetSession gamename
        match session with
        | None -> return! (redirectTo false "/admin/archipelago" next ctx)
        | Some value -> return! htmlView (Client.Admin.ArchipelagoSessions.SessionDetails.Page value) next ctx
        
    }
let getFile (gamename: string) (next: HttpFunc) (ctx: HttpContext) =
    task {
        
        let response = ctx.Response
        response.Clear()
        response.ContentType <- "application/octet-stream"
        response.Headers.Add ("Content-Disposition", string ("attachment; filename=" + gamename + ".txt;") )
        do! response.SendFileAsync (gamename)
        return Some ctx
    }
    
let Router (dbcontext: DBContext.Data) =
    router {
        // ReSharper disable FSharpInterpolatedString
        post "" (SessionAdd dbcontext)
        postf "/%s" (fun name next ctx ->
            task {
                let! form = ctx.Request.ReadFormAsync()

                match string form["method"] with
                | "delete" -> return! removeSession dbcontext name next ctx
                | _ -> return! next ctx
            })
        getf "/%s/file" (getFile)
        getf "/%s" (detailsPage dbcontext)
        get "" (overviewPage dbcontext)

    }

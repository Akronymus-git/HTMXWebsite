module Server.Admin.ArchipelagoSessions

open System
open System.Collections.Generic
open Archipelago.MultiClient.Net
open Archipelago.MultiClient.Net.Enums
open Archipelago.MultiClient.Net.MessageLog.Messages
open DBContext.ArchipelagoSessions
open Giraffe
open Microsoft.AspNetCore.Http
open Newtonsoft.Json.Linq
open Saturn

let memorySessions: IDictionary<string, ArchipelagoSession * (LogMessage -> unit)> =
    dict []

let logging gamename (message: LogMessage) =
    Console.WriteLine(gamename + (message.ToString()))

let addSession gamename uri game name password =
    let newSession = ArchipelagoSessionFactory.CreateSession uri

    
    let result = newSession.TryConnectAndLogin(
        game,
        name,
        ItemsHandlingFlags.AllItems,
        null,
        [| "DeathLink" |],
        null,
        (match password with "" -> null | _ -> password),
        true
    )
    match result.Successful with
    | true ->
        let logger = (logging gamename)
        newSession.MessageLog.add_OnMessageReceived logger
            
        Some (newSession, logger)
    | false -> None
    
    
    

let removeSession (dbcontext: DBContext.Data) gamename =
    match memorySessions.TryGetValue gamename with
    | true, (session, logger) ->
        match session.Socket.Connected with
        | true -> session.Socket.DisconnectAsync() |> Async.AwaitTask |> Async.RunSynchronously
        | false -> ()

        session.MessageLog.remove_OnMessageReceived logger
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
        | Some (newSession, logger) ->
            memorySessions.Add(KeyValuePair(gamename, (newSession, logger)))
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
            | None -> failwith "todo"
            | Some (newSession, logger) -> memorySessions.Add(KeyValuePair(sess.GameName, (newSession, logger)))

        List.iter addsession dbSessions 
    }


let Router (dbcontext: DBContext.Data) =
    router {
        // ReSharper disable FSharpInterpolatedString
        getf "/%s" (failwith "todo")
        post "" (SessionAdd dbcontext)
        deletef "/%s" (removeSession dbcontext)
        get "" (htmlView Client.Admin.ArchipelagoSessions.Index.Page)

    }

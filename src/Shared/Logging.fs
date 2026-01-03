module ClassLibrary1.Logging

open System
open System.Globalization
open System.IO
open Microsoft.AspNetCore.Http
open Saturn.ControllerHelpers
open Giraffe.Core


type LoggingData =
    | Arr of LoggingData list
    | Obj of (string *LoggingData) list
    | Str of string
    | Num of float
    | Bool of bool
let UnboxLoggingData data =
    match data with
    | Str s -> s
    | Num n -> $"{n}"
    | Bool b -> if b then "true" else "false"
    | _ -> raise (InvalidDataException(data.ToString()))
let StringifyLoggingObj (log: LoggingData) =
    
    let collapseValues strings =
        if Seq.length strings > 0 then 
            Seq.reduce (fun a b -> a + "," + b) strings
        else
            ""
    let formatValue (str:string) =
        str.Replace ("\"","\\\"")
        |> fun x ->  "\"" + x + "\""
    let rec folder l =
        seq {
            match l with
            | Arr x ->
                yield $"""[{(collapseValues <| Seq.collect folder x)}]"""
            | Obj (keyvals) ->
                yield
                    seq {
                        for (key, value) in keyvals do
                            yield $$"""{{"\""}}{{key}}":{{collapseValues <| folder value}}"""
                        } |> collapseValues
                    |> fun x -> $$"""{{{x}}}""" 
                
            | Str x -> 
                yield formatValue x
            | i ->
                yield $"{UnboxLoggingData i}"
            }
    collapseValues (folder log)
let AddLoggingData (ctx:HttpContext) (path:string) (value: LoggingData) =
    let data = ctx.Items["LoggingData"] :?> LoggingData
    let partials = path.Split "." |> List.ofArray
    let rec insertData (currPart: LoggingData) (path: string list) (currKey:string) (data: LoggingData) =
        match path with
        | [] ->
            match currPart with
            | Arr x ->
                match data with
                | Arr d ->  Arr (List.append x d)
                | _ ->  Arr (List.append x [data])
            | Obj x -> Obj (List.append x [currKey, data]) 
            | _ -> Arr [currPart; data] 
        | h::t ->
            match currPart with
            | Arr x -> Arr (List.append x [data])
            | Obj x ->
                let nextKey = h
                if List.exists (fst >> (=) currKey) x then
                    seq {
                        for keyval in x do
                            match fst keyval = currKey with
                            | true ->
                                yield currKey, insertData (snd keyval) t nextKey data
                            | _ ->
                                yield keyval
                    } |> List.ofSeq
                else
                    List.append x [currKey, insertData (Obj []) t nextKey data]
                |> Obj
            | _ -> Arr [currPart; data] 
    ctx.Items["LoggingData"] <- insertData data (List.tail partials) (List.head partials) value
    
let logRequest path success (ctx:HttpContext) =
    AddLoggingData ctx "request.success" (Bool success)
    let loggingObj = ctx.Items["LoggingData"] :?> LoggingData
    let logLine = StringifyLoggingObj loggingObj
    if success then
        if Random.Shared.Next(0,100) < 100 then
            File.WriteAllText (path,logLine)
            Console.WriteLine logLine
    else
        File.WriteAllText (path,logLine)
        Console.WriteLine logLine
let withLogger (filepath: string) (next:HttpFunc) (ctx: HttpContext) =
    let requestInfo = Obj [
            "request", Obj [
                "path", Str ctx.Request.Path
                "method", Str ctx.Request.Method
                "time", Str (DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss",CultureInfo.InvariantCulture))
                ]
        ]
    ctx.Items.Add ("LoggingData", requestInfo)
    for param in ctx.Request.Query do
        for value in param.Value do
            AddLoggingData ctx $"request.queryparams.{param.Key}" (Str value)
    try 
        let res = next ctx
        AddLoggingData ctx "response.statusCode" (Num ctx.Response.StatusCode)
        logRequest filepath true ctx
        res
    with
    | e ->
        AddLoggingData ctx "Errors" (Str <| e.Message)
        logRequest filepath false ctx
        Response.internalError ctx e.Message

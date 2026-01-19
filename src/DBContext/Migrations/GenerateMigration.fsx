#!/usr/bin/env -S dotnet fsi

open System
open System.IO

[<Literal>]
let dateFormat = "yyyyMMddHHmmss"
printf "Migration Name: "
let name = Console.ReadLine()
let fileName = DateTime.UtcNow.ToString(dateFormat) + "_" + name + ".sql"
let path = Path.Combine(__SOURCE_DIRECTORY__, fileName)

File.WriteAllText(path, "-- Migration: " + name)
printfn "Created %s" path
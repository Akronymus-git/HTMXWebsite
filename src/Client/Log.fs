module Client.Log

open Giraffe.ViewEngine
let DisplayLogs (results: DBContext.Logs.Log seq) = html [] []

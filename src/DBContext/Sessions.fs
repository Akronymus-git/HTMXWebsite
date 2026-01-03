module Data.Sessions

open System
open Microsoft.Data.Sqlite


type Session =
    {
        Name: string;
        Id: int;
        Email: string;
    }

type Sessions (connection: SqliteConnection) =
    member val private Connection = connection
    member _.CreateSession userId (expires:DateTime) =
        let comm = connection.CreateCommand()
        let key = Guid.NewGuid()
        comm.CommandText <- "insert into Sessions (key, userId, expires) values ($key, $userId, $expires);"
        comm.Parameters.AddWithValue ("key",key.ToString()) |> ignore
        comm.Parameters.AddWithValue ("userId",userId) |> ignore
        comm.Parameters.AddWithValue ("expires",expires.ToString("yyyyMMDDhhmmss")) |> ignore
        async {
            Async.AwaitTask (comm.ExecuteNonQueryAsync()) |> ignore
            return key
        }
    member _.DeleteSession (key: Guid) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "delete Sessions where key = $key"
        comm.Parameters.AddWithValue ("key",key.ToString()) |> ignore
        async {
            Async.AwaitTask (comm.ExecuteNonQueryAsync())|> ignore
        }
    member _.DeleteAllUserSessions (userId: int) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "delete Sessions where userId = $userId"
        comm.Parameters.AddWithValue ("userId",userId) |> ignore 
        async {
            Async.AwaitTask (comm.ExecuteNonQueryAsync())|> ignore
        }
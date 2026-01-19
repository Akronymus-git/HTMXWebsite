module DBContext.Sessions

open System
open System.Globalization
open Data.SharedConsts
open Microsoft.Data.Sqlite


type Session =
    {
        key: string;
        userId: int;
        expires: DateTime;
    }

type Sessions (connection: SqliteConnection) =
    member val private Connection = connection
    member _.CreateSession userId (expires:DateTime) =
        let comm = connection.CreateCommand()
        let key = Guid.NewGuid()
        comm.CommandText <- "insert into Sessions (key, userId, expires) values ($key, $userId, $expires);"
        comm.Parameters.AddWithValue ("key",key.ToString()) |> ignore
        comm.Parameters.AddWithValue ("userId",userId) |> ignore
        comm.Parameters.AddWithValue ("expires",expires.ToString(DateTimeStorageFormat)) |> ignore
        async {
            Async.AwaitTask (comm.ExecuteNonQueryAsync()) |> ignore
            return key
        }
        
    member _.GetSession userId  =
        let comm = connection.CreateCommand()
        let key = Guid.NewGuid()
        comm.CommandText <- "select key, expires, userId from Sessions where userId = $userId and key = $key and expires > $expires"
        comm.Parameters.AddWithValue ("key",key.ToString()) |> ignore
        comm.Parameters.AddWithValue ("userId",userId) |> ignore
        comm.Parameters.AddWithValue ("expires",DateTime.UtcNow.ToString(DateTimeStorageFormat)) |> ignore
        task {
            let! reader = comm.ExecuteReaderAsync()
            match! reader.ReadAsync() with
            | false ->
                return None
            | true ->
                return Some {key = string reader["key"]; userId =  int (reader["userId"].ToString()); expires =  DateTime.ParseExact ((string reader["Email"]), DateTimeStorageFormat,CultureInfo.InvariantCulture) }
        }
    member _.DeleteSession (key: Guid) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "delete Sessions where key = $key and expires > $expires"
        comm.Parameters.AddWithValue ("key",key.ToString()) |> ignore
        comm.Parameters.AddWithValue ("expires",DateTime.UtcNow.ToString(DateTimeStorageFormat)) |> ignore
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
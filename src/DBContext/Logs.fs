module DBContext.Logs


open System
open System.Collections.Generic
open System.Globalization
open Data.SharedConsts
open Microsoft.Data.Sqlite



type Log =
    { TimeStamp: DateTime
      Data: string
      UserId: int option }

type Logs(connection: SqliteConnection) =
    member val private Connection = connection

    member _.insertLog text userId =
        let comm = connection.CreateCommand()

        comm.CommandText <-
            "insert into Logs (data, userId, timestamp) values ($data, $userid, $timestammp); select last_insert_rowid()"

        comm.Parameters.AddWithValue(
            "userid",
            match userId with
            | Some x -> x
            | None -> DBNull.Value
        )
        |> ignore

        comm.Parameters.AddWithValue("text", text) |> ignore
        comm.Parameters.AddWithValue("timestamp", text) |> ignore

        task {
            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }

    member _.getLogs limit offset =
        let comm = connection.CreateCommand()
        comm.CommandText <- "select data, userId, timestamp from Logs limit $limit offset $offset"
        comm.Parameters.AddWithValue("limit", limit) |> ignore
        comm.Parameters.AddWithValue("offset", offset) |> ignore

        task {
            let reader = comm.ExecuteReader()
            let results = System.Collections.Generic.List<Log>()

            while! reader.ReadAsync() do
                let userid =
                    match Int32.TryParse(string reader["userId"]) with
                    | false, _ -> None
                    | true, x -> Some x

                let timestamp =
                    DateTime.ParseExact(string reader["timestamp"], DateTimeStorageFormat, CultureInfo.InvariantCulture)

                results.Add
                    { Data = string (reader["data"])
                      UserId = userid
                      TimeStamp = timestamp }

            return results |> List.ofSeq
        }

    member _.DeleteAllUserSessions(userId: int) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "delete Sessions where userId = $userId"
        comm.Parameters.AddWithValue("userId", userId) |> ignore
        async { Async.AwaitTask(comm.ExecuteNonQueryAsync()) |> ignore }

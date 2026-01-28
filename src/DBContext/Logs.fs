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

    member _.insertLog text userId =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "insert into Logs (data, userId, timestamp) values ($data, $userid, $timestamp); select last_insert_rowid()"



            match userId with
            | Some x -> comm.Parameters.AddWithValue("$userid", x)|> ignore
            | None -> comm.Parameters.AddWithValue("$userid", DBNull.Value)|> ignore

            comm.Parameters.AddWithValue("$data", text) |> ignore
            comm.Parameters.AddWithValue("$timestamp", DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)) |> ignore

            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }

    member _.getLogs limit offset =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "select data, userId, timestamp from Logs order by timestamp desc limit $limit offset $offset"
            comm.Parameters.AddWithValue("limit", limit) |> ignore
            comm.Parameters.AddWithValue("offset", offset) |> ignore

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
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "delete Sessions where userId = $userId"
            comm.Parameters.AddWithValue("userId", userId) |> ignore
            let! _ = comm.ExecuteNonQueryAsync()
            ()
        }

module DBContext.Logs


open System
open System.Collections.Generic
open System.Globalization
open Data.SharedConsts
open Microsoft.Data.Sqlite



type Log =
    { TimeStamp: DateTime
      Data: string
      UserId: int option
      UserAgent: string option
      IP: string option
      StatusCode: int option
      Path: string option
      Success: string option
      Method: string option }

type Logs(connection: SqliteConnection) =

    member _.insertLog text userId userAgent ip statusCode path success method =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "insert into Logs (data, userId, timestamp, UserAgent, IP, StatusCode, Path, Success, Method)
                 values ($data, $userid, $timestamp, $userAgent, $ip, $statusCode, $path, $success, $method);
                 select last_insert_rowid()"




            comm.Parameters.AddWithValue("$data", text) |> ignore
            comm.Parameters.AddWithValue("$timestamp", DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)) |> ignore
            comm.Parameters.AddWithValue("$statusCode", statusCode) |> ignore

            match userId with
            | Some x -> comm.Parameters.AddWithValue("$userid", x) |> ignore
            | None -> comm.Parameters.AddWithValue("$userid", DBNull.Value) |> ignore
            
            match userAgent with
            | Some x -> comm.Parameters.AddWithValue("$userAgent", x) |> ignore
            | None -> comm.Parameters.AddWithValue("$userAgent", DBNull.Value) |> ignore

            match ip with
            | Some x -> comm.Parameters.AddWithValue("$ip", x) |> ignore
            | None -> comm.Parameters.AddWithValue("$ip", DBNull.Value) |> ignore

            match path with
            | Some x -> comm.Parameters.AddWithValue("$path", x) |> ignore
            | None -> comm.Parameters.AddWithValue("$path", DBNull.Value) |> ignore

            match success with
            | true -> comm.Parameters.AddWithValue("$success", "true") |> ignore
            | false -> comm.Parameters.AddWithValue("$success", "false") |> ignore
            match method with
            | Some m  -> comm.Parameters.AddWithValue("method", m) |> ignore
            | None -> comm.Parameters.AddWithValue("method", DBNull.Value) |> ignore

            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }

    member _.getLogs limit offset =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "select data, userId, timestamp, UserAgent, IP, StatusCode, Path, Success, Method
                 from Logs
                 order by timestamp desc
                 limit $limit offset $offset"

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

                let getOptString (col: string) =
                    match reader[col] with
                    | null -> None
                    | :? DBNull -> None
                    | v -> Some(string v)

                let getOptInt (col: string) =
                    match reader[col] with
                    | null -> None
                    | :? DBNull -> None
                    | v ->
                        match Int32.TryParse(string v) with
                        | true, x -> Some x
                        | _ -> None

                results.Add
                    { Data = string (reader["data"])
                      UserId = userid
                      TimeStamp = timestamp
                      UserAgent = getOptString "UserAgent"
                      IP = getOptString "IP"
                      StatusCode = getOptInt "StatusCode"
                      Path = getOptString "Path"
                      Success = getOptString "Success"
                      Method = getOptString "Method" }

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

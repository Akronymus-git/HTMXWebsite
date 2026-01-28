module DBContext.Sessions

open System
open System.Globalization
open Data.SharedConsts
open Microsoft.Data.Sqlite


type Session =
    { key: string
      userId: int
      expires: DateTime }

type Sessions(connection: SqliteConnection) =

    member _.CreateSession userId (expires: DateTime) =
        task {
            let comm = connection.CreateCommand()
            let key = Guid.NewGuid()
            comm.CommandText <- "insert into Sessions (key, userId, expires) values ($key, $userId, $expires);"
            comm.Parameters.AddWithValue("key", key.ToString()) |> ignore
            comm.Parameters.AddWithValue("userId", userId) |> ignore

            comm.Parameters.AddWithValue("expires", expires.ToString(DateTimeStorageFormat))
            |> ignore

            let! _ = comm.ExecuteNonQueryAsync()
            return key
        }

    member _.GetSession userId =
        task {
            let comm = connection.CreateCommand()
            let key = Guid.NewGuid()

            comm.CommandText <-
                "select key, expires, userId from Sessions where userId = $userId and key = $key and expires > $expires"

            comm.Parameters.AddWithValue("key", key.ToString()) |> ignore
            comm.Parameters.AddWithValue("userId", userId) |> ignore

            comm.Parameters.AddWithValue("expires", DateTime.UtcNow.ToString(DateTimeStorageFormat))
            |> ignore

            let! reader = comm.ExecuteReaderAsync()

            match! reader.ReadAsync() with
            | false -> return None
            | true ->
                return
                    Some
                        { key = string reader["key"]
                          userId = int (reader["userId"].ToString())
                          expires =
                            DateTime.ParseExact(
                                (string reader["Email"]),
                                DateTimeStorageFormat,
                                CultureInfo.InvariantCulture
                            ) }
        }

    member _.DeleteSession(key: Guid) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "delete Sessions where key = $key and expires > $expires"
            comm.Parameters.AddWithValue("key", key.ToString()) |> ignore

            comm.Parameters.AddWithValue("expires", DateTime.UtcNow.ToString(DateTimeStorageFormat))
            |> ignore

            let _ = comm.ExecuteNonQueryAsync()
            ()
        }

    member _.DeleteAllUserSessions(userId: int) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "delete Sessions where userId = $userId"
            comm.Parameters.AddWithValue("userId", userId) |> ignore
            let! _ = comm.ExecuteNonQueryAsync()
            ()
        }

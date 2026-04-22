module DBContext.ArchipelagoSessions

open System
open FSharp.Control
open Microsoft.Data.Sqlite

type ArchipelagoSessionRow =
    { GameName: string
      Uri: string
      Game: string
      Name: string
      Password: string }

type ArchipelagoSessions(connection: SqliteConnection) =

    member _.InsertSession(gameName: string, uri: string, game: string, name: string, password: string option) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "insert into ArchipelagoSessions (gamename, uri, game, name, password)
                 values ($gamename, $uri, $game, $name, $password)"

            comm.Parameters.AddWithValue("$gamename", gameName) |> ignore
            comm.Parameters.AddWithValue("$uri", uri) |> ignore
            comm.Parameters.AddWithValue("$game", game) |> ignore
            comm.Parameters.AddWithValue("$name", name) |> ignore

            match password with
            | Some p -> comm.Parameters.AddWithValue("$password", p) |> ignore
            | None -> comm.Parameters.AddWithValue("$password", DBNull.Value) |> ignore

            let! rowsAffected = comm.ExecuteNonQueryAsync()
            return rowsAffected > 0
        }

    member _.GetSession(gameName: string) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "select gamename, uri, game, name, password
                 from ArchipelagoSessions
                 where gamename = $gamename"

            comm.Parameters.AddWithValue("$gamename", gameName) |> ignore
            let! reader = comm.ExecuteReaderAsync()

            let! hasData = reader.ReadAsync()

            if hasData then
                return
                    Some
                        { GameName = string reader["gamename"]
                          Uri = string reader["uri"]
                          Game = string reader["game"]
                          Name = string reader["name"]
                          Password =
                            match reader["password"] with
                            | null
                            | :? DBNull -> ""
                            | v -> string v }
            else
                return None
        }

    member _.DeleteSession(gameName: string) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "delete from ArchipelagoSessions where gamename = $gamename"
            comm.Parameters.AddWithValue("$gamename", gameName) |> ignore
            let! rowsAffected = comm.ExecuteNonQueryAsync()
            return rowsAffected > 0
        }

    member _.GetAllSessions() =
        taskSeq {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "select gamename, uri, game, name, password
                 from ArchipelagoSessions
                 order by gamename"

            let! reader = comm.ExecuteReaderAsync()
            let results = System.Collections.Generic.List<ArchipelagoSessionRow>()

            while! reader.ReadAsync() do
                yield
                    { GameName = string reader["gamename"]
                      Uri = string reader["uri"]
                      Game = string reader["game"]
                      Name = string reader["name"]
                      Password =
                        match reader["password"] with
                        | null
                        | :? DBNull -> ""
                        | v -> (string v) }
        }

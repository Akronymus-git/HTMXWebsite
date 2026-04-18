module DBContext.Redirects

open FSharp.Control
open Microsoft.Data.Sqlite


type Redirect =
    { Id: int
      Source: string
      Target: string }

type Redirects(connection: SqliteConnection) =

    member _.getAllRedirects() =
        taskSeq {
            let comm = connection.CreateCommand()
            comm.CommandText <- "select Id, Source, Target from Redirects order by Id"
            let! reader = comm.ExecuteReaderAsync()
            while! reader.ReadAsync() do
                yield
                    { Id = reader.GetInt32(0)
                      Source = reader.GetString(1)
                      Target = reader.GetString(2) }

        }

    member _.getRedirectById(id: int) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "select Id, Source, Target from Redirects where Id = $id"
            comm.Parameters.AddWithValue("$id", id) |> ignore
            let! reader = comm.ExecuteReaderAsync()

            let! hasData = reader.ReadAsync()

            if hasData then
                return
                    Some
                        { Id = reader.GetInt32(0)
                          Source = reader.GetString(1)
                          Target = reader.GetString(2) }
            else
                return None
        }

    member _.getRedirectBySource(source: string) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "select Id, Source, Target from Redirects where Source = $source"
            comm.Parameters.AddWithValue("$source", source.ToLowerInvariant().TrimStart('/')) |> ignore
            let! reader = comm.ExecuteReaderAsync()
            let! hasData = reader.ReadAsync()
            if hasData then
                return Some { Id = reader.GetInt32(0)
                              Source = reader.GetString(1)
                              Target = reader.GetString(2) }
            else
                return None
        }

    member _.insertRedirect(source: string, target: string) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "insert into Redirects (Source, Target) values ($source, $target);
                 select last_insert_rowid()"

            comm.Parameters.AddWithValue("$source", source.ToLowerInvariant()) |> ignore
            comm.Parameters.AddWithValue("$target", target) |> ignore
            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }

    member _.updateRedirect(id: int, source: string, target: string) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "update Redirects set Source = $source, Target = $target where Id = $id"
            comm.Parameters.AddWithValue("$id", id) |> ignore
            comm.Parameters.AddWithValue("$source", source) |> ignore
            comm.Parameters.AddWithValue("$target", target) |> ignore
            let! rowsAffected = comm.ExecuteNonQueryAsync()
            return rowsAffected > 0
        }

    member _.deleteRedirect(id: int) =
        task {
            let comm = connection.CreateCommand()
            comm.CommandText <- "delete from Redirects where Id = $id"
            comm.Parameters.AddWithValue("$id", id) |> ignore
            let! rowsAffected = comm.ExecuteNonQueryAsync()
            return rowsAffected > 0
        }

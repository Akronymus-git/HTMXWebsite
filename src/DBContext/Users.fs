module DBContext.Users

open System
open Data.SharedConsts
open Microsoft.Data.Sqlite
open FSharp.Control

type User =
    { Name: string; Id: int; Email: string }

type Users(connection: SqliteConnection) =
    member val private Connection = connection

    member _.GetUserById(id: int) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "select * from Users where id = $id"
        comm.Parameters.AddWithValue("id", id) |> ignore

        task {
            let! reader = comm.ExecuteReaderAsync()

            match! reader.ReadAsync() with
            | false -> return None
            | true ->
                return
                    Some
                        { Name = string reader["Name"]
                          Id = int (reader["id"].ToString())
                          Email = string reader["Email"] }
        }
    member _.getUserList limit offset =
        let comm = connection.CreateCommand()
        comm.CommandText <- "select * from Users limit $limit offset $offset"
        comm.Parameters.AddWithValue("limit", limit) |> ignore
        comm.Parameters.AddWithValue("offset", offset) |> ignore
        taskSeq {
            use! reader = comm.ExecuteReaderAsync()
            while! reader.ReadAsync() do
                yield { 
                    Name = string reader["Name"]
                    Id = int (reader["id"].ToString())
                    Email = string reader["Email"] 
                }
        }
    member _.CreateUser name email password =
        let comm = connection.CreateCommand()

        comm.CommandText <-
            "insert into Users (Name, Email, Password) values ($name, $email, $password); select last_insert_rowid()"

        comm.Parameters.AddWithValue("name", name) |> ignore
        comm.Parameters.AddWithValue("email", email) |> ignore
        comm.Parameters.AddWithValue("password", password) |> ignore

        task {
            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }

    member _.FindUserBySession sessionId =
        let comm = connection.CreateCommand()
        let currDateTime = DateTime.UtcNow.ToString(DateTimeStorageFormat)

        comm.CommandText <-
            $"select u.* from Users as u inner join main.Sessions S on u.id = S.userId where s.key = $key and S.expires > $currDate"

        comm.Parameters.AddWithValue("key", sessionId) |> ignore
        comm.Parameters.AddWithValue("currDate", currDateTime) |> ignore

        task {
            let! reader = comm.ExecuteReaderAsync()

            match! reader.ReadAsync() with
            | false -> return None
            | true ->
                return
                    Some
                        { Name = string reader["Name"]
                          Id = int (reader["id"].ToString())
                          Email = string reader["Email"] }
        }
    member _.FindUserIdAndPasswordHashByName(name: string) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "select id, Password from Users where Name = $name"
        comm.Parameters.AddWithValue("name", name) |> ignore

        task {
            let! reader = comm.ExecuteReaderAsync()

            match! reader.ReadAsync() with
            | false -> return None
            | true ->
                let userId = int (reader["id"].ToString())
                let pwHash = string reader["Password"]
                return Some(userId, pwHash)
        }
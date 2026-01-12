module DBContext.Users

open System
open Microsoft.Data.Sqlite

type User =
    {
        Name: string;
        Id: int;
        Email: string;
    }

type Users (connection: SqliteConnection) =
    member val private Connection = connection
    member _.GetUserById (id: int) =
        let comm = connection.CreateCommand()
        comm.CommandText <- "select * from Users where id = $id"
        comm.Parameters.AddWithValue ("id",id) |> ignore
        task {
            let! reader = comm.ExecuteReaderAsync()
            match! reader.ReadAsync() with
            | false ->
                return None
            | true ->
                return Some {Name = string reader["Name"]; Id = int (reader["id"].ToString()); Email = string reader["Email"] }
        }
            
    member _.CreateUser name email password =
        let comm = connection.CreateCommand()
        comm.CommandText <- "insert into Users (Name, Email, Password) values ($name, $email, $password); select last_insert_rowid()"
        comm.Parameters.AddWithValue ("name",name) |> ignore
        comm.Parameters.AddWithValue ("email",email) |> ignore
        comm.Parameters.AddWithValue ("password",password) |> ignore
        task {
            let! res = comm.ExecuteScalarAsync()
            return res.ToString() |> int
        }
        
    member _.FindUserBySession sessionId =
        let comm = connection.CreateCommand()
        let currDateTime = DateTime.Now.ToString("YYYYMMDDhhmmss")
        comm.CommandText <- $"select u.* from Users as u inner join main.Sessions S on u.id = S.userId where s.key = $key and S.expires < '{currDateTime}'"
        comm.Parameters.AddWithValue ("key", sessionId) |> ignore
        task {
            let! reader = comm.ExecuteReaderAsync()
            match! reader.ReadAsync() with
            | false ->
                return None
            | true ->
                return Some {Name = string reader["Name"]; Id = int (reader["id"].ToString()); Email = string reader["Email"] }
        }
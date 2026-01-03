namespace DBContext

open DBContext.Users
open Data.Sessions
open Microsoft.Data.Sqlite

type Data (connection: SqliteConnection) =
    member val Users = Users connection
    member val Sessions = Sessions connection
    member val private Connection = connection
    member x.Open () =
        x.Connection.Open()

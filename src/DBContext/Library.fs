namespace DBContext

open DBContext.Permissions
open DBContext.Users
open DBContext.Logs
open DBContext.Sessions
open Microsoft.Data.Sqlite

type Data(connection: SqliteConnection) =
    member val Users = Users connection
    member val Sessions = Sessions connection
    member val Logs = Logs connection
    member val Permissions = Permissions connection
    member val private Connection = connection
    member x.Open() = x.Connection.Open()

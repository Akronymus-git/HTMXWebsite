namespace DBContext

open DBContext.ArchipelagoSessions
open DBContext.Permissions
open DBContext.Users
open DBContext.Logs
open DBContext.Sessions
open DBContext.Redirects
open Microsoft.Data.Sqlite

type Data(connection: SqliteConnection) =
    member val Users = Users connection
    member val Sessions = Sessions connection
    member val Logs = Logs connection
    member val Permissions = Permissions connection
    member val Redirects = Redirects connection
    member val ArchipelagoSessions = ArchipelagoSessions connection

    member x.Open() = connection.Open()

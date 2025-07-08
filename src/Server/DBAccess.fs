module Server.DBAccess
open System
open Dapper.FSharp.SQLite
open Microsoft.Data.Sqlite
[<Literal>]
let connectionString = "Data Source=data.db"
let connection = new SqliteConnection(connectionString)

Dapper.FSharp.SQLite.OptionTypes.register()

type Name = {
    id: Guid
    name: string
}
let nameTable = table<Name>
insert {
    into nameTable
    value {id = Guid.NewGuid(); name = "test" }
} |> connection.InsertAsync
|> ignore
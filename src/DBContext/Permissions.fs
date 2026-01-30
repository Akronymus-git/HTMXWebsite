module DBContext.Permissions

open FSharp.Control
open Microsoft.Data.Sqlite
open System.Collections.Generic

type Permission = { Id: int; Key: string; Name: string }

type Permissions(connection: SqliteConnection) =

    member _.GetUserPermissions(userId: int) =
        taskSeq {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "SELECT p.Id, p.Key, p.Name FROM Permissions p 
                 INNER JOIN UserPermissions up ON p.Id = up.PermissionId 
                 WHERE up.UserId = $userId"

            comm.Parameters.AddWithValue("$userId", userId) |> ignore

            let! reader = comm.ExecuteReaderAsync()

            while! reader.ReadAsync() do
                yield
                    { Id = reader.GetInt32(0)
                      Key = reader.GetString(1)
                      Name = reader.GetString(2) }
        }

    member _.GetPermissionsList() =
        taskSeq {
            let comm = connection.CreateCommand()
            comm.CommandText <- "SELECT Id, Key, Name FROM Permissions"

            use! reader = comm.ExecuteReaderAsync()

            while! reader.ReadAsync() do
                yield
                    { Id = reader.GetInt32(0)
                      Key = reader.GetString(1)
                      Name = reader.GetString(2) }
        }
    member _.AddPermissionToUser(userId: int, permissionId: int) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "INSERT INTO UserPermissions (UserId, PermissionId) 
                 VALUES ($userId, $permissionId)"

            comm.Parameters.AddWithValue("$userId", userId) |> ignore
            comm.Parameters.AddWithValue("$permissionId", permissionId) |> ignore

            let! rows = comm.ExecuteNonQueryAsync()
            return rows > 0
        }
    member _.RemovePermissionFromUser(userId: int, permissionId: int) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "delete from UserPermissions where userId = $userId and PermissionId = $Id"
            comm.Parameters.AddWithValue("$userId", userId) |> ignore
            comm.Parameters.AddWithValue("$Id", permissionId) |> ignore
            let! rows = comm.ExecuteNonQueryAsync()
            return rows > 0
        }
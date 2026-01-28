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

    member _.AddPermissionToUser(userId: int, permissionKey: string) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "INSERT INTO UserPermissions (UserId, PermissionId) 
                 VALUES ($userId, (SELECT Id FROM Permissions WHERE Key = $key))"

            comm.Parameters.AddWithValue("$userId", userId) |> ignore
            comm.Parameters.AddWithValue("$key", permissionKey) |> ignore

            let! rows = comm.ExecuteNonQueryAsync()
            return rows > 0
        }
    member _.RemovePermissionFromUser(userId: int, permissionKey: string) =
        task {
            let comm = connection.CreateCommand()

            comm.CommandText <-
                "delete from UserPermissions where userId = $userId and PermissionId = (SELECT Id FROM Permissions WHERE Key = $key)"
            comm.Parameters.AddWithValue("$userId", userId) |> ignore
            comm.Parameters.AddWithValue("$key", permissionKey) |> ignore
            let! rows = comm.ExecuteNonQueryAsync()
            return rows > 0
        }
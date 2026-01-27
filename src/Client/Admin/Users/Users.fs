module Client.Admin.Users.Users
open Giraffe.ViewEngine
type User = { Name: string; Email: string }

let View (users: User seq) = html [] []

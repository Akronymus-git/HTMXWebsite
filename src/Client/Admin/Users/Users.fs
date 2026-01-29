module Client.Admin.Users.Users
open Giraffe.ViewEngine
type User = { Name: string; Email: string }

let View (users: User seq) =
    html [] [
        for user in users do
            div [_style "display: flex; space-between: 20px;"] [
                Text user.Name
                br []
                Text user.Email
            ]
    ]

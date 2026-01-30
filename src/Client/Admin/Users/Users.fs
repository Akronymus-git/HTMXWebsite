module Client.Admin.Users.Users
open Giraffe.ViewEngine

let View (users: DBContext.Users.User seq) =
    html [] [
        for user in users do
            div [_style "display: flex; space-between: 20px;"] [
                a [_href $"/admin/users/{user.Id}"] [Text user.Name]
                br []
                Text user.Email
                
            ]
    ]

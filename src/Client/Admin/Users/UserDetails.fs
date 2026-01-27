module Client.Admin.Users.UserDetails

open Giraffe.ViewEngine

type Permission = { Name: string }

type UserData =
    { Name: string
      Email: string
      Permissions: Permission seq }

let View (userData: UserData) =
    html
        []
        [ div
              []
              [ Text userData.Name
                Text userData.Email
                ul
                    []
                    [ for p in userData.Permissions do
                          li [] [ Text p.Name ] ]

                ] ]

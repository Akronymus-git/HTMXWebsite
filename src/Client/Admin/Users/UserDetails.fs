module Client.Admin.Users.UserDetails

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx

type Permission = { Name: string; Id: int }

type UserData =
    { Id: int
      Name: string
      Email: string
      Permissions: Permission seq }

let View (userData: UserData) (addablePermissions: Permission seq) =
    html
        []
        [ div
              []
              [ Text userData.Name
                Text userData.Email
                ul
                    []
                    [ for p in userData.Permissions do
                          li
                              []
                              [ Text p.Name
                                form [_method "post"; _action $"/admin/users/{userData.Id}"] [
                                    input [_type "hidden"; _name "permissionId"; _value $"{p.Id}"]
                                    input [_type "hidden"; _name "method"; _value "delete"]
                                    button [_type "submit"] [Text "Delete"]
                                ]] ]
                ul
                    []
                    [ for p in addablePermissions do
                          li
                              []
                              [ Text p.Name
                                form [_method "post"; _action $"/admin/users/{userData.Id}"] [
                                    input [_type "hidden"; _name "permissionId"; _value $"{p.Id}"]
                                    input [_type "hidden"; _name "method"; _value "patch"]
                                    button [_type "submit"] [Text "Add"]
                                ]] ]
                ] ]

module Shared.Permission

[<Literal>]
let AdminStr = "admin"


let (|AdminUser|_|) x =
        match x with
        | Some n ->
            if Seq.exists (fun (x: DBContext.Permissions.Permission) -> x.Key = AdminStr) n then
                Some AdminUser
            else
                None
        | None -> None
        
let isAdmin ctx =
    match User.getPermissions ctx with
    | AdminUser -> true
    | _ -> false
module Server.Admin.Admin

open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Server.Admin.Logs
open Server.Admin.Users



let adminAuth =
    pipeline {
        plug (fun next ctx ->
            match Shared.User.getPermissions ctx with
            | Some p ->
                if Seq.exists (fun (x:DBContext.Permissions.Permission) -> x.Key = Shared.Permission.AdminStr) p then
                    next ctx
                else
                    (pipeline {
                        set_status_code StatusCodes.Status403Forbidden
                        // Optionally add: render_html (Client.Error.Forbidden)
                    }) next ctx    
            | None -> 
                (pipeline {
                    set_status_code StatusCodes.Status403Forbidden
                    // Optionally add: render_html (Client.Error.Forbidden)
                }) next ctx
        )
    }
    

    
let Router (dbcontext: DBContext.Data) =
    router {
        pipe_through (adminAuth)
        forward "/Logs" (Logs.Router dbcontext)
        forward "/Users" (Users.Router dbcontext)
    }


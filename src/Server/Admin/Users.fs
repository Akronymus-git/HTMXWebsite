module Server.Admin.Users

open Client.Admin.Users
open Client.Admin.Users.UserDetails
open Client.Admin.Users.Users
open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Saturn
open Shared

[<CLIMutable>]
type LogQuery = { limit: int; offset: int }


let userTable (context: DBContext.Data) next (ctx: HttpContext) =
    let query = ctx.BindQueryString<LogQuery>()
    task {
        let! (results: Users.User list) =
            context.Users.getUserList query.limit query.offset
            |> FSharp.Control.TaskSeq.map (fun x -> {User.Name = x.Name; Email = x.Email}) 
            |> FSharp.Control.TaskSeq.toListAsync
            
        return!
            ((pipeline {
                render_html (Users.View results)
                set_status_code 200
            }) next ctx)
    }
let userDetails (context: DBContext.Data) id next (ctx: HttpContext) =
    
    task {
            
        match! context.Users.GetUserById id with
        | None ->
            return!
                    ((pipeline {
                        render_html ( Client.Status_404.Page)
                        set_status_code 404
                    }) next ctx)
        | Some u ->
            match! context.Users.GetUserById u.Id with
            | None ->
                return!
                    ((pipeline {
                        render_html ( Client.Status_404.Page)
                        set_status_code 404
                    }) next ctx)
            | Some p-> 
                let (data: UserData) = {Email = u.Email; Name= u.Name; Permissions = []}
                return!
                    ((pipeline {
                        render_html (UserDetails.View data)
                        set_status_code 200
                    }) next ctx)
    }
let addPermissionToUser (context: DBContext.Data) userId next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        match form.TryGetValue "permissionKey" with
        | true, stringValues ->
            match stringValues.Count with
            | 0 -> ()
            | _ ->
                let! res = context.Permissions.AddPermissionToUser (userId, (stringValues[0]))
                ()
        | _ ->
            ()
        return! userDetails context userId next ctx
    }
 
let removePermissionFromUser (context: DBContext.Data) userId next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        match form.TryGetValue "permissionKey" with
        | true, stringValues ->
            match stringValues.Count with
            | 0 -> ()
            | _ ->
                let! res = context.Permissions.RemovePermissionFromUser (userId, (stringValues[0]))
                ()
        | _ ->
            ()
        return! userDetails context userId next ctx
    }   
let Router (dbcontext: DBContext.Data) =
    router {
        getf "%i" (userDetails dbcontext)
        postf "%i" (addPermissionToUser dbcontext)
        deletef "%i" (removePermissionFromUser dbcontext)
        get "" (userTable dbcontext)
    }

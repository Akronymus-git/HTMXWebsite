module Server.Admin.Users

open Client.Admin.Users
open Client.Admin.Users.UserDetails
open Client.Admin.Users.Users
open FSharp.Control
open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Saturn
open Shared

[<CLIMutable>]
type UsersQuery = { limit: int; offset: int }


let userTable (context: DBContext.Data) next (ctx: HttpContext) =
    let query = ctx.BindQueryString<UsersQuery>()

    task {
        let! (results: DBContext.Users.User list) =
            context.Users.getUserList query.limit query.offset
            |> FSharp.Control.TaskSeq.toListAsync

        return!
            ((pipeline {
                render_html (Users.View results)
                set_status_code 200
            })
                next
                ctx)
    }

let userDetails (context: DBContext.Data) id next (ctx: HttpContext) =

    task {


        match! context.Users.GetUserById id with
        | None ->
            return!
                ((pipeline {
                    render_html (Client.Status_404.Page)
                    set_status_code 404
                })
                    next
                    ctx)
        | Some p ->
            let! permissions = context.Permissions.GetUserPermissions id |> TaskSeq.toListAsync
            let! allPermissions = context.Permissions.GetPermissionsList() |> TaskSeq.toListAsync

            let filteredPermissions =
                allPermissions
                |> Seq.filter (fun (x: DBContext.Permissions.Permission) -> Seq.contains x permissions |> not)
                |> Seq.map (fun x -> { Name = x.Name; Id = x.Id })

            let (data: UserData) =
                { Id = id
                  Email = p.Email
                  Name = p.Name
                  Permissions =
                    Seq.map (fun (x: DBContext.Permissions.Permission) -> { Name = x.Name; Id = x.Id }) permissions }

            return!
                ((pipeline {
                    render_html (UserDetails.View data filteredPermissions)
                    set_status_code 200
                })
                    next
                    ctx)
    }

let addPermissionToUser (context: DBContext.Data) userId next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        match form.TryGetValue "permissionId" with
        | true, stringValues ->
            match stringValues.Count with
            | 0 -> ()
            | _ ->
                let! res = context.Permissions.AddPermissionToUser (userId, (stringValues[0] |> int))
                ()
        | _ ->
            ()
        return! userDetails context userId next ctx
    }

let removePermissionFromUser (context: DBContext.Data) userId next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        match form.TryGetValue "permissionId" with
        | true, stringValues ->
            match stringValues.Count with
            | 0 -> ()
            | _ ->
                let! res = context.Permissions.RemovePermissionFromUser (userId, (stringValues[0] |> int))
                ()
        | _ ->
            ()
        return! userDetails context userId next ctx
    }
let modifyUserPermission context userId next (ctx:HttpContext)=
    task {
        let! form = ctx.Request.ReadFormAsync()
        match form.TryGetValue "method" with
        | true, stringValues ->
            match stringValues[0] with
            | "delete" -> return! removePermissionFromUser context userId next ctx
            | "patch" ->  return! addPermissionToUser context userId next ctx
            | _ -> return! next ctx
        | _ -> return! next ctx
    }
let Router (dbcontext: DBContext.Data) =
    router {
        getf "/%i" (userDetails dbcontext)
        postf "/%i" (modifyUserPermission dbcontext)
        deletef "/%i" (removePermissionFromUser dbcontext)
        patchf "/%i" (addPermissionToUser dbcontext)
        get "" (userTable dbcontext)
        get "/" (userTable dbcontext)
    }

module Server.Admin.Redirects

open FSharp.Control
open Giraffe
open Microsoft.AspNetCore.Http
open Saturn
open Saturn.Router
open Giraffe.Core


let redirectList (dbcontext: DBContext.Data) next (ctx: HttpContext) =
    task {
        let! redirects =
            dbcontext.Redirects.getAllRedirects()
            |> FSharp.Control.TaskSeq.toListAsync
        return!
            ((pipeline {
                render_html (Client.Admin.Redirects.Index redirects)
                set_status_code 200
            })
                next
                ctx)
    }


let  createRedirect (dbcontext: DBContext.Data) next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        let source = string form["source"]
        let target = string form["target"]

        if System.String.IsNullOrWhiteSpace source || System.String.IsNullOrWhiteSpace target then
            return! redirectTo false "/admin/redirects" next ctx
        else
            let! _ = dbcontext.Redirects.insertRedirect(source, target)
            return! redirectTo false "/admin/redirects" next ctx
    }


let  details (dbcontext: DBContext.Data) id next (ctx: HttpContext) =
    task {
        let! redirect = dbcontext.Redirects.getRedirectById id

        match redirect with
        | None ->
            return!
                ((pipeline {
                    render_html (Client.Status_404.Page)
                    set_status_code 404
                })
                    next
                    ctx)
        | Some value ->
            return!
                ((pipeline {
                    render_html (Client.Admin.Redirects.Details value)
                    set_status_code 200
                })
                    next
                    ctx)
    }


let  updateRedirect (dbcontext: DBContext.Data) id next (ctx: HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        let source = string form["source"]
        let target = string form["target"]

        let! updated = dbcontext.Redirects.updateRedirect(id, source, target)

        if updated then
            return! redirectTo false $"/admin/redirects/{id}" next ctx
        else
            return! setStatusCode 404 next ctx
    }


let  deleteRedirect (dbcontext: DBContext.Data) id next (ctx: HttpContext) =
    task {
        let! deleted = dbcontext.Redirects.deleteRedirect id

        if deleted then
            return! redirectTo false "/admin/redirects" next ctx
        else
            return! setStatusCode 404 next ctx
    }


let Router (dbcontext: DBContext.Data) =
    router {
        get "" (redirectList dbcontext)
        get "/" (redirectList dbcontext)
        post "" (createRedirect dbcontext)
        post "/" (createRedirect dbcontext)
        getf "/%i" (details dbcontext)
        postf "/%i" (fun id next ctx ->
            task {
                let! form = ctx.Request.ReadFormAsync()

                match string form["method"] with
                | "patch" -> return! updateRedirect dbcontext id next ctx
                | "delete" -> return! deleteRedirect dbcontext id next ctx
                | _ -> return! next ctx
            })
    }

let HandleRedirect (dbcontext: DBContext.Data) (next: HttpFunc) (ctx: HttpContext) : HttpFuncResult =
    task {
        let path = ctx.Request.Path
        let! redirect = dbcontext.Redirects.getRedirectBySource path 
        return!
            match redirect with
            | None -> skipPipeline
            | Some value -> (redirectTo false value.Target next ctx)
    }
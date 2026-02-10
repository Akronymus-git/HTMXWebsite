module Server.Pipelines.Path

open Giraffe
open Microsoft.AspNetCore.Http

let WithPathNormalization next (ctx: HttpContext) =
    let path = ctx.Request.Path.Value
    let loweredPath = path.ToLowerInvariant()
    if path.EndsWith '/' && path.Length > 1  then
        redirectTo true (loweredPath.Substring(0, path.Length - 1)) earlyReturn ctx 
    else
        if (path <> loweredPath) then
            if ctx.Request.QueryString.HasValue then
                redirectTo true (loweredPath+ ctx.Request.QueryString.Value) earlyReturn ctx
            else
                redirectTo true loweredPath earlyReturn ctx
        else
            next ctx
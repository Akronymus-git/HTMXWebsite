module Server.Login
open System
open System.IO
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Xml
open BCrypt.Net
open Microsoft.AspNetCore.Http
open Microsoft.Data.Sqlite
open Microsoft.Extensions.Primitives
open Org.BouncyCastle.Crypto.Generators
open Saturn
open Saturn.Router
open Giraffe.Core
open FSharp.Data.Sql


let validateFields (username:string option) (password:string option) (email:string option)=
    seq {
        match username with
        | None -> yield "username", "Must not be empty"
        | Some u ->
            if u.Length < 5 then
                yield "username", "Min length: 5"
        if Option.isNone email then
            yield "email", "Must not be empty"
        match password with
        | None -> yield "password", "Must not be empty"
        | Some pw ->
            if pw.Length < 10 then
                yield "password", "Min length: 5"
            if Regex.IsMatch (pw,"[0-9]") |> not then
                yield "password", "Must contain digit"
            if Regex.IsMatch (pw, "[a-z]") |> not then
                yield "password", "Must contain lower case letter"
            if Regex.IsMatch (pw, "[A-Z]") |> not then
                yield "password", "Must contain upper case letter"
            if Regex.IsMatch (pw, "[!@#$%^&*()_+=-]") |> not then
                yield "password", "Must contain special character [!@#$%^&*()_+=-]"
    } |> List.ofSeq
    
let extractFieldsFromForm (form:IFormCollection) =
    let mapItem itm =
        Seq.tryHead (form.Item itm)
    mapItem "username", mapItem "password", mapItem "email"
let createUser (dbcontext: DBContext.Data) next (ctx:HttpContext) =
    task {
        let! form = ctx.Request.ReadFormAsync()
        let username, password, email = extractFieldsFromForm form
        let errors = validateFields username password email
        match errors with
        | [] ->
            let pw = BCrypt.HashPassword (password.Value)
            let! userid = dbcontext.Users.CreateUser (username.Value) (email.Value) pw 
            let! session = dbcontext.Sessions.CreateSession userid (DateTime.UtcNow.AddDays(31)) 
            let opts = new CookieOptions()
            opts.Expires <- DateTime.UtcNow.AddDays(31)
            opts.Secure <- true
            opts.HttpOnly <- true
            ctx.Response.Cookies.Append ("user-auth", session.ToString(), opts)
            return
                pipeline {
                    redirect_to false "/"
                } 
        | _ ->
            let fieldValues = List.concat [
                (Option.map (fun x -> "username", x) >> Option.toList) username
                (Option.map (fun x -> "email", x) >> Option.toList) email
            ]
        
            return
                pipeline {
                    set_status_code 400
                    render_html (Client.Login.RegisterFormWithErrors fieldValues errors)
                }
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> fun x -> x next ctx

let verifyEmail (dbcontext: DBContext.Data) next (ctx:HttpContext) =
    Giraffe.Core.earlyReturn ctx
let Router (dbcontext: DBContext.Data) =
    router {
        get "/register" (htmlView Client.Login.RegisterForm)
        post "/register" (createUser dbcontext)
        get "/verify" (verifyEmail dbcontext)
        get "" (htmlView (Client.Login.LoginForm (None)))
        post "" (htmlString "")
    }
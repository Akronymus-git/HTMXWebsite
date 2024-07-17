module Server

open Giraffe
open Microsoft.AspNetCore.Identity
open Saturn
open MyDbContext

[<CLIMutable>]
type RegisterModel = 
    {
        // it's okay this is capitalized. 
        Email : string
        Password : string
    }

let registerHandler : HttpHandler =
    fun next ctx -> 
        task {
            let userManager = ctx.GetService<UserManager<IdentityUser>>()
            let! form = ctx.TryBindFormAsync<RegisterModel>()
            match form with
            | Error _ -> 
                return! htmlView (Client.Index.registerPage ["Something went wrong, please try again"]) next ctx
            | Ok form -> 
                let user = IdentityUser(Email = form.Email, UserName = form.Email)

                let! result = userManager.CreateAsync(user, form.Password)

                if result.Succeeded then
                    return! redirectTo false "/Account/RegisterThanks" next ctx
                else
                    // result.Errors contains stuff like:
                    //  Passwords must have at least one non alphanumeric character.
                    //  Passwords must have at least one digit ('0'-'9').
                    //  Passwords must have at least one uppercase ('A'-'Z').
                    let errors = result.Errors |> Seq.map (fun e -> e.Description)  |> List.ofSeq
                    return! htmlView (Client.Index.registerPage errors) next ctx
        }
let webApp =
    router {
        get "/Account/Register" (htmlView (Client.Index.registerPage []))
        post "/Account/Register" registerHandler
        get "/" (text "Hello from SAFE!")
    }
let configServices services =
    configureDBContextService services
let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
        service_config configServices
    }

run app

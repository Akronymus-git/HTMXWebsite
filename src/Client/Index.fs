module Client.Index
open Giraffe.ViewEngine

let layout children =
    HtmlElements.body
        []
        children
    
let registerPage (errors : string list) = 
    [
        h1 [] [str "Please Register"]
        form [_method "post"] [
            input [_type "text"; _placeholder "email"; _name "email"] 
            input [_type "password"; _placeholder "password"; _name "password"] 
            input [_type "submit"; _value "Register"] 
        ]
        a [_href "/Account/Login"] [str "Already registered?"]
        ul [] (errors |> List.map (fun err -> li [] [str err]))
    ] |> layout

let thanksForRegisteringPage = 
    [
        p [] [ str "thanks for registering, we'll send you a confirmation email soon!" ]
        a [_href "/Account/Login"] [str "go back to login page"]
    ] |> layout
module Client.Login

open Giraffe.ViewEngine

let inputGrid = tag "input-grid"
let errorLabel = tag "error-label"
let errorCollection = tag "error-collection"
let _open = attr "open" "true"

let RegisterFormWithErrors values errors =
    let displayError field =
        let errors =
            List.where (fun x -> fst x = field) errors
            |> List.map (fun x -> errorLabel [] [ Text(snd x) ])

        match errors with
        | [] -> None
        | _ ->
            let elems = summary [] [ Text "Errors" ] :: errors
            Some(details [ _open ] elems)

    let createInput inputtype field lbl =
        seq {
            yield label [ _for field ] [ Text lbl ]

            yield
                input
                    [ _type inputtype
                      _id field
                      _name field
                      _required
                      _value (
                          match List.tryFind (fun x -> fst x = field) values with
                          | Some e -> snd e
                          | None -> ""
                      ) ]

            let errors = displayError field

            match errors with
            | None -> ()
            | Some e -> yield e

        }


    html
        []
        [ head
              []
              [ title [] [ Text "CapSim" ]
                HtmlElements.link [ _rel "stylesheet"; _href "/style.css" ]
                script [ _src "/htmx.min.js" ] [] ]
          body
              []
              [ form
                    [ _method "post" ]
                    [ inputGrid
                          []
                          (Seq.concat
                              [ createInput "text" "username" "Username"
                                createInput "email" "email" "E-Mail"
                                createInput "password" "password" "Password"
                                [ input [ _type "submit"; _value "Register"; _style "grid-column: span 2" ] ] ]
                           |> List.ofSeq) ] ] ]


let RegisterForm = RegisterFormWithErrors [] []

let LoginForm (username: string option) =
    let createInput inputtype field lbl value =
        seq {
            yield label [ _for field ] [ Text lbl ]
            yield
                input
                    [ _type inputtype
                      _id field
                      _name field
                      _required
                      _value (Option.defaultWith (fun () -> "") value) ]
        }


    html
        []
        [ head
              []
              [ title [] [ Text "CapSim" ]
                HtmlElements.link [ _rel "stylesheet"; _href "/style.css" ]
                script [ _src "/htmx.min.js" ] [] ]
          body
              []
              [ form
                    [ _method "post" ]
                    [ inputGrid
                          []
                          (Seq.concat
                              [ createInput "text" "username" "Username" username
                                createInput "password" "password" "Password" None
                                Option.map
                                    (fun _ ->
                                        details [ _open ] [ summary [] [ Text "Errors" ]; Text "Invalid Password" ])
                                    username
                                |> Option.toList
                                |> Seq.ofList
                                [ input [ _type "submit"; _value "Login"; _style "grid-column: span 2" ] ] ]
                           |> List.ofSeq) ] ] ]

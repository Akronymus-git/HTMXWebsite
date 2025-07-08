module Client.SharedView

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx



let linkBoosted path name =
    a [ _href path; _hxBoost; _hxTarget "[style=\"grid-area:main\"]" ] [ Text name ]

let link path name =
    a [ _href path;  ] [ Text name ]
let basicLayout = div [ _id "pagelayout" ]

let basicNavbar =
    [ div [ _id "navbar" ] [ linkBoosted "/" "Home"; linkBoosted "/about" "About"; linkBoosted "/randomStuff" "Random stuff"; link "/discord" "Discord"; link "/rpg" "Incremental Prototype" ] ]
 
let SharedViewLayout
    (pagelayout: XmlNode list -> XmlNode)
    (navbar: XmlNode list)
    (_title: string)
    (_body: XmlNode list)
    hxRequest
    =
    match hxRequest with
    | false ->
         html
              []
              [ head
                    []
                    [ title [] [ Text _title ]
                      HtmlElements.link [ _rel "stylesheet"; _href "style.css" ]
                      script [ _src "htmx.min.js" ] [] ]
                body
                    []
                    [ pagelayout
                          [ div [ _style "grid-area:navbar;" ] navbar
                            div [ _style "grid-area:main" ] _body
                            div [ _style "grid-area:sidebar" ] [] ] ] ]  
    | true ->
        div
              []
              [ div [ _hxSwapOob "title" ] [ Text _title ]
                div [ _style "grid-area:main" ] _body ] 


let basicPage = SharedViewLayout basicLayout basicNavbar

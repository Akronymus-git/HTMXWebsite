module Client.Wplace
open SharedView
open Giraffe.ViewEngine




let Wplace =
    html
        []
        [ head
              []
              [ title [] [ Text "Wplace image converter" ]
                HtmlElements.link [ _rel "stylesheet"; _href "style.css" ]
                script [ _src "wplace.js"; _type "module" ] [] ]
          body
              []
              [ basicLayout
                    [ div [ _style "grid-area:navbar;" ] basicNavbar
                      div [ _style "grid-area:main" ] [ div [ _id "elmish-app" ] [] ]
                      div [ _style "grid-area:sidebar" ] [] ] ] ]

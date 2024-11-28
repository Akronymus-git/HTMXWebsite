﻿module Client.Incremental
open SharedView
open Giraffe.ViewEngine




let Incremental
    =
    html
        []
        [ head
              []
              [ title [] [ Text "Incremental" ]
                HtmlElements.link [ _rel "stylesheet"; _href "style.css" ]
                script [ _src "bundle.js"; _type "module" ] [] ]
          body
              []
              [ basicLayout
                    [ div [ _style "grid-area:navbar;" ] basicNavbar
                      div [ _style "grid-area:main" ]
                        [div [_id "elmish-app"] [] ]
                      div [ _style "grid-area:sidebar" ] [] ] ] ]
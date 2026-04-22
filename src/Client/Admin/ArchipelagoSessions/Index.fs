module Client.Admin.ArchipelagoSessions.Index

open Giraffe.ViewEngine

let Page =
    html
        []
        [ head
              []
              [ title [] [ Text "Archipelago Sessions" ]
                HtmlElements.link [ _rel "stylesheet"; _href "/style.css" ] ]
          body
              []
              [ h1 [] [ Text "Archipelago Sessions" ]
                form
                    [ _method "post"; _action "/admin/archipelago" ]
                    [ label [] [ Text "Game name" ]
                      input [ _type "text"; _name "gamename"; _required ]
                      br []

                      label [] [ Text "URI" ]
                      input [ _type "text"; _name "uri"; _required ]
                      br []

                      label [] [ Text "Game" ]
                      input [ _type "text"; _name "game"; _required ]
                      br []

                      label [] [ Text "Name" ]
                      input [ _type "text"; _name "name"; _required ]
                      br []

                      label [] [ Text "Password" ]
                      input [ _type "password"; _name "password" ] 
                      br []

                      input [ _type "submit"; _value "Add Session" ] ] ] ]
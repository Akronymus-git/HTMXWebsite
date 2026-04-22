module Client.Admin.ArchipelagoSessions.Index

open DBContext
open Giraffe.ViewEngine

let Page (sessions: ArchipelagoSessions.ArchipelagoSessionRow list) =
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

                      input [ _type "submit"; _value "Add Session" ] ]
                div
                    [ _style "display:flex;flex-direction:column;" ]
                    [ for sess in sessions do
                          div
                              [ _style "display:flex;flex-direction:row;" ]
                              [
                                a [_href $"/admin/archipelago/{sess.GameName}" ] [ Text sess.GameName ]
                                form
                                    [ _method "post"; _action $"/admin/archipelago/{sess.GameName}" ]
                                    [ input [ _type "hidden"; _name "method"; _value "delete" ]
                                      button [ _type "submit" ] [ Text "Delete" ] ] ] ] ] ]

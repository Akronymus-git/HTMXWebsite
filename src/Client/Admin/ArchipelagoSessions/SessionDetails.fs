module Client.Admin.ArchipelagoSessions.SessionDetails

open DBContext
open Giraffe.ViewEngine

let Page (session: ArchipelagoSessions.ArchipelagoSessionRow ) =
    html
        []
        [ head
              []
              [ title [] [ Text "Archipelago Sessions" ]
                HtmlElements.link [ _rel "stylesheet"; _href "/style.css" ] ]
          body
              []
              [
                a [_href "/admin/archipelago"] [Text "back"]
                a [_href $"/admin/archipelago/{session.GameName}/file"] [Text "Get file"]
                h1 [] [ Text $"Archipelago Session {session.GameName}" ]
                br []
                Text session.Uri
                br []
                Text session.Game
                br []
                Text session.Name
                br []
                Text session.Password
                form
                    [ _method "post"; _action $"/admin/archipelago/{session.GameName}" ]
                    [
                      input [ _type "hidden"; _name "method"; _value "delete" ]
                      button [ _type "submit" ] [ Text "Delete" ] ] ] ]
                
                                

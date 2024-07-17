module Client.SharedView

open Giraffe.ViewEngine

let basicLayout = div [ _style """grid-template-areas: "navbar" "main";width:100%;""" ]
let basicNavbar =
    [div
        [_style "display:flex;flex-direction:row;background-color: #8888BB;"]
        [
            a
                [
                    _href "/"
                ]
                [Text "Home"]
        ]]

let SharedViewLayout (pagelayout: XmlNode list -> XmlNode) (navbar: XmlNode list) (_title: string) (_body: XmlNode list) =
    html
        []
        [
            head
                []
                [
                    title
                        []
                        [ Text _title ]
                    link [
                        _rel "stylesheet"
                        _href "style.css"
                    ]
                ]
            body
                []
                [ 
                    pagelayout
                        [
                            div [_style "grid-area:navbar;"] navbar
                            div [_style "grid-area:main"] _body
                        ]
                ]
        ]


let basicPage = SharedViewLayout basicLayout basicNavbar
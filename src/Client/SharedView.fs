module Client.SharedView

open Giraffe.ViewEngine



let link path name =
    a [_href path] [Text name]

let basicLayout = div [
    _id "pagelayout" 
]
let basicNavbar =
    [div
        [_id "navbar"]
        [
          link "/" "Home"
          link "/about" "About"
          link "/randomStuff" "Random stuff"
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
                    HtmlElements.link [
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
                            div [_style "grid-area:sidebar"] []
                        ]
                ]
        ]


let basicPage = SharedViewLayout basicLayout basicNavbar
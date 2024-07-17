module Client.About
open Giraffe.ViewEngine
let About =
    SharedView.basicPage
        "About"
        [
            div
                []
                [
                    a [_href "https://github.com/Akronymus-git/HTMXWebsite"; _target "blank"] [Text "Website source code"]
                ]
        ]
module Client.NotFound

open SharedView
open Giraffe.ViewEngine
let link = SharedView.linkBoosted
let NotFound =
    basicPage
        "Not found"
        [
            h1 [] [Text "Page not found"]
            link "/" "Return to main page"
        ] 
module Client.NotFound

open ClassLibrary1
open ClassLibrary1.Logging
open SharedView
open Giraffe.ViewEngine
let link = SharedView.linkBoosted
let NotFound ctx =
    AddLoggingData ctx "response.notfound" (LoggingData.Bool true) 
    basicPage
        "Not found"
        [
            h1 [] [Text "Page not found"]
            link "/" "Return to main page"
        ] 
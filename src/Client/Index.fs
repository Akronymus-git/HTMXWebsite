module Client.Index
open Giraffe.ViewEngine

        
        
let Index  =
    SharedView.basicPage "index"
        [
            h1
                [_style "background-color: grey;"]
                [
                    Text "welcome to my page"
                ]
        ]

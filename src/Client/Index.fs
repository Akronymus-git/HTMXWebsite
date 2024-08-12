module Client.Index
open Giraffe.ViewEngine

        
        
let Index  =
    SharedView.basicPage "index"
        [
            h1
                []
                [
                    Text "welcome to my page"
                ]
        ]

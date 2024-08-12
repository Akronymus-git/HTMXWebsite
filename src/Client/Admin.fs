module Client.Admin
open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx
let Index  =
    SharedView.basicPage
        "Admin"
        [
            h1
                []
                [
                    Text "welcome to my admin page"
                ]
        ]

let AddToken =
    SharedView.basicPage "Add token"
        [
            form
                []
                [
                    label [_for "name"] [ Text "Name" ]
                    input [_type "text"; _id "name"] 
                    label [_for "token"] [ Text "Token" ]
                    input [_type "text"; _id "token"]
                    input [_type "submit"; _value "submit"; _method "post"]
                    
                ]
        ]
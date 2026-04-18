module Client.Admin.Redirects
open DBContext.Redirects
open Giraffe.ViewEngine


let Index (redirects: Redirect seq) =
    html [] [
        body [] [
            h1 [] [ Text "Redirects" ]

            h2 [] [ Text "Add new redirect" ]
            form [_method "post"; _action "/admin/redirects"] [
                div [] [
                    label [] [ Text "Source" ]
                    input [_type "text"; _name "source"; _placeholder "/old-path"]
                ]
                div [] [
                    label [] [ Text "Target" ]
                    input [_type "text"; _name "target"; _placeholder "/new-path"]
                ]
                button [_type "submit"] [ Text "Add redirect" ]
            ]

            h2 [] [ Text "Existing redirects" ]
            table [] [
                thead [] [
                    tr [] [
                        th [] [ Text "Id" ]
                        th [] [ Text "Source" ]
                        th [] [ Text "Target" ]
                        th [] [ Text "Actions" ]
                    ]
                ]
                tbody [] [
                    for r in redirects do
                        tr [] [
                            td [] [ Text (string r.Id) ]
                            td [] [ Text r.Source ]
                            td [] [ Text r.Target ]
                            td [] [
                                a [_href $"/admin/redirects/{r.Id}"] [ Text "Details" ]
                            ]
                        ]
                ]
            ]
        ]
    ]

let Details (redirect: Redirect) =
    html [] [
        body [] [
            h1 [] [ Text $"Redirect #{redirect.Id}" ]

            form [_method "post"; _action $"/admin/redirects/{redirect.Id}"] [
                input [_type "hidden"; _name "method"; _value "patch"]

                div [] [
                    label [] [ Text "Source" ]
                    input [_type "text"; _name "source"; _value redirect.Source]
                ]
                div [] [
                    label [] [ Text "Target" ]
                    input [_type "text"; _name "target"; _value redirect.Target]
                ]
                button [_type "submit"] [ Text "Update" ]
            ]

            form [_method "post"; _action $"/admin/redirects/{redirect.Id}"] [
                input [_type "hidden"; _name "method"; _value "delete"]
                button [_type "submit"] [ Text "Delete" ]
            ]

            p [] [
                a [_href "/admin/redirects"] [ Text "Back to overview" ]
            ]
        ]
    ]
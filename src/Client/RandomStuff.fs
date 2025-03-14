module Client.RandomStuff

open Giraffe.ViewEngine
let link = SharedView.link

let RandomStuff =
    SharedView.basicPage
        "Random stuff"
        [ div
              [ _style "display:flex;flex-direction:column" ]
              [ h1 [] [ Text "This is where my random side projects go" ]
                a [_href "/murderBingo"] [Text "Murder bingo for DDRJake"]
                link "/isometricGridRandomizer" "Randomizer for Tytar"
                link "/tools" "tools"] ]

﻿module Client.RandomStuff

open Giraffe.ViewEngine
let link = SharedView.linkBoosted
let linkNoBoost = SharedView.link

let RandomStuff =
    SharedView.basicPage
        "Random stuff"
        [ div
              [ _style "display:flex;flex-direction:column" ]
              [ h1 [] [ Text "This is where my random side projects go" ]
                link "/isometricGridRandomizer" "Randomizer for Tytar"
                linkNoBoost "/tools" "tools" ] ]

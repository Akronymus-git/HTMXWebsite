module Client.Grid
open Giraffe.ViewEngine

let gridComponent rows cols =
    let generateGridCells  rows cols =
        seq {
            for x in 1..cols do
                for y in 1..rows ->
                    (div
                         [_style "display:flex;flex-direction:column;"]
                         [
                            span [] [Text $"r: {y}"]
                            span [] [Text $"c: {x}"]
                            ])
        } |> List.ofSeq
    div
        [ _style $"display:grid;grid-template: repeat({rows}, 1fr) / repeat({cols}, 1fr);aspect-ratio:1;min-width:max-content;width:max-content;gap:4px;transform: rotate(45deg) scale(1, 0.7)" ]
        (generateGridCells rows cols)

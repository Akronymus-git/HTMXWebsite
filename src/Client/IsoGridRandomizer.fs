module Client.IsoGridRandomizer

open System
open Giraffe.ViewEngine

let IsoGridRandomizer seed xSize ySize =
    let xs =
        match xSize with
        | s when s > 100 -> 100
        | s when s < 1 -> 1
        | s -> s
    let ys =
        match ySize with
        | s when s > 100 -> 100
        | s when s < 1 -> 1
        | s -> s
    let random = new Random(seed)
    let randx = random.Next(0,xs)
    let randy = random.Next(0,ys)
    SharedView.basicPage
        "IsoGridRandomier"
        [
            div [_class "isoGridContainer"; _style $"grid-template-columns: repeat({xSize},1fr);"] [
                for i in 0..xs - 1 do
                    for j in 0..ys - 1 do
                        match i,j with
                        | x, y when x = randx && y = randy -> div [_class "tile hit"; _style "aspect-ratio:1"] [ Text $"{i},{j}" ] 
                        | _ -> div [_class "tile grass"; _style "aspect-ratio:1"] [ Text $"{i},{j}" ]
            ]
        ]
        
        
let IsoGridForm =
    SharedView.basicPage
        "IsoGridRandomizer"
        [
            form [] [
                label [_for "x"] [Text "x"]
                input [_type "number"; _id "x"; _name "x"] 
                label [_for "y"] [Text "y"]
                input [_type "number"; _id "y"; _name "y"]
                input [_type "submit"; _value "get grid"]
            ]
        ]
module Client.IsoGridRandomizer

open System
open Giraffe.ViewEngine

let IsoGridRandomizer seed xSize ySize =
    
    let random = new Random(seed)
    let randx = random.Next(0,xSize)
    let randy = random.Next(0,ySize)
    SharedView.basicPage
        "IsoGridRandomier"
        [
            div [_class "isoGridContainer"; _style $"grid-template-columns: repeat({xSize + 1},1fr);"] [
                for i in 0..xSize  do
                    for j in 0..ySize do
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
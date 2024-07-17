module Client.IsoGridRandomizer

open System
open Giraffe.ViewEngine

let IsoGridRandomizer () =
    let size = 10
    let random = new Random((DateTimeOffset.UtcNow.ToUnixTimeSeconds() % (Int32.MaxValue |> int64)) |> int)
    Console.Write "Seed: "
    Console.WriteLine ((DateTimeOffset.UtcNow.ToUnixTimeSeconds() % (Int32.MaxValue |> int64)) |> int)
    let randx = random.Next(0,size)
    let randy = random.Next(0,size)
    SharedView.basicPage
        "IsoGridRandomier"
        [
            div [_class "isoGridContainer"; _style $"grid-template-columns: repeat({size + 1},1fr);"] [
                for i in 0..size  do
                    for j in 0..size do
                        match i,j with
                        | x, y when x = randx && y = randy -> div [_class "tile hit"; _style "aspect-ratio:1"] [ Text $"{i},{j}" ] 
                        | _ -> div [_class "tile grass"; _style "aspect-ratio:1"] [ Text $"{i},{j}" ]
            ]
        ]
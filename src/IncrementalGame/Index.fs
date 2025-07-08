module Index

open System
open Browser
open Elmish.Obsolete
open Feliz

open IncrementalGame.Storage

let localStoragecpy = WebStorage.localStorage
type Stats = { MaxHealth: float; CurrHealth: float }

type Model =
    { LastSave: DateTime
      Name: string
      Stats: Stats }

type Msg =
    | Tick of DateTime
    | Save
    | Load

open Elmish
let storage = StorageBuilder Browser.WebStorage.localStorage

let save (model: Model) =
    storage { storage.Save "Name" model.Name }

let loadFromLocalStorage () =
    storage {
        return
            { Name = storage.Load "Name" id "test"
              LastSave = storage.Load "LastTick" DateTime.Parse (DateTime.Now)
              Stats = { CurrHealth = 0; MaxHealth = 0 } }
    }

let init () = loadFromLocalStorage (), Cmd.Empty


let update msg (model: Model) =
    console.log msg

    match msg with
    | Tick curr ->
        match model.LastSave.AddMinutes 5 < curr with
        | true ->
            let newModel = { model with LastSave = curr }
            save newModel
            newModel, Cmd.Empty
        | false -> model, Cmd.Empty
    | Save ->
        save model
        model, Cmd.none
    | Load -> loadFromLocalStorage (), Cmd.none

let inventory = Interop.createElement "rpg-inventory"
let inventorySlot = Interop.createElement "rpg-inventory-slot"
let screen = Interop.createElement "rpg-screen"

let view (model: Model) dispatch =
    Html.div
        [ prop.children
              [ Html.text model.Name
                screen [ prop.children [inventory [ prop.children (List.replicate 50 (inventorySlot [])) ] ]]

                Html.button [ prop.text "save"; prop.onClick (fun _ -> dispatch Save) ]
                Html.button [ prop.text "load"; prop.onClick (fun _ -> dispatch Load) ] ] ]

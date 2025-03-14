module Index

open System
open Browser
open Elmish.Obsolete
open Feliz
open IncrementalGame.ItemsList
open IncrementalGame.Storage
let localStoragecpy = WebStorage.localStorage


type Stats =
    { MaxHealth: float
      CurrHealth: float }

type Model =
    { Value: int
      Total: int
      LastSave: DateTime
      TickDelay: int
      ItemListInGame: Item list
      Equipment: Item list
      Inventory: Item list}

type Msg =
    | Increment
    | Decrement
    | Tick of DateTime
    | Save
    | Load

open Elmish

let storage = StorageBuilder Browser.WebStorage.localStorage
let save model =
    storage {
        storage.Save "Value" model.Value
        storage.Save "Total" model.Total
        storage.Save "LastTick" model.LastSave
        storage.Save "TickDelay" model.TickDelay
    }
     
let loadFromLocalStorage () =
     storage {
        return {
            Value = storage.Load "Value" int 0
            Total = storage.Load "Total" int 0
            LastSave = storage.Load "LastTick" DateTime.Parse (DateTime.Now)
            TickDelay = storage.Load "TickDelay" int 1000
            Equipment = []
            Inventory = []
            ItemListInGame = [] 
        }
    }

let init () =
    loadFromLocalStorage(),
    Cmd.Empty


let update msg model =
    match msg with
    | Increment -> { model with Value = model.Value + 1 }, Cmd.Empty
    | Decrement -> { model with Value = model.Value - 1 }, Cmd.Empty
    | Tick curr ->
        match model.LastSave.AddMinutes 5 < curr with
        | true ->
            let newModel = 
                { model with
                    LastSave = curr
                    Total = model.Total + model.Value
                    TickDelay = match model.TickDelay with | x when x > 40 -> x - 1 | x -> x }
            save newModel
            newModel, Cmd.Empty
        | false -> 
           model ,Cmd.Empty
    | Save ->
        save model
        model, Cmd.none
    | Load ->
        loadFromLocalStorage(),Cmd.none

let view (model: Model) dispatch =
    Html.div
        [ prop.className "flex flex-col items-center justify-center h-full"
          prop.children
              [ Html.h1
                    [ prop.className "text-center text-5xl font-bold text-white mb-3 rounded-md p-4"
                      prop.text "Website" ]
                Html.text model.Total
                Html.div
                    [
                      prop.style [
                          style.display.flex
                          style.flexDirection.column
                          style.width.maxContent
                      ]
                      prop.children
                            [ Html.button [ prop.text "+"; prop.onClick (fun _ -> dispatch Increment) ]
                              Html.button [ prop.text "-"; prop.onClick (fun _ -> dispatch Decrement) ]] ]
                Html.text $"Delta {model.Value}" 
                Html.text $"Delay {model.TickDelay}"
                Html.button [prop.text "save"; prop.onClick (fun _ -> dispatch Save)] 
                Html.button [prop.text "load"; prop.onClick (fun _ -> dispatch Load)] ] ]

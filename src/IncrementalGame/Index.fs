module Index

open System
open Feliz

type Model =
    { Value: int
      Total: int
      LastTick: DateTime
      TickDelay: int }

type Msg =
    | Increment
    | Decrement
    | Tick of DateTime

open Elmish

let init () =
    { Value = 0
      Total = 0
      LastTick = DateTime.Now
      TickDelay = 1000 },
    Cmd.Empty


let update msg model =
    match msg with
    | Increment -> { model with Value = model.Value + 1 }, Cmd.Empty
    | Decrement -> { model with Value = model.Value - 1 }, Cmd.Empty
    | Tick curr ->
        { model with
            LastTick = curr
            Total = model.Total + model.Value
            TickDelay = match model.TickDelay with | x when x > 40 -> x - 1 | x -> x },
        Cmd.Empty


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
                      ]
                      prop.children
                            [ Html.button [ prop.text "+"; prop.onClick (fun _ -> dispatch Increment) ]
                              Html.button [ prop.text "-"; prop.onClick (fun _ -> dispatch Decrement) ]] ]
                Html.text $"Delta {model.Value}" 
                Html.text $"Delay {model.TickDelay}" ] ]

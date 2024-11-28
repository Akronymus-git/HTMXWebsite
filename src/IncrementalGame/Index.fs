module Index

open Feliz

type Model = { Value: int }

type Msg =
    | Increment
    | Decrement

open Elmish

let init () = { Value = 0 }, Cmd.Empty


let update msg model =
    match msg with
    | Increment when model.Value < 2 -> { model with Value = model.Value + 1 }, Cmd.Empty
    | Increment -> { model with Value = model.Value + 1 }, Cmd.Empty
    | Decrement when model.Value > 1 -> { model with Value = model.Value - 1 }, Cmd.Empty
    | Decrement -> { model with Value = model.Value - 1 }, Cmd.Empty


let view (model: Model) dispatch =


    Html.div
        [ prop.className "flex flex-col items-center justify-center h-full"
          prop.children
              [ Html.h1
                    [ prop.className "text-center text-5xl font-bold text-white mb-3 rounded-md p-4"
                      prop.text "Website" ]
                Html.text model.Value
                Html.button [ prop.text "+"; prop.onClick (fun _ -> dispatch Increment) ]
                Html.button [ prop.text "-"; prop.onClick (fun _ -> dispatch Decrement) ] ] ]

module App

open System
open Elmish
open Elmish.React

open Fable.Core
open Fable.Core.JsInterop
open Index
let timer onTick model =
    let start dispatch =
        let intervalId = 
            JS.setInterval
                (fun _ -> dispatch (onTick DateTime.Now))
                model.TickDelay
        { new IDisposable with
            member _.Dispose() = JS.clearInterval intervalId }
    start

let subscribe (model: Model) =
    [ ["timer"], timer Msg.Tick model ]

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withSubscription subscribe
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"

|> Program.run
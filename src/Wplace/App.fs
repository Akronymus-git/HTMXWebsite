module App

open System
open Elmish
open Elmish.React

open Fable.Core
open Fable.Core.JsInterop
open Index

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"

|> Program.run
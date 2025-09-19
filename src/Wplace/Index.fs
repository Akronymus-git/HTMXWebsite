module Index

open System.Text
open Browser
open Browser.Types
open Fable.Core
open Browser
open Feliz
open Feliz.style
open Feliz
open Fable.Core.JsInterop
open Fable.Core.JS
open Browser.Blob
open Microsoft.FSharp.Collections
open Fable.Core.Extensions

open Fable.Core.JsInterop

type Image =
    { Image: Blob
      Type: string
      Name: string
      DataUrl: string option }

type Model =
    { Name: string
      Image: Option<Image>
      SourceCanvas: CanvasRenderingContext2D option }

type Msg =
    | InitCanvas
    | HandleFileUploaded of Blob * string * string
    | HandleFileUploadedURL of byte[]

open Elmish

[<Emit("btoa($0)")>]
let toBase64String (bytes: byte[]) : string = failwith "JS"


let init () =
    { Name = ""
      Image = None
      SourceCanvas = None },
    Cmd.none

let handleFileEvent onReadArrayBuffer onReadDataURL (fileEvent: Browser.Types.Event) =
    let files: Browser.Types.FileList = !!fileEvent.target?files

    if files.length > 0 then
        let name = files[0].name
        let ``type`` = files[0].``type``
        let reader = Browser.Dom.FileReader.Create()
        reader.onload <- (fun _ -> reader.result |> unbox |> (fun x -> onReadArrayBuffer (x, name, ``type``)))
        reader.readAsArrayBuffer (files.[0])
        let reader2 = Browser.Dom.FileReader.Create()
        
        reader2.onload <-
            (fun e ->
                e.target?result
                |> unbox
                |> onReadDataURL)

        reader2.readAsDataURL (files.[0])

type ShadowDomInit (Mode: EncapsulationMode, delegateFocus: bool) =
    let mutable delFocus = delegateFocus
    let mutable currMode = Mode
    interface ShadowRootInit with
        member val delegatesFocus = delFocus with get, set
        member this.mode = currMode
        member this.mode with set value = currMode <- value

let update msg (model: Model) =
    match msg with
    | InitCanvas ->
        match model.SourceCanvas with
        | Some _ -> model, Cmd.none
        | None ->
            let sourceCanvas = document.getElementById ("sourceImage") :?> HTMLCanvasElement
            
            console.log sourceCanvas 
            let img = document.createElement "img" :?> HTMLImageElement
            img.id <- "srcImgHolder"  

            sourceCanvas.parentElement.appendChild (img) |> ignore
            match model.Image with
            | None -> ()
            | Some i ->
                match i.DataUrl with
                | Some x ->
                    img.src <- x
                | None -> ()

            console.log img.width
            sourceCanvas.width <- 100 
            sourceCanvas.height <- 100
            let ctx = sourceCanvas.getContext_2d ()
            ctx.fillStyle <- (!^ "red")
            ctx.fillRect (0,0,100,100)
            console.log img
            ctx.drawImage (!^img, 0, 0,200, 200, 0,0,100,100)
            ctx.moveTo (50, 50)
            
            ctx.lineWidth <- 50
            ctx.strokeStyle <- (!^ "blue")
            ctx.lineTo (75, 75)
            ctx.stroke ()
            { model with SourceCanvas = Some ctx }, Cmd.none
            
    | HandleFileUploaded(blob, filename, ``type``) ->
        { model with
            Name = filename
            Image =
                Some
                    { Image = blob
                      Type = ``type``
                      Name = filename
                      DataUrl = None } },
        Cmd.none
    | HandleFileUploadedURL str ->
        { model with
            Image =
                match model.Image with
                | Some x -> Some { x with DataUrl = Some ((new StringBuilder()).Append(str).ToString()) }
                | None -> None },
        Cmd.ofMsg InitCanvas


let view (model: Model) dispatch =
    Html.div
        [ prop.children
              [ Html.text "Wplace image converter"
                Html.canvas
                    [ prop.id "sourceImage"
                      prop.style (
                          match model.Image with
                          | Some _ -> []
                          | None -> [ style.display.none ]
                      ) ]
                match model.Image with
                | Some image -> Html.none
                | None ->
                    Html.input
                        [ prop.type' "file"
                          prop.label "choose an image"
                          prop.onChange (
                              handleFileEvent (HandleFileUploaded >> dispatch) (HandleFileUploadedURL >> dispatch)
                          ) ]

                ] ]

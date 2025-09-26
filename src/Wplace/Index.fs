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
      SourceCanvas: CanvasRenderingContext2D option
      Layers: (CanvasRenderingContext2D * bool) list }

type Msg =
    | InitCanvas
    | AddLayer
    | DeleteLayer of int
    | HandleFileUploaded of Blob * string * string
    | HandleFileUploadedURL of string
    | ProcessImage
let insertlog x =
    console.log x
    x
let validColors =
    [ "#9c8431"
      "#000000"
      "#3c3c3c"
      "#787878"
      "#aaaaaa"
      "#d2d2d2"
      "#ffffff"
      "#600018"
      "#a50e1e"
      "#ed1c24"
      "#fa8072"
      "#e45c1a"
      "#ff7f27"
      "#f6aa09"
      "#f9dd3b"
      "#fffabc"
      "#c5ad31"
      "#e8d45f"
      "#4a6b3a"
      "#5a944a"
      "#84c573"
      "#0eb968"
      "#13e67b"
      "#87ff5e"
      "#0c816e"
      "#10aea6"
      "#13e1be"
      "#0f799f"
      "#60f7f2"
      "#bbfaf2"
      "#28509e"
      "#4093e4"
      "#7dc7ff"
      "#4d31b8"
      "#6b50f6"
      "#99b1fb"
      "#4a4284"
      "#7a71c4"
      "#b5aef1"
      "#780c99"
      "#aa38b9"
      "#e09ff9"
      "#cb007a"
      "#ec1f80"
      "#f38da9"
      "#9b5249"
      "#d18078"
      "#fab6a4"
      "#684634"
      "#95682a"
      "#dba463"
      "#7b6352"
      "#9c846b"
      "#d6b594"
      "#d18051"
      "#f8b277"
      "#ffc5a5"
      "#6d643f"
      "#948c6b"
      "#cdc59e"
      "#333941"
      "#6d758d"
      "#b3b9d1" ]
    |> fun x ->
        console.log x
        x
    |> List.map (_.Substring(1))
    |> fun x ->
        console.log x
        x
    |> List.map (fun (x:string) -> Seq.chunkBySize 2 x |> Seq.toList |> List.map (fun (y: char array) -> Seq.map string y |> Seq.fold (+) "" |> fun i -> parseInt i 16 |> uint8))
    |> fun x ->
        console.log x
        x


open Elmish


let init () =
    { Name = ""
      Image = None
      SourceCanvas = None
      Layers = [] },
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

        reader2.onload <- (fun e -> e.target?result |> unbox |> onReadDataURL)

        reader2.readAsDataURL (files.[0])



let update msg (model: Model) =
    match msg with
    | InitCanvas ->
        console.log "initcanvas"

        match model.SourceCanvas with
        | Some _ -> model, Cmd.none
        | None ->
            let sourceCanvas = document.getElementById ("targetImage") :?> HTMLCanvasElement
            let img = document.getElementById "sourceImage" :?> HTMLImageElement
            sourceCanvas.width <- img.width
            sourceCanvas.height <- img.height
            let ctx = sourceCanvas.getContext_2d ()
            ctx.drawImage (!^img, 0, 0, img.width, img.height, 0, 0, sourceCanvas.width, sourceCanvas.height)

            { model with
                SourceCanvas = Some ctx
                Layers = [] },
            Cmd.ofMsg AddLayer
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
        let img = document.getElementById "sourceImage" :?> HTMLImageElement
        img.src <- str

        { model with
            Image =
                match model.Image with
                | Some x ->
                    Some
                        { x with
                            DataUrl = Some((new StringBuilder()).Append(str).ToString()) }
                | None -> None },
        Cmd.none
    | AddLayer ->
        let newLayer = document.createElement "canvas" :?> HTMLCanvasElement

        match model.SourceCanvas with
        | Some x ->
            newLayer.width <- x.canvas.width
            newLayer.height <- x.canvas.height
        | _ -> ()

        let ctx = newLayer.getContext_2d ()

        { model with
            Layers = (List.append model.Layers [ ctx, true ]) },
        Cmd.none
    | DeleteLayer i ->
        let newLayers =
            List.mapi (fun idx elem -> (idx, elem)) model.Layers
            |> List.choose (fun x -> if fst x <> i then Some(snd x) else None)

        { model with Layers = newLayers }, Cmd.none
    | ProcessImage ->
        match model.Layers, model.SourceCanvas with
        | h :: t, Some source ->
            console.log "process"
            let src = (source.getImageData (0, 0, source.canvas.width, source.canvas.height))
            let data = src.data

            for idx in 0..4 .. data.Length - 4 do
                let mutable closestMatch = 2.0**30
                let mutable cRed = (uint8) 0
                let mutable cBlue = (uint8) 0
                let mutable cGreen = (uint8) 0
                let exponent = 1.5
                for color in validColors do

                    let difference =
                        (Math.pow (exponent,(Math.abs ((float)(color[0] - data[idx]))))
                        + Math.pow (exponent,(Math.abs ((float)(color[1] - data[idx+1]))))
                        + Math.pow (exponent,(Math.abs ((float)(color[2] - data[idx+2])))))

                    if difference <= closestMatch then
                        closestMatch <- difference
                        cRed <- color[0]
                        cBlue <- color[1]
                        cGreen <- color[2]
                if (idx > 990 && idx < 1000) then
                    console.log cRed
                    console.log cBlue
                    console.log cGreen
                data[idx] <- cRed
                data[idx + 1] <- cBlue
                data[idx + 2] <- cGreen

            let lay = fst h
            lay.clearRect (0, 0, source.canvas.width, source.canvas.height)
            lay.putImageData (src, 0, 0,0,0,src.width, src.height)
            ()
        | _ -> ()

        model, Cmd.none


let view (model: Model) dispatch =
    Html.div
        [ prop.children
              [ Html.text "Wplace image converter"
                Html.img
                    [ prop.id "sourceImage"
                      prop.onLoad (fun (_: Event) -> InitCanvas |> dispatch)
                      prop.style [ style.display.none ] ]
                Html.button [ prop.onClick (fun _ -> dispatch ProcessImage); prop.text "abc" ]
                Html.canvas
                    [ prop.id "targetImage"
                      prop.style (
                          match model.Image with
                          | Some _ -> []
                          | None -> [ style.display.none ]
                      ) ]
                for c in model.Layers do
                    match snd c with
                    | false -> Html.none
                    | true ->
                        let ctx = fst c
                        Html.canvas
                            [
                              prop.onClick (fun e ->
                                  (e.target :?> HTMLCanvasElement)
                                      .getContext_2d()
                                      .putImageData (
                                          ctx.getImageData (0, 0, ctx.canvas.height, ctx.canvas.height),
                                          0,
                                          0
                                      ))
                              match model.SourceCanvas with
                              | Some c ->
                                console.log $"w{c.canvas.width}" 
                                prop.width c.canvas.width
                              | None ->
                                prop.width (length.px 100)
                                
                              match model.SourceCanvas with
                              | Some c ->
                                console.log $"h{c.canvas.height}"
                                prop.height c.canvas.height
                              | None ->
                                prop.height (length.px 100)
                            ]
                match model.Image with
                | Some image -> Html.none
                | None ->
                    Html.input
                        [ prop.type' "file"
                          prop.label "choose an image"
                          prop.accept "image/*"
                          prop.onChange (
                              handleFileEvent (HandleFileUploaded >> dispatch) (HandleFileUploadedURL >> dispatch)
                          ) ] ] ]

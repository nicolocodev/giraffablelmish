module Client.Counter

open Fable.Core
open Fable.Import
open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props

open System
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

type Model = { Count : int }

type Messages = Incr | Decr 

let update msg model = 
    match msg with
    | Incr -> { Count = model.Count + 1 }
    | Decr -> { Count = model.Count - 1 }

// Helper function, a string is a valid ReactElement
let text (content: string) = unbox content
// Helper function, for the initial model
let init() = { Count = 0 }, Cmd.none

let view model dispatch = 
    div 
      []
      [
          button [ OnClick (fun _ -> dispatch Incr) ] [ text "Increment" ]
          div [] [ text (string model.Count) ]
          button [ OnClick (fun _ -> dispatch Decr) ] [ text "Decrement"]
      ]
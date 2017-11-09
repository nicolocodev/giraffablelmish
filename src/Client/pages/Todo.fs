module Client.Todo

open Fable.Core
open Fable.Import
open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props

open System
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open WebApp.Models

type Model = {
    Todos : Types.Todo list
    NewText : string
}

type Messages =
| SetText of string
| NewTask
| TaskAdded of Types.Todo
| TaskError of exn
| Done of Guid

let addTask text =
     promise {

         let task = Todo.create text

         let body = toJson task

         let props = 
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        
        try
            let url = "http://localhost:5000" + ServerUrls.TodoList
            let! response = Fetch.fetch url props

            if not response.Ok then
                return! failwithf "Error: %d" response.Status
            else    
                let! data  = response.text()
                return (data |> ofJson : Types.Todo)
        with
        | _ -> return! failwithf "Could not authenticate user."
     }
let addTaskCmd task =
    Cmd.ofPromise addTask task TaskAdded TaskError

let update msg model = 
    match msg with
    | SetText text -> { model with NewText = text }, Cmd.none
    | NewTask -> model, addTaskCmd model.NewText
    | TaskAdded task -> {Todos = task::model.Todos;NewText=""}, Cmd.none
    | TaskError (_) -> failwith "Not Implemented"
    | Done (_) -> failwith "Not Implemented"
    

// Helper function, a string is a valid ReactElement
let text (content: string) = unbox content
// Helper function, for the initial model
let init() = 
    let t = Todo.create "Hola mundo"
    {Todos = [t];NewText=""}

let view (model:Model) dispatch =  

    let todos = 
        model.Todos 
        |> List.map(fun t -> 
            div [ClassName "row"] [
                text t.Text
                button [
                    Id (t.Id.ToString()) 
                    OnClick (fun ev -> dispatch (Done !!ev.target?id))] 
                    [ text "Done"]
                ])

    div [ClassName "text-center"] [
        yield div [] [
            input [
                ClassName "form-control input-lg"
                Placeholder "What needs to be done?"
                OnChange (fun ev -> dispatch (SetText !!ev.target?value))
            ]
            button [ OnClick (fun _ -> dispatch NewTask) ] [ text "Add"]
        ]
        yield! todos
    ]


    
  


    
    
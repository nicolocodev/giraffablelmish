module Client.App

open Fable.Core
open JsInterop

open Fable.Import
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.HMR
open Pages

importSideEffects "whatwg-fetch"
importSideEffects "babel-polyfill"

// Model

type PageModel =
    | HomePageModel
    | CounterPageModel of Counter.Model
    | TodoPageModel of Todo.Model

type Msg =
    | OpenTodo
    | OpenCounter
    | TodoMsg of Todo.Messages
    | CounterMsg of Counter.Messages
    | MenuMsg of Menu.Msg

type Model =
  { Menu : Menu.Model
    PageModel : PageModel }

let urlUpdate (result:Page option) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        ( model, Navigation.modifyUrl (toHash Page.Home) )

    | Some Page.Counter ->
        let m = Counter.init()
        { model with PageModel = CounterPageModel m}, Cmd.none

    | Some Page.Todo -> 
        let m = Todo.init()
        {model with PageModel = TodoPageModel m}, Cmd.none

    | Some Page.Home ->
        { model with PageModel = HomePageModel }, Cmd.none
    

let init result =
    let menu,menuCmd = Menu.init()
    let m =
        { Menu = menu
          PageModel = HomePageModel }

    let m,cmd = urlUpdate result m
    m,Cmd.batch[cmd; menuCmd]

let update msg model =
    match msg, model.PageModel with

    | OpenCounter, _ ->
        let m = Counter.init()
        { model with PageModel = CounterPageModel m}
        , Navigation.newUrl (toHash Page.Counter)

    | CounterMsg msg, CounterPageModel cpm -> 
        let m = Counter.update msg cpm
        { model with PageModel = CounterPageModel m}, Cmd.none

    | OpenTodo, _ ->
        let m = Todo.init()
        { model with PageModel = TodoPageModel m}, Navigation.newUrl (toHash Page.Todo)

    | TodoMsg msg, TodoPageModel tpm -> 
        let m,c = Todo.update msg tpm
        let cmd = Cmd.map TodoMsg c
        { model with PageModel = TodoPageModel m}, cmd

    | _, _ -> model,Cmd.none    
// VIEW

open Fable.Helpers.React
open Style

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel -> Home.view ()
    | CounterPageModel m -> [ Counter.view m (CounterMsg >> dispatch) ]
    | TodoPageModel m -> [ Todo.view m (TodoMsg >> dispatch) ]

/// Constructs the view for the application given the model.
let view model dispatch =
  div []
    [ Menu.view model.Menu (MenuMsg >> dispatch)
      hr []
      div [ centerStyle "column" ] (viewPage model dispatch)
    ]

open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.toNavigable Pages.urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

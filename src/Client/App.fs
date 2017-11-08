module Client.App

open Fable.Core
open Fable.Core.JsInterop

open Fable.Import
open Elmish
open Elmish.React
open Fable.Import.Browser
open Elmish.Browser.Navigation
open Elmish.HMR
open Client.Pages

JsInterop.importSideEffects "whatwg-fetch"
JsInterop.importSideEffects "babel-polyfill"

// Model

type PageModel =
    | HomePageModel
    | CounterPageModel of Counter.Model

type Msg =
    | OpenCounter
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
        let m,cmd = Counter.init()
        { model with PageModel = CounterPageModel m}, cmd

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
        let m,cmd = Counter.init()
        { model with PageModel = CounterPageModel m}
        , Cmd.batch [cmd; Navigation.newUrl (toHash Page.Counter) ]
    | CounterMsg msg, CounterPageModel cpm -> 
        let m = Counter.update msg cpm
        { model with PageModel = CounterPageModel m}, Cmd.none
    |_, _ -> model,Cmd.none    
// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.PageModel with
    | HomePageModel -> Home.view ()
    | CounterPageModel m ->
        [ Counter.view m (fun msg -> dispatch(CounterMsg msg)) ] //(CounterMsg >> dispatch)

/// Constructs the view for the application given the model.
let view model dispatch =
  div []
    [ Menu.view model.Menu (MenuMsg >> dispatch)
      hr []
      div [ centerStyle "column" ] (viewPage model dispatch)
    ]

open Elmish.React
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

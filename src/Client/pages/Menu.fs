module Client.Menu

open Elmish
open Fable.Helpers.React
open Client.Style
open Client.Pages

/// The user data sent with every message.
type UserData = 
  { UserName : string }

type Model = {
    User : UserData option
}

type Msg =
    | Logout

let init() = { User = Utils.load "user" }, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg> = 
    match msg with
    | Logout ->
        model, Cmd.none

let view (model:Model) dispatch =
    div [ centerStyle "row" ] [
          yield viewLink Page.Home "Home"
         
          yield viewLink Page.Counter "Counter" 

          yield viewLink Page.Todo "Todo" 
        ]
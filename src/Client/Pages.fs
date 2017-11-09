module Client.Pages

open Elmish.Browser.UrlParser

/// The different pages of the application. If you add a new page, then add an entry here.
[<RequireQualifiedAccess>]
type Page = 
    | Home
    | Counter
    | Todo

let toHash =
    function
    | Page.Home -> "#home"
    | Page.Counter -> "#counter"
    | Page.Todo -> "#todo"

/// The URL is turned into a Result.
let pageParser : Parser<Page -> Page,_> =
    oneOf
        [ 
            map Page.Home (s "home")
            map Page.Counter (s "counter")
            map Page.Todo (s "todo")
        ]
        
let urlParser location = parseHash pageParser location
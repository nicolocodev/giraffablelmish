namespace WebApp.Models

open System

module Types =

    [<CLIMutable>]
    type Todo = 
        {
            Id : Guid
            Done : bool 
            Text : string
        }

module Todo = 

    open Types
    let create text =
        {
            Id = Guid.NewGuid()
            Done = false
            Text = text
        }

module ServerUrls =

    [<Literal>]
    let TodoList = "/todo"
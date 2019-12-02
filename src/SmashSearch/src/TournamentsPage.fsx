#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Shared

let TournamentsPage =
    FunctionComponent.Of (
        (fun () -> div [] [
                        h1 [] [str "TournamentsPage"]
                        button [] [ A Route.Root [str "Go Back Home"] ]
                   ]
        )
        , "TournamentsPage")
    
exportDefault TournamentsPage
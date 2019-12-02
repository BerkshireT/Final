#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Shared

// I wanna add a brief overview of how this works to this page
// Link it to github and whatnot as well
let HomePage =
    FunctionComponent.Of (
        (fun () -> div [] [
                        h1 [] [str "HOME PAGE Goes here"]
                        button [] [ A (Route.Detail 2) [str "Detail"] ]
                        br []
                        br []
                        button [] [ A (Route.Tournaments "test") [str "Tournaments"] ]
                   ]
        )
        , "HomePage")
    
exportDefault HomePage

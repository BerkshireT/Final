//******************************************************************************
//
//      filename:  HomePage.fsx
//
//      description:  Page for project info and basic usage
//
//       author:  Berkshire, Tyler P.
//
//       Copyright (c) 2019 Tyler P Berkshire, University of Dayton
//******************************************************************************

#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Shared

// TODO I wanna add a brief overview of how this works to this page
// TODO Link it to github and whatnot as well
// TODO Update readme as well with pretty much the same info
let HomePage =
    FunctionComponent.Of (
        (fun () -> div [] [
                        h1 [] [str "HOME PAGE Goes here"]
                        br []
                        br []
                        button [] [ A (Route.Tournaments) [str "Tournaments"] ]
                   ]
        )
        , "HomePage")
    
exportDefault HomePage

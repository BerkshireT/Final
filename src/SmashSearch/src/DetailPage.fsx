#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Shared

let DetailPage =
    FunctionComponent.Of (
        (fun () -> div [] [
                        h1 [] [str "DETAIL PAGE"]
                        (*button [OnClick (fun _ -> Msg.Navigate Route.Root |> useDispatch() )] [str "Go Back to Home"]*)
                        button [] [
                            A Route.Root [str "Go Back Home"]
                        ]
                   ]
        )
        , "DetailPage")
    
exportDefault DetailPage
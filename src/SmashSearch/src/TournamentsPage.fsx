#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Shared

let TournamentsPage =
    FunctionComponent.Of (
        (fun () ->
            let model = useModel()
            let dispatch = useDispatch()
            div [] [
                h1 [] [str "TournamentsPage"]
                button [] [ A Route.Root [str "Go Back Home"] ]
                h1 [] [ ofString model.Location ]
            ]
        )
        , "TournamentsPage")
    
exportDefault TournamentsPage
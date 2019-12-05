#load "Shared.fsx"
#load "Helpers.fsx"

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Shared
open Helpers

//TODO sort by date, game, location, name
// TODO filter by past and upcoming
let TournamentsPage =
    FunctionComponent.Of (
        (fun () ->
            let model = useModel()
            let dispatch = useDispatch()
            
            let tournaments =
                model.Tournaments
                |> Array.toList
                |> List.map (fun (id, name, _, _, startTime, address, imageURL, _, _, _) ->
                        a [Href "#";
                           OnClick (fun ev -> ev.preventDefault(); Msg.Navigate (Route.Detail id) |> dispatch);
                           Key !!id] [
                            li [ClassName "tournaments"] [
                                img [ClassName "tournaments"; Src imageURL]
                                h3 [ClassName "tournaments"] [str name]
                                h3 [ClassName "tournaments"] [ str address ]
                                h3 [] [str (getDate(startTime))]
                                hr []
                            ]
                        ]
                )
            
            div [ClassName "tournaments"] [
                h1 [] [str "TournamentsPage"]
                ul [ClassName "tournaments"] [ ofList tournaments ]
                br []
                button [] [ A Route.Root [str "Return To Home"] ]
                br []
                br []
                br []
                br []
            ]
        )
        , "TournamentsPage")
    
exportDefault TournamentsPage
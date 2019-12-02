open System.Text

#load "../.paket/load/netstandard2.0/main.group.fsx"

open Fable.React
open Fable.React.Props
open Elmish

type Tournament = string

type Route =
    | Root
    | Detail of int

let toRouteUrl route =
    match route with
    | Route.Root -> "/"
    | Route.Detail id -> sprintf "/detail/%d" id

type Model =
    { CurrentRoute: Route option
      Tournaments: Map<int, Tournament> }

type Msg =
    | Navigate of Route
    | TournamentsLoaded of Map<int,Tournament>
 
module Routing =
    open Elmish.UrlParser
    let private route =
        oneOf
            [ map Route.Root (s "")
              map Route.Detail (s "detail" </> i32) ]
    
    // Take a window.location object and return an option of Route
    let parsePath location = UrlParser.parsePath route location
 
let urlUpdate (route: Route option) (model: Model) =
    { model with CurrentRoute = route }, Cmd.none

open Thoth.Json

let getTournaments (dispatch: Dispatch<Msg>) =
    let decoder =
        Decode.object (fun get ->
            get.Required.Field "id" Decode.int, get.Required.Field "name" Decode.string)
        |> Decode.array
    
    Fetch.fetch "/tournaments.json" []
    |> Promise.bind (fun response -> response.text())
    |> Promise.map (fun json ->
        let result = Decode.fromString decoder json
        match result with
        | Ok tournaments ->
            tournaments
            |> Map.ofArray
            |> Msg.TournamentsLoaded
            |> dispatch
        | Error err ->
            printfn "%s" err)
    |> Promise.catchEnd (fun ex ->
        printfn "%A" ex)

let init _ =
    let model = { Tournaments = Map.empty
                  CurrentRoute = None }
    let route = Routing.parsePath Browser.Dom.document.location
    let cmd = Cmd.ofSub getTournaments
    let model', cmd' = urlUpdate route model
    model', (Cmd.batch [cmd; cmd'])

let update msg model =
    match msg with
    | Navigate route ->
        model, Elmish.Navigation.Navigation.newUrl (toRouteUrl route)
    | TournamentsLoaded tournaments ->
        { model with Tournaments = tournaments }, Cmd.none
    
[<NoComparison>]
type AppContext =
    { Model: Model
      Dispatch: Dispatch<Msg> }
  
let defaultContextValue : AppContext = Fable.Core.JS.undefined

let appContext = ReactBindings.React.createContext(defaultContextValue)

let useModel() : Model =
    let ac = Hooks.useContext(appContext)
    ac.Model
    
let useDispatch() : Dispatch<Msg> =
    let ac = Hooks.useContext(appContext)
    ac.Dispatch
    
type AProps = { Children: ReactElement seq; Route: Route }
 
let A route children = // Wrapper for link elements
    FunctionComponent.Of (
        (fun (props: AProps) ->
            let dispatch = useDispatch()
            a [Href "#"; OnClick (fun ev -> ev.preventDefault(); Msg.Navigate props.Route |> dispatch )] props.Children
        )
        , "A"
        , equalsButFunctions) ({ Children = children; Route = route })

let DetailPage =
    FunctionComponent.Of (
        (fun () -> div [] [
                        h1 [] [str "DETAIL PAGE"]
                        (*button [OnClick (fun _ -> Msg.Navigate Route.Root |> useDispatch() )] [str "Go Back to Home"]*)
                        button [] [
                            A Route.Root [str "Go Back Home"]
                        ]
                   ])
        , "DetailPage"
        , equalsButFunctions)

let layout dispatch page =
    div [] [
        h1 [] [str "Smash Search"]
        h2 [] [
                str "Location"
                input [Type "text"; Id "locationInput"]
                button [] [str "Locate Me"]
        ]
        button [(*OnClick (fun _ ->  dispatch FindTournies)*)] [str "Search"]
        page
        footer [ClassName "footer"] [ str "CPS 452 (Fall 2019) Final Project by Tyler Berkshire" ]
    ]

let App =
    FunctionComponent.Of (
        (fun () ->
            let model = useModel()
            let dispatch = useDispatch()
            
            match model.CurrentRoute with
            | Some(Route.Root) -> h1 [] [str "HOME PAGE"]
            | Some(Route.Detail id) -> DetailPage()
            | None -> h1 [] [str "404 - Page not found"]
            |> layout dispatch
        )
        , "App"
        , equalsButFunctions)

let ElmishCapture =
    FunctionComponent.Of (
        (fun (props:AppContext) ->
            contextProvider appContext props [ App() ]
        )
        , "ElmishCapture"
        , equalsButFunctions)

let view model dispatch =
    ElmishCapture ({ Model = model; Dispatch = dispatch })

open Elmish.React
open Elmish.Navigation

Program.mkProgram init update view
|> Program.toNavigable Routing.parsePath urlUpdate
|> Program.withReactBatched "app" // view output gets mapped to "app" id in html
|> Program.run

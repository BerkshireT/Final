#load "Shared.fsx"

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Elmish
open Elmish.React
open Elmish.Navigation
open Thoth.Json
open Shared

module Routing =
    open Elmish.UrlParser
    let private route =
        oneOf
            [ map Route.Root (s "")
              map Route.Detail (s "detail" </> i32) ]
    
    // Take a window.location object and return an option of Route
    let parsePath location = UrlParser.parsePath route location

let toRouteUrl route =
    match route with
    | Route.Root -> "/"
    | Route.Detail id -> sprintf "/detail/%d" id
 
let urlUpdate (route: Route option) (model: Model) =
    { model with CurrentRoute = route }, Cmd.none

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
                  CurrentRoute = None
                  IsLoadingTournaments = false }
    let route = Routing.parsePath Browser.Dom.document.location
    let cmd = Cmd.ofSub getTournaments
    let model', cmd' = urlUpdate route model
    model', (Cmd.batch [cmd; cmd']) // Combine possible urlUpdate cmds with getTournaments

let update msg model =
    match msg with
    | Navigate route ->
        model, Elmish.Navigation.Navigation.newUrl (toRouteUrl route)
    | TournamentsLoaded tournaments ->
        { model with Tournaments = tournaments }, Cmd.none

let suspense fallback children =
    let props = createObj [ "fallback" ==> fallback ]
    ofImport "Suspense" "react" props children

let fallback =
    p [] [ i [ClassName "spin"; DangerouslySetInnerHTML { __html = "&orarr;" }] []
           str "loading your page..." ]

let layout page =
    div [] [
        h1 [] [str "Smash Search"]
        h2 [] [
                str "Location"
                input [Type "text"; Id "locationInput"]
                button [] [str "Locate Me"]
        ]
        button [(*OnClick (fun _ ->  dispatch FindTournies)*)] [str "Search"]
        suspense fallback [page]
        footer [ClassName "footer"] [ str "CPS 452 (Fall 2019) Final Project by Tyler Berkshire" ]
    ]

let DetailPage props : ReactElement =
    let detailPage = ReactBindings.React.``lazy`` (fun () -> importDynamic "./DetailPage.fsx")
    ReactBindings.React.createElement(detailPage, props, [])

let App =
    FunctionComponent.Of (fun () ->
        let model = useModel()
        
        if model.IsLoadingTournaments then
            fallback
        else
            match model.CurrentRoute with
            | Some(Route.Root) -> h1 [] [str "HOME PAGE"]
            | Some(Route.Detail _) -> DetailPage()
            | None -> h1 [] [str "404 - Page not found"]
        |> layout
    , "App")

let ElmishCapture =
    FunctionComponent.Of (
        fun (props:AppContext) ->
            contextProvider appContext props [ App() ]
        , "ElmishCapture", memoEqualsButFunctions)

let view model dispatch =
    ElmishCapture ({ Model = model; Dispatch = dispatch })

Program.mkProgram init update view
|> Program.toNavigable Routing.parsePath urlUpdate
|> Program.withReactBatched "app" // view output gets mapped to "app" id in html
|> Program.run

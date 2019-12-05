#load "Shared.fsx"
#load "Helpers.fsx"

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Elmish
open Elmish.React
open Elmish.Navigation
open Shared
open Helpers

module Routing =
    open Elmish.UrlParser
    let private route =
        oneOf
            [ map Route.Root (s "")
              map Route.Tournaments (s "tournaments")
              map Route.Detail (s "detail" </> i32) ]
    
    // Take a window.location object and return an option of Route
    let parsePath location = UrlParser.parsePath route location

let toRouteUrl route =
    match route with
    | Route.Root -> "/"
    | Route.Tournaments -> "/tournaments"
    | Route.Detail id -> sprintf "/detail/%d" id
 
let urlUpdate (route: Route option) (model: Model) =
    match route with
    | Some(Route.Detail id) ->
        { model with SelectedTournament = id
                     CurrentRoute = route }, Cmd.none
    | _ ->
        { model with CurrentRoute = route }, Cmd.none

let init _ =
    let model = { Tournaments = Array.empty
                  CurrentRoute = None
                  IsLoadingTournaments = false
                  SelectedTournament = 0 }
    let route = Routing.parsePath Browser.Dom.document.location
    let cmd = Cmd.ofSub getTournaments
    let model', cmd' = urlUpdate route model
    model', Cmd.batch [cmd;cmd']

let update msg model =
    match msg with
    | Navigate route ->
        model, Elmish.Navigation.Navigation.newUrl (toRouteUrl route)
    | GetTournaments ->
        model,  Cmd.ofSub getTournaments
    | TournamentsLoaded tournaments ->
        { model with Tournaments = tournaments
                     IsLoadingTournaments = false
        }, Cmd.none
    | FailedToLoad err ->
        printfn "Error loading tournaments: %s" err
        // TODO Update to add something to the model for the user to see something went wrong
        model, Cmd.none

let suspense fallback children =
    let props = createObj [ "fallback" ==> fallback ]
    ofImport "Suspense" "react" props children

let fallback =
    p [] [ i [ClassName "spin"; DangerouslySetInnerHTML { __html = "&orarr;" }] []
           str "Loading..." ]

let layout page =
    let model = useModel()
    let dispatch = useDispatch()
    div [] [
        br []
        br []
        br []
        br []
        (* h4 [] [
                str "Enter Your Location or Click \"Locate Me\""
                br []
                br []
                input [Type "text"
                       Value model.Location
                       Placeholder "Latitude,Longitude"
                       OnChange (fun ev -> Msg.ChangeLocation ev.Value |> dispatch)]
                button [OnClick (fun _ ->  Msg.ChangeLocation "" |> dispatch)] [str "Locate Me"]
        ] *)
        suspense fallback [page]
    ]
 
let HomePage props : ReactElement =
    let homePage = ReactBindings.React.``lazy`` (fun () -> importDynamic "./HomePage.fsx")
    ReactBindings.React.createElement(homePage, props, [])
    
let TournamentsPage props : ReactElement =
    let tournamentsPage = ReactBindings.React.``lazy`` (fun () -> importDynamic "./TournamentsPage.fsx")
    ReactBindings.React.createElement(tournamentsPage, props, [])

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
            | Some(Route.Root) -> HomePage()
            | Some(Route.Tournaments _) -> TournamentsPage()
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

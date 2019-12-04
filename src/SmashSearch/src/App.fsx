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
              map Route.Tournaments (s "tournaments" </> str)
              map Route.Detail (s "detail" </> i32) ]
    
    // Take a window.location object and return an option of Route
    let parsePath location = UrlParser.parsePath route location

let toRouteUrl route =
    match route with
    | Route.Root -> "/"
    | Route.Tournaments location -> sprintf "/tournaments/%s" location
    | Route.Detail id -> sprintf "/detail/%d" id
 
let urlUpdate (route: Route option) (model: Model) =
    { model with CurrentRoute = route }, Cmd.none

let init _ =
    let model = { Tournaments = Array.empty
                  CurrentRoute = None
                  IsLoadingTournaments = false
                  Location = "" }
    let route = Routing.parsePath Browser.Dom.document.location
    urlUpdate route model

let update msg model =
    match msg with
    | Navigate route ->
        model, Elmish.Navigation.Navigation.newUrl (toRouteUrl route)
    | ChangeLocation location ->
        { model with Location = location }, Cmd.none
    | GetTournaments ->
        model,  Cmd.ofSub getTournaments
    | TournamentsLoaded tournaments ->
        { model with Tournaments = tournaments
                     IsLoadingTournaments = false
        }, Cmd.ofMsg (Navigate (Route.Tournaments model.Location))
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
        h1 [] [str "Smash Search"]
        br []
        h4 [] [
                str "Enter Your Location or Click \"Locate Me\""
                br []
                br []
                input [Type "text"
                       //DefaultValue model.Location
                       Value model.Location
                       Placeholder "Latitude,Longitude"
                       OnChange (fun ev -> Msg.ChangeLocation ev.Value |> dispatch)]
                button [OnClick (fun _ ->  Msg.ChangeLocation ev.Value |> dispatch)] [str "Locate Me"]
                // Add slider for radius
        ]
        button [ClassName "SearchButton"; OnClick (fun _ ->  Msg.GetTournaments |> dispatch)] [str "Search"]
        suspense fallback [page]
        footer [ClassName "footer"] [ str "CPS 452 (Fall 2019) Final Project by Tyler Berkshire" ]
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

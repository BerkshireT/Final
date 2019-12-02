#load "../.paket/load/main.group.fsx"

open Fable.React
open Fable.React.Props
open Elmish
open Browser.Types

type Tournament = string

type Location = string

type Route =
    | Root
    | Tournaments of Location
    | Detail of int

type Msg =
    | Navigate of Route
    | TournamentsLoaded of Map<int,Tournament>

type Model =
    { CurrentRoute: Route option
      Tournaments: Map<int, Tournament>
      IsLoadingTournaments: bool }

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
        fun (props: AProps) ->
            let dispatch = useDispatch()
            let onClick (ev:Event) =
                ev.preventDefault()
                Msg.Navigate props.Route
                |> dispatch
            a [Href "#"; OnClick onClick] children
        , "A" , equalsButFunctions) ({ Children = children; Route = route })
#load "../.paket/load/main.group.fsx"

open Fable.React
open Fable.React.Props
open Elmish
open Browser.Types

type Tournament = {
    id: int;
    name: string;
    city: string;
    state: string;
    startTime: int64
    address: string;
    imageURL: string;
    slug: string;
    primaryContact: string;
    events: array<(string * int)>
}

type Location = string

type Route =
    | Root
    | Tournaments
    | Detail of int

type Msg =
    | Navigate of Route
    | GetTournaments
    | TournamentsLoaded of array<(int * string * string * string * int64 * string * string * string * string * array<(string * int)>)>
    | FailedToLoad of string

type Model =
    { CurrentRoute: Route option
      Tournaments: array<(int * string * string * string * int64 * string * string * string * string * array<(string * int)>)>
      IsLoadingTournaments: bool
      SelectedTournament: int }

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
    
type AProps =
    { Children: ReactElement seq;
      Route: Route }
 
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

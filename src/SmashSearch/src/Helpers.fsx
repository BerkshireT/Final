#load "Shared.fsx"

open FSharp.Data.GraphQL.Client
open Thoth.Json
open Elmish
open Fable.Core
open Shared

type IGeolocate =
    abstract getGeolocation : unit -> string

[<Import("*",from="./Geolocation.js")>]
let geoLib: IGeolocate = jsNative

let getGeolocation (dispatch: Dispatch<Msg>) =
    Msg.ChangeLocation (geoLib.getGeolocation()) |> dispatch

(*
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
*)
#load "Shared.fsx"

open FSharp.Data
open Thoth.Json
open System
open System.Diagnostics
open Elmish
open Fable.Core
open Shared

//let dt =  DateTimeOffset.FromUnixTimeSeconds(unix)  //val dt : DateTimeOffset = 2017/02/28 4:01:31 +00:00

let getTournaments (dispatch: Dispatch<Msg>) =
    // TODO: Regex to determine which API call to use
    
    let decoder =
        Decode.object (fun get ->
            get.Required.Field "id" Decode.int,
            get.Required.Field "name" Decode.string,
            get.Required.Field "city" Decode.string,
            get.Required.Field "addrState" Decode.string,
            get.Required.Field "startAt" Decode.int,
            get.Required.Field "venueAddress" Decode.string,
            get.Required.Field "imageURL" Decode.string)
        |> Decode.array
    
    Fetch.fetch "/tournaments.json" []
    |> Promise.bind (fun response -> response.text())
    |> Promise.map (fun json ->
        let result = Decode.fromString decoder json
        match result with
        | Ok tournaments ->
            Msg.TournamentsLoaded tournaments
            |> dispatch
        | Error err ->
            Msg.FailedToLoad err
            |> dispatch
    )
    |> Promise.catchEnd (fun ex ->
        Msg.FailedToLoad (ex.ToString())
       |> dispatch
    )
(* Fable compiler does not implement System.Diagnostics fully
    let proc = ProcessStartInfo(FileName = "Requests.py")
    proc.Arguments <- "4 39.7420426,-50.1845668 50mi"
    proc.RedirectStandardOutput <- true
    proc.RedirectStandardError <- true
    proc.UseShellExecute <- false
    proc.WorkingDirectory <- Environment.CurrentDirectory
    
    let outputs = System.Collections.Generic.List<string>()
    let errors = System.Collections.Generic.List<string>()
    let outputHandler f (_sender:obj) (args:DataReceivedEventArgs) = f args.Data
    let p = new Process(StartInfo = proc)
    p.OutputDataReceived.AddHandler(DataReceivedEventHandler (outputHandler outputs.Add))
    p.ErrorDataReceived.AddHandler(DataReceivedEventHandler (outputHandler errors.Add))
    let started =
      try
            p.Start()
      with | ex ->
            ex.Data.Add("filename", "Requests.py")
            reraise()
    if not started then
        failwithf "Failed to start process %s" "Requests.py"
    p.BeginOutputReadLine()
    p.WaitForExit()
    p.Close()
*)


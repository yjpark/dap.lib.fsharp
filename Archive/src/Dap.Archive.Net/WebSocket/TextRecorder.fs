[<RequireQualifiedAccess>]
module Dap.Archive.WebSocket.TextRecorder

open FSharp.Control.Tasks
open Dap.Prelude
open Dap.Platform
open Dap.Remote
open Dap.Archive

module RecorderTypes = Dap.Archive.Recorder.Types
module EventRecorder = Dap.Archive.Recorder.EventRecorder

module ClientTypes = Dap.WebSocket.Client.Types
module TextClient = Dap.WebSocket.Client.TextClient

module ConnTypes = Dap.WebSocket.Conn.Types
module TextConn = Dap.WebSocket.Conn.TextConn

let watchClient (agent : EventRecorder.Agent) (onEvent : IEvent<ClientTypes.Evt<string>>) =
    onEvent.Add (fun evt ->
        match evt with
        | ClientTypes.OnSent (_stat, pkt) ->
            EventRecorder.appendEvent' agent "OnSent" pkt
        | ClientTypes.OnReceived (_stat, pkt) ->
            EventRecorder.appendEvent' agent "OnReceived" pkt
        | _ -> ()
    )

let watchConn (agent : EventRecorder.Agent) (onEvent : IEvent<ConnTypes.Evt<string>>) =
    onEvent.Add (fun evt ->
        match evt with
        | ConnTypes.OnSent (_stat, pkt) ->
            EventRecorder.appendEvent' agent "OnSent" pkt
        | ConnTypes.OnReceived (_stat, pkt) ->
            EventRecorder.appendEvent' agent "OnReceived" pkt
        | _ -> ()
    )

let createForClientAsync (agent : IAgent) (profile : Profile) (param : EventRecorder.BundleParam') (client : TextClient.Agent) = task {
    let! (recorder, _) = agent.Env.HandleAsync <| DoGetAgent' EventRecorder.Kind client.Ident.Key
    let recorder = recorder :?> EventRecorder.Agent
    let! meta = recorder.PostAsync <| RecorderTypes.DoBeginRecording' (EventRecorder.createBundle' profile param)
    logInfo agent "Recorder" "Start_Recording" (recorder.Ident, meta)
    watchClient recorder client.Actor.OnEvent
    return recorder
}
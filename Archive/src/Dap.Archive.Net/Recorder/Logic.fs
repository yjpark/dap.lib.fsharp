[<RequireQualifiedAccess>]
module Dap.Archive.Recorder.Logic

open System.IO
open Elmish
open Dap.Prelude
open Dap.Platform

open Dap.Archive

let private doBeginRecording msg ((bundle, callback) : Bundle'<'extra, 'frame> * Callback<Meta<'extra>>)
                            : Operate<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner (model, cmd) ->
        match model.Bundle with
        | None -> ()
        | Some bundle ->
            bundle.Close runner
            bundle.Volume |> Option.iter (fun v -> model.Args.FireEvent' <| OnFinishRecording v.Meta)
        bundle.Open runner runner.Clock.Now
        match bundle.Volume with
        | Some volume ->
            reply runner callback <| ack msg volume.Meta
            model.Args.FireEvent' <| OnBeginRecording volume.Meta
            ({model with Bundle = Some bundle}, cmd)
        | None ->
            reply runner callback <| nak msg "Bundle_Open_Failed" bundle
            ({model with Bundle = None}, cmd)

let private doFinishRecording msg (callback : Callback<Meta<'extra>>)
                            : Operate<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner (model, cmd) ->
        match model.Bundle with
        | Some bundle ->
            bundle.Close runner
            match bundle.Volume with
            | Some volume ->
                reply runner callback <| ack msg volume.Meta
                model.Args.FireEvent' <| OnFinishRecording volume.Meta
            | None ->
                reply runner callback <| nak msg "Bundle_Has_No_Volume" bundle
            ({model with Bundle = None}, cmd)
        | None ->
            reply runner callback <| nak msg "Not_Recording" None
            (model, cmd)

let private doAppendFrame' (args : Args<'extra, 'frame>) msg ((frame, callback) : 'frame * Callback<Meta<'extra> * 'frame>)
                            : StateAction<IRunner, Bundle'<'extra, 'frame>> =
    fun runner bundle ->
        try
            bundle.WriteFrame runner frame
            match bundle.Volume with
            | Some volume ->
                reply runner callback <| ack msg (volume.Meta, frame)
                args.FireEvent' <| OnAppendFrame (volume.Meta, frame)
            | None ->
                reply runner callback <| nak msg "Bundle_Has_No_Volume" (frame, bundle)
                failwith "Bundle_Has_No_Volume"
        with e ->
            reply runner callback <| nak msg "AppendFrame_Failed" (frame, bundle, e)
            args.FireEvent' <| OnAppendFrameFailed (frame, e)

let private doAppendFrame msg ((frame, callback) : 'frame * Callback<Meta<'extra> * 'frame>)
                            : Operate<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner (model, cmd) ->
        match model.Bundle with
        | Some bundle ->
            doAppendFrame' model.Args msg (frame, callback) runner bundle
        | None ->
            reply runner callback <| nak msg "Not_Recording" None
        (model, cmd)

let private handleReq msg req : Operate<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner (model, cmd) ->
        match req with
        | DoBeginRecording (a, b) -> doBeginRecording msg (a, b)
        | DoFinishRecording a -> doFinishRecording msg a
        | DoAppendFrame (a, b) -> doAppendFrame msg (a, b)
        <| runner <| (model, cmd)

let private handleEvt _msg evt : Operate<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner (model, cmd) ->
        match evt with
        | OnAppendFrameFailed (_frame, _e) -> 
            model.Bundle |> Option.iter (fun bundle -> bundle.Close runner)
            setModel {model with Bundle = None}
        | _ -> noOperation
        <| runner <| (model, cmd)

let private update : Update<IRunner, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun runner model msg -> 
        match msg with
        | RecorderReq req -> handleReq msg req
        | RecorderEvt evt -> handleEvt msg evt
        <| runner <| (model, [])

let private init : Init<IAgent, Args<'extra, 'frame>, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    fun _runner args ->
        ({
            Args = args
            Bundle = None
        }, noCmd)

let private subscribe (runner : IAgent) (model : Model<'extra, 'frame>) : Cmd<Msg<'extra, 'frame>> =
    subscribeEvent runner model RecorderEvt model.Args.OnEvent

let logic : Logic<IAgent, Args<'extra, 'frame>, Model<'extra, 'frame>, Msg<'extra, 'frame>> =
    {
        Init = init
        Update = update
        Subscribe = subscribe
    }

let getSpec (newArgs : unit -> Args<'extra, 'frame>) : AgentSpec<Args<'extra, 'frame>, Model<'extra, 'frame>, Msg<'extra, 'frame>, Req<'extra, 'frame>, Evt<'extra, 'frame>> =
    {
        Actor =
            {
                NewArgs = newArgs
                Logic = logic
                WrapReq = RecorderReq
                GetOnEvent = fun model -> model.Args.OnEvent
            }
        OnAgentEvent = None
        GetSlowCap = None
    }
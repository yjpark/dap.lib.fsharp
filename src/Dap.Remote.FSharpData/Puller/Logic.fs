[<RequireQualifiedAccess>]
module Dap.Remote.FSharpData.Puller.Logic

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote

open Dap.Remote.FSharpData.Puller.Types
module TickerTypes = Dap.Platform.Ticker.Types
module Http = Dap.Remote.FSharpData.Http

let private doPause (req : IReq) (callback : Callback<unit>) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        replyAfter runner callback <| ack req ()
        ({model with Paused = true}, cmd)

let private doResume (req : IReq) (callback : Callback<unit>) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        replyAfter runner callback <| ack req ()
        ({model with Paused = false}, cmd)

let private doPull<'actorMsg, 'res when 'actorMsg :> IMsg> (req : IReq) (callback : Callback<unit>) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        runner.Part.Args.NewRequest ()
        |> Option.map (fun request ->
            let index = model.NextIndex
            let callback = fun (res : Http.Response<'res>) ->
                runner.Deliver <| InternalEvt ^<| OnHttpRes (index, res)
            Http.handle runner request callback
            updateModel (fun m -> {m with Waiting = Some index})
        )|> Option.defaultValue noOperation
        <| runner <| (model, cmd)

let private handleReq<'actorMsg, 'res when 'actorMsg :> IMsg> (req : Req) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        match req with
        | DoPause a -> doPause req a
        | DoResume a -> doResume req a
        | DoPull a -> doPull<'actorMsg, 'res> req a
        <| runner <| (model, cmd)

let inline addDoPullCmd runner (model, cmd) =
    (runner, model, cmd)
    |=|> addSubCmd PullerReq ^<| DoPull None

let private doInit : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        let ident = sprintf "Puller:%s" <| newGuid ()
        runner.Part.Args.Ticker.Actor2.OnEvent.AddWatcher model.WatcherOwner ident (fun evt ->
            match evt with
            | TickerTypes.OnTick (a, b) ->
                runner.Deliver <| InternalEvt ^<| OnTick (a, b)
            | _ -> ()
        )
        match model.Paused with
        | true -> noOperation
        | false -> addDoPullCmd
        <| runner <| (model, [])

let private onHttpRes ((index, res) : int * Http.Response<'res>) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        let waiting =
            model.Waiting
            |> Option.bind (fun waiting ->
                if waiting <= index then
                    None
                else
                    Some waiting
            )
        let pull : Pull<'res> = {
            Index = index
            Base = res
        }
        let history =
            pull :: model.History
            |> List.truncate runner.Part.Args.HistorySize
        (runner, model, cmd)
        |-|> updateModel (fun m -> {m with Waiting = waiting ; History = history})
        |=|> addSubCmd PullerEvt ^<| OnPull pull

let private onTick ((time, delta) : Instant * Duration) : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        match model.Paused with
        | true ->
            (model, cmd)
        | false ->
            model.Latest
            |> Option.map (fun res ->
                let interval =
                    match runner.Part.Args.Mode with
                    | FromLastReq -> time - res.ReqTime
                    | FromLastRes -> time - res.ResTime
                if interval < runner.Part.Args.Interval then
                    noOperation
                else
                    addDoPullCmd
            )|> Option.defaultValue addDoPullCmd
            <| runner <| (model, cmd)

let private handleInternalEvt evt : PartOperate<'actorMsg, 'res> =
    fun runner (model, cmd) ->
        match evt with
        | OnTick (a, b) -> onTick (a, b)
        | OnHttpRes (a, b) -> onHttpRes (a, b)
        | DoInit -> doInit
        <| runner <| (model, [])

let private update : Update<Part<'actorMsg, 'res>, Model<'res>, Msg<'res>> =
    fun runner msg model ->
        match msg with
        | InternalEvt evt -> handleInternalEvt evt
        | PullerReq req -> handleReq req
        | PullerEvt _evt -> noOperation
        <| runner <| (model, [])

let private init : ActorInit<Args<'res>, Model<'res>, Msg<'res>> =
    fun runner args ->
        (runner, {
            Paused = not args.AutoStart
            Waiting = None
            History = []
            WatcherOwner = IOwner.Create (sprintf "%s:Puller" <| runner.Ident.ToLuid ())
        }, [])
        |=|> addSubCmd InternalEvt DoInit

let spec<'actorMsg, 'res when 'actorMsg :> IMsg> (args : Args<'res>) =
    new ActorSpec<Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>>
        (Part<'actorMsg, 'res>.Spawn, args, PullerReq, castEvt<'res>, init, update)

let create<'actorRunner, 'actorModel, 'actorMsg, 'res
            when 'actorRunner :> IAgent<'actorMsg> and 'actorMsg :> IMsg>
        (args : Args<'res>) partMsg wrapMsg agent =
    let spec = spec<'actorMsg, 'res> args
    agent |> Part.create<'actorRunner, 'actorModel, 'actorMsg, Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>> spec partMsg wrapMsg

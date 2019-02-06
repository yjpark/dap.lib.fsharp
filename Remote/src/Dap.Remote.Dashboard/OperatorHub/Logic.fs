[<RequireQualifiedAccess>]
module Dap.Remote.Dashboard.OperatorHub.Logic

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote

open Dap.Remote.Dashboard
open Dap.Remote.Dashboard.OperatorHub.Types

type ActorOperate = Operate<Agent, Model, Msg>

let private onStatusChanged (status : LinkStatus) : ActorOperate =
    fun runner (model, cmd) ->
        match status with
        | LinkStatus.Closed ->
            ({model with Auth = false}, cmd)
        | _ ->
            (model, cmd)

let private handleInternalEvt (evt : InternalEvt) : ActorOperate =
    match evt with
    | OnStatusChanged a -> onStatusChanged a

let private checkToken (runner : Agent) (token : string) =
    runner.Actor.Args.Token = token

let private getInfo (runner : Agent) =
    toJson JsonNil

let private handleReq (req : Req) : ActorOperate =
    fun runner (model, cmd) ->
        match req with
        | Req.DoAuth (r, callback) ->
            match checkToken runner r.Value with
            | true ->
                reply runner callback <| ack req ^<| Ok ^<| Operator.AuthRes.Create (getInfo runner)
                ({model with Auth = true}, cmd)
            | false ->
                reply runner callback <| ack req ^<| Error Operator.AuthErr.InvalidToken
                ({model with Auth = false}, cmd)
        | Req.DoInspect (r, callback) ->
            match model.Auth with
            | true ->
                let snapshot = runner.Env.TakeSnapshot ()
                reply runner callback <| ack req ^<| Ok snapshot
                let history =
                    snapshot :: model.History
                    |> List.truncate runner.Actor.Args.HistorySize
                ({model with Latest = snapshot ; History = history}, cmd)
            | false ->
                reply runner callback <| ack req ^<| Error Operator.InspectErr.PermissionDenied
                (model, cmd)

let private update : Update<Agent, Model, Msg> =
    fun runner msg model ->
        match msg with
        | HubReq req ->
            handleReq req
        | HubEvt _evt ->
            noOperation
        | InternalEvt evt ->
            handleInternalEvt evt
        <| runner <| (model, [])

let private init : ActorInit<Args, Model, Msg> =
    fun runner args ->
        ({
            Auth = false
            Latest = runner.Env.TakeSnapshot ()
            History = []
        }, noCmd)

let spec pack (args : Args) =
    new ActorSpec<Agent, Args, Model, Msg, Req, Evt>
        (Agent.Spawn pack, args, HubReq, castEvt, init, update)

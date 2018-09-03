module Dap.Remote.FSharpData.Puller.Types

open Dap.Prelude
open Dap.Platform
open Dap.Remote

module Http = Dap.Remote.FSharpData.Http

type Ticker = Dap.Platform.Ticker.Service.Service

type IntervalMode =
    | FromLastReq
    | FromLastRes

type Args<'res> = {
    Ticker : Ticker
    Mode : IntervalMode
    Interval : Duration
    AutoStart : bool
    NewRequest : unit -> Http.Request<'res> option
    HistorySize : int
}

and Pull<'res> = {
    Index : int
    Base : Http.Response<'res>
} with
    member this.ReqTime = this.Base.ReqTime
    member this.Request = this.Base.Request
    member this.ResTime = this.Base.ResTime
    member this.ResBody = this.Base.ResBody
    member this.Result = this.Base.Result

and Model<'res> = {
    Paused : bool
    Waiting : int option
    History : Pull<'res> list
    WatcherOwner : IOwner
} with
    member this.Latest =
        this.History |> List.tryHead
    member this.NextIndex =
        match this.Waiting with
        | Some index ->
            index + 1
        | None ->
            this.History
            |> List.tryHead
            |> function
                | None -> 1
                | Some res -> res.Index + 1

and Req =
    | DoPause of Callback<unit>
    | DoResume of Callback<unit>
    | DoPull of Callback<unit>
with interface IReq

and Evt<'res> =
    | OnPull of Pull<'res>
with interface IEvt

and InternalEvt<'res> =
    | DoInit
    | OnTick of Instant * Duration
    | OnHttpRes of int * Http.Response<'res>

and Msg<'res> =
    | InternalEvt of InternalEvt<'res>
    | PullerReq of Req
    | PullerEvt of Evt<'res>
with interface IMsg

let castEvt<'res> : CastEvt<Msg<'res>, Evt<'res>> =
    function
    | PullerEvt evt -> Some evt
    | _ -> None

type Part<'actorMsg, 'res when 'actorMsg :> IMsg> (param) =
    inherit BasePart<'actorMsg, Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>> (param)
    override this.Runner = this
    static member Spawn (param) = new Part<'actorMsg, 'res> (param)
    member this.ResumeOrPull callback =
        if this.Part.State.Paused then
            this.Post <| DoResume callback
        else
            this.Post <| DoPull callback

type PartOperate<'actorMsg, 'res when 'actorMsg :> IMsg> =
    ActorOperate<Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>>
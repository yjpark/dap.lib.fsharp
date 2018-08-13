module Dap.Remote.FSharpData.Puller.Types

open Dap.Prelude
open Dap.Platform
open Dap.Remote

module Http = Dap.Remote.FSharpData.Http

type Ticker = Dap.Platform.Ticker.Types.Ticker

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

and PullReq<'res> = {
    Index : int
    Time : Instant
    Request : Http.Request<'res>
}

and Pull<'res> = {
    Index : int
    ReqTime : Instant
    Request : Http.Request<'res>
    ResTime : Instant
    Response : Http.Response<'res>
}

and Model<'res> = {
    Paused : bool
    Waiting : PullReq<'res> option
    History : Pull<'res> list
} with
    member this.Latest =
        this.History |> List.tryHead
    member this.NextIndex =
        match this.Waiting with
        | Some req ->
            req.Index + 1
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
    | OnHttpRes of PullReq<'res> * Http.Response<'res>

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

type PartOperate<'actorMsg, 'res when 'actorMsg :> IMsg> =
    ActorOperate<Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>>
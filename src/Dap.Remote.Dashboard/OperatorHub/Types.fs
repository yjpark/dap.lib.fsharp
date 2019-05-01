[<AutoOpen>]
module Dap.Remote.Dashboard.OperatorHub.Types

open Dap.Prelude
open Dap.Platform
open Dap.Remote

open Dap.Remote.Dashboard

[<Literal>]
let Kind = "OperatorHub"

[<Literal>]
let GatewayKind = "OperatorHubGateway"

type Req = Operator.ServerReq
type Evt = Operator.Evt

and Args = OperatorArgs

and Model = {
    Auth : bool
    Latest : EnvSnapshot
    History : EnvSnapshot list
}

and InternalEvt =
    | OnStatusChanged of LinkStatus

and Msg =
    | HubReq of Req
    | HubEvt of Evt
    | InternalEvt of InternalEvt
with interface IMsg

let castEvt : CastEvt<Msg, Evt> =
    function
    | HubEvt evt -> Some evt
    | _ -> None

type Agent (pack, param) =
    inherit PackAgent<ITickingPack, Agent, Args, Model, Msg, Req, Evt> (pack, param)
    override this.Runner = this
    static member Spawn k m = new Agent (k, m)

let setGateway (gateway : IGateway) : Func<Agent, unit> =
    fun runner ->
        gateway.OnStatus.AddWatcher runner "OnStatus" (runner.Deliver << InternalEvt << OnStatusChanged)

let HubSpec =
    Hub.getHubSpec<Agent, Req, Evt> Kind Operator.ServerReq.HubSpec setGateway



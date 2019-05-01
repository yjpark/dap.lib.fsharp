module Dap.Remote.Dashboard.Meta

open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.WebSocket.Meta
open Dap.Remote.Meta
open Dap.Remote.Meta.Net

[<Literal>]
let OperatorHubKind = "OperatorHub"

[<Literal>]
let OperatorHubGatewayKind = "OperatorHubGateway"

type M with
    static member operatorHub (?args : ArgsMeta, ?kind : Kind) =
        let args = defaultArg args <| JsonArgs "OperatorHubTypes.Args"
        let kind = defaultArg kind OperatorHubKind
        let alias = "OperatorHubTypes", "Dap.Remote.Dashboard.OperatorHub.Types"
        let type' = "OperatorHubTypes.Agent"
        let spec = "Dap.Remote.Dashboard.OperatorHub.Logic.spec"
        M.agent (args, type', spec, kind = kind, aliases = [alias])
    static member operatorHubGateway (?kind : Kind, ?logTraffic : bool) =
        M.gateway (
            aliases = [("OperatorHubTypes", "Dap.Remote.Dashboard.OperatorHub.Types")],
            reqEvt = "OperatorHubTypes.Req, OperatorHubTypes.Evt",
            hubSpec = "OperatorHubTypes.HubSpec",
            ?logTraffic = logTraffic,
            kind = defaultArg kind OperatorHubGatewayKind
        )





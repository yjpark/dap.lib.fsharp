[<AutoOpen>]
module Dap.Remote.Dashboard.Pack

open System.Threading
open System.Threading.Tasks
open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform

module PacketConn = Dap.Remote.WebSocketGateway.PacketConn
module OperatorHubTypes = Dap.Remote.Dashboard.OperatorHub.Types
module Gateway = Dap.Remote.WebSocketGateway.Gateway

(*
 * Generated: <Pack>
 *)
type IDashboardPackArgs =
    inherit ITickingPackArgs
    abstract PacketConn : PacketConn.Args with get
    abstract OperatorHub : OperatorHubTypes.Args with get
    abstract OperatorHubGateway : Gateway.Args<OperatorHubTypes.Req, OperatorHubTypes.Evt> with get
    abstract AsTickingPackArgs : ITickingPackArgs with get

type IDashboardPack =
    inherit IPack
    inherit ITickingPack
    abstract Args : IDashboardPackArgs with get
    abstract GetPacketConnAsync : Key -> Task<PacketConn.Agent * bool>
    abstract GetOperatorHubAsync : Key -> Task<OperatorHubTypes.Agent * bool>
    abstract GetOperatorHubGatewayAsync : Key -> Task<Gateway.Gateway * bool>
    abstract AsTickingPack : ITickingPack with get
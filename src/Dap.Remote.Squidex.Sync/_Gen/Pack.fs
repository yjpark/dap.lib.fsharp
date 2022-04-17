[<AutoOpen>]
module Dap.Remote.Squidex.Sync.Pack

open System.Threading
open System.Threading.Tasks
open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform
open Dap.Remote.Squidex

module Context = Dap.Platform.Context

(*
 * Generated: <Pack>
 *)
type ISyncPackArgs =
    inherit ITickingPackArgs
    abstract Sync : Context.Args<ISyncConfig> with get
    abstract AsTickingPackArgs : ITickingPackArgs with get

type ISyncPack =
    inherit IPack
    inherit ITickingPack
    abstract Args : ISyncPackArgs with get
    abstract Sync : Context.Agent<ISyncConfig> with get
    abstract AsTickingPack : ITickingPack with get
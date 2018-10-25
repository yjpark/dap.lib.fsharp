[<AutoOpen>]
module Dap.Local.IAppPack

open Dap.Prelude
open Dap.Context
open Dap.Platform
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Dap.Context.Builder

module Context = Dap.Platform.Context

(*
 * Generated: <Pack>
 *)
type IAppPackArgs =
    abstract Preferences : Context.Args<IPreferences> with get
    abstract SecureStorage : Context.Args<ISecureStorage> with get

type IAppPack =
    inherit IPack
    abstract Args : IAppPackArgs with get
    abstract Preferences : Context.Agent<IPreferences> with get
    abstract SecureStorage : Context.Agent<ISecureStorage> with get
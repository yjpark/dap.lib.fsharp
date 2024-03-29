[<AutoOpen>]
module Dap.Local.Farango.Packs

open System.Threading
open System.Threading.Tasks
open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform

module FarangoDb = Dap.Local.Farango.Db

type Db = FarangoDb.Model

(*
 * Generated: <Pack>
 *)
type IDbPackArgs =
    abstract FarangoDb : FarangoDb.Args with get

type IDbPack =
    inherit IPack
    abstract Args : IDbPackArgs with get
    abstract FarangoDb : FarangoDb.Agent with get
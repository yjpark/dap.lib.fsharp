[<AutoOpen>]
module Dap.Local.Farango.Packs

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform

module FarangoDb = Dap.Local.Farango.Db

type Db = FarangoDb.Model

type IDbPackArgs =
    abstract FarangoDb : FarangoDb.Args with get

type IDbPack =
    inherit ILogger
    abstract Env : IEnv with get
    abstract Args : IDbPackArgs with get
    abstract FarangoDb : FarangoDb.Agent with get
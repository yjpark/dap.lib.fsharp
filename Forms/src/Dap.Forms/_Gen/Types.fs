[<AutoOpen>]
module Dap.Forms.Types

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform
open Dap.Local

type IFormsPackArgs =
    inherit ILocalPackArgs
    abstract Temp : FileSystemArgs with get

type IFormsPack =
    inherit ILocalPack
    abstract Args : IFormsPackArgs with get
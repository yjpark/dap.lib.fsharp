[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Feature.Environment

open System.IO
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

let mutable private instance : IEnvironment option = None

let private getInstance () =
    if instance.IsNone then
        instance <- Some <| Feature.create<IEnvironment> (getLogging ())
    instance.Value

[<AbstractClass>]
type Context<'context when 'context :> IEnvironment> (logging : ILogging) =
    inherit BaseEnvironment<'context> (logging)
    do (
        if instance.IsNone then
            instance <- Some base.AsEnvironment
    )

type Context (logging : ILogging) =
    inherit Context<Context> (logging)
    override this.Self = this
    override __.Spawn l = new Context (l)
    interface IFallback

type IEnvironment with
    static member Instance = getInstance ()
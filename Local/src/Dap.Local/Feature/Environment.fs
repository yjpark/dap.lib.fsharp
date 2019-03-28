[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Feature.Environment

open System.IO
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

[<Literal>]
let EnvironmentKind = "Environment"

let mutable private instance : IEnvironment option = None

let private getInstance () =
    if instance.IsNone then
        instance <- Some <| Feature.create<IEnvironment> (getLogging ())
    instance.Value

[<AbstractClass>]
type Context<'context when 'context :> IEnvironment> (logging : ILogging) as self =
    inherit CustomContext<'context, ContextSpec<EnvironmentProps>, EnvironmentProps> (logging, new ContextSpec<EnvironmentProps>(EnvironmentKind, EnvironmentProps.Create))
    let inspect = base.Handlers.Add<unit, Json> (E.unit, D.unit, E.json, D.json, "inspect")
    let preferences : IPreferences = Feature.create<IPreferences> (logging)
    let secureStorage : ISecureStorage = Feature.create<ISecureStorage> (logging)
    do (
        if instance.IsNone then
            instance <- Some (self :> IEnvironment)
    )
    member this.EnvironmentProps : EnvironmentProps = this.Properties
    member __.Inspect : IHandler<unit, Json> = inspect
    interface IEnvironment with
        member this.EnvironmentProps = this.Properties
        member __.Inspect = inspect
        member __.Preferences = preferences
        member __.SecureStorage = secureStorage
    member this.AsEnvironment = this :> IEnvironment

type Context (logging : ILogging) =
    inherit Context<Context> (logging)
    override this.Self = this
    override __.Spawn l = new Context (l)
    interface IFallback

type IEnvironment with
    static member Instance = getInstance ()
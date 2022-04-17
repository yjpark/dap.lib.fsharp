[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Feature.Environment

open System.IO

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Platform.Cli
open Dap.Local

[<Literal>]
let EnvironmentKind = "Environment"

let mutable private instance : IEnvironment option = None

let internal getInstance () =
    if instance.IsNone then
        instance <- Some <| Feature.create<IEnvironment> (getLogging ())
    instance.Value

let private loadVersion (logging : ILogging) : Version =
    let versions = CliHook.createAll<IVersion> logging
    if versions.Length > 1 then
        failWith "Got_Multiple_Versions" (versions.Length, versions)
    elif versions.Length = 1 then
        (List.head versions) .ToVersion ()
    else
        Version.Create (
            major = 0,
            minor = 1,
            patch = 0,
            commit = "master",
            comment = "Under Development"
        )

[<AbstractClass>]
type Context<'context when 'context :> IEnvironment> (logging : ILogging) as self =
    inherit CustomContext<'context, ContextSpec<EnvironmentProps>, EnvironmentProps> (logging, new ContextSpec<EnvironmentProps>(EnvironmentKind, EnvironmentProps.Create))
    let inspect = base.Handlers.Add<unit, Json> (E.unit, D.unit, E.json, D.json, "inspect")
    let version : Lazy<Version> = lazy (loadVersion logging)
    let preferences : Lazy<IPreferences> = lazy (Feature.create<IPreferences> (logging))
    let secureStorage : Lazy<ISecureStorage> = lazy (Feature.create<ISecureStorage> (logging))
    do (
        if instance.IsNone then
            instance <- Some (self :> IEnvironment)
    )
    member this.EnvironmentProps : EnvironmentProps = this.Properties
    member __.Inspect : IHandler<unit, Json> = inspect
    interface IEnvironment with
        member this.EnvironmentProps = this.Properties
        member __.Inspect = inspect
        member __.Version = version.Force ()
        member __.Preferences = preferences.Force ()
        member __.SecureStorage = secureStorage.Force ()
    member this.AsEnvironment = this :> IEnvironment

type Context (logging : ILogging) =
    inherit Context<Context> (logging)
    override this.Self = this
    override __.Spawn l = new Context (l)
    interface IFallback
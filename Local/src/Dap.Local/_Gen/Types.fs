[<AutoOpen>]
module Dap.Local.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type SetTextReq = {
    Path : (* SetTextReq *) Luid
    Text : (* SetTextReq *) string
} with
    static member Create
        (
            ?path : (* SetTextReq *) Luid,
            ?text : (* SetTextReq *) string
        ) : SetTextReq =
        {
            Path = (* SetTextReq *) path
                |> Option.defaultWith (fun () -> (newLuid ()))
            Text = (* SetTextReq *) text
                |> Option.defaultWith (fun () -> "")
        }
    static member SetPath ((* SetTextReq *) path : Luid) (this : SetTextReq) =
        {this with Path = path}
    static member SetText ((* SetTextReq *) text : string) (this : SetTextReq) =
        {this with Text = text}
    static member JsonEncoder : JsonEncoder<SetTextReq> =
        fun (this : SetTextReq) ->
            E.object [
                "path", E.string (* SetTextReq *) this.Path
                "text", E.string (* SetTextReq *) this.Text
            ]
    static member JsonDecoder : JsonDecoder<SetTextReq> =
        D.object (fun get ->
            {
                Path = get.Required.Field (* SetTextReq *) "path" D.string
                Text = get.Required.Field (* SetTextReq *) "text" D.string
            }
        )
    static member JsonSpec =
        FieldSpec.Create<SetTextReq> (SetTextReq.JsonEncoder, SetTextReq.JsonDecoder)
    interface IJson with
        member this.ToJson () = SetTextReq.JsonEncoder this
    interface IObj
    member this.WithPath ((* SetTextReq *) path : Luid) =
        this |> SetTextReq.SetPath path
    member this.WithText ((* SetTextReq *) text : string) =
        this |> SetTextReq.SetText text

(*
 * Generated: <Combo>
 *)
type EnvironmentProps (owner : IOwner, key : Key) =
    inherit WrapProperties<EnvironmentProps, IComboProperty> ()
    let target' = Properties.combo (owner, key)
    let dataDirectory = target'.AddVar<(* EnvironmentProps *) string> (E.string, D.string, "data_directory", "data", None)
    let cacheDirectory = target'.AddVar<(* EnvironmentProps *) string> (E.string, D.string, "cache_directory", "cache", None)
    do (
        base.Setup (target')
    )
    static member Create (o, k) = new EnvironmentProps (o, k)
    static member Create () = EnvironmentProps.Create (noOwner, NoKey)
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<EnvironmentProps> (EnvironmentProps.Create, key)
    override this.Self = this
    override __.Spawn (o, k) = EnvironmentProps.Create (o, k)
    override __.SyncTo t = target'.SyncTo t.Target
    member __.DataDirectory (* EnvironmentProps *) : IVarProperty<string> = dataDirectory
    member __.CacheDirectory (* EnvironmentProps *) : IVarProperty<string> = cacheDirectory

(*
 * Generated: <Context>
 *)
type IEnvironment =
    inherit IFeature
    inherit IContext<EnvironmentProps>
    abstract EnvironmentProps : EnvironmentProps with get
    abstract Inspect : IHandler<unit, Json> with get

(*
 * Generated: <Context>
 *)
[<Literal>]
let EnvironmentKind = "Environment"

[<AbstractClass>]
type BaseEnvironment<'context when 'context :> IEnvironment> (logging : ILogging) =
    inherit CustomContext<'context, ContextSpec<EnvironmentProps>, EnvironmentProps> (logging, new ContextSpec<EnvironmentProps>(EnvironmentKind, EnvironmentProps.Create))
    let inspect = base.Handlers.Add<unit, Json> (E.unit, D.unit, E.json, D.json, "inspect")
    member this.EnvironmentProps : EnvironmentProps = this.Properties
    member __.Inspect : IHandler<unit, Json> = inspect
    interface IEnvironment with
        member this.EnvironmentProps : EnvironmentProps = this.Properties
        member __.Inspect : IHandler<unit, Json> = inspect
    interface IFeature
    member this.AsEnvironment = this :> IEnvironment

(*
 * Generated: <Combo>
 *)
type PreferencesProps (owner : IOwner, key : Key) =
    inherit WrapProperties<PreferencesProps, IComboProperty> ()
    let target' = Properties.combo (owner, key)
    let root = target'.AddVar<(* PreferencesProps *) string> (E.string, D.string, "root", "preferences", None)
    do (
        base.Setup (target')
    )
    static member Create (o, k) = new PreferencesProps (o, k)
    static member Create () = PreferencesProps.Create (noOwner, NoKey)
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<PreferencesProps> (PreferencesProps.Create, key)
    override this.Self = this
    override __.Spawn (o, k) = PreferencesProps.Create (o, k)
    override __.SyncTo t = target'.SyncTo t.Target
    member __.Root (* PreferencesProps *) : IVarProperty<string> = root

(*
 * Generated: <Context>
 *)
type IPreferences =
    inherit IFeature
    inherit IContext<PreferencesProps>
    abstract PreferencesProps : PreferencesProps with get
    abstract Has : IHandler<Luid, bool> with get
    abstract Get : IHandler<Luid, string option> with get
    abstract Set : IHandler<SetTextReq, unit> with get
    abstract Remove : IHandler<Luid, unit> with get
    abstract Clear : IHandler<unit, unit> with get

(*
 * Generated: <Context>
 *)
[<Literal>]
let PreferencesKind = "Preferences"

[<AbstractClass>]
type BasePreferences<'context when 'context :> IPreferences> (logging : ILogging) =
    inherit CustomContext<'context, ContextSpec<PreferencesProps>, PreferencesProps> (logging, new ContextSpec<PreferencesProps>(PreferencesKind, PreferencesProps.Create))
    let has = base.Handlers.Add<Luid, bool> (E.string, D.string, E.bool, D.bool, "has")
    let get = base.Handlers.Add<Luid, string option> (E.string, D.string, (E.option E.string), (D.option D.string), "get")
    let set = base.Handlers.Add<SetTextReq, unit> (SetTextReq.JsonEncoder, SetTextReq.JsonDecoder, E.unit, D.unit, "set")
    let remove = base.Handlers.Add<Luid, unit> (E.string, D.string, E.unit, D.unit, "remove")
    let clear = base.Handlers.Add<unit, unit> (E.unit, D.unit, E.unit, D.unit, "clear")
    member this.PreferencesProps : PreferencesProps = this.Properties
    member __.Has : IHandler<Luid, bool> = has
    member __.Get : IHandler<Luid, string option> = get
    member __.Set : IHandler<SetTextReq, unit> = set
    member __.Remove : IHandler<Luid, unit> = remove
    member __.Clear : IHandler<unit, unit> = clear
    interface IPreferences with
        member this.PreferencesProps : PreferencesProps = this.Properties
        member __.Has : IHandler<Luid, bool> = has
        member __.Get : IHandler<Luid, string option> = get
        member __.Set : IHandler<SetTextReq, unit> = set
        member __.Remove : IHandler<Luid, unit> = remove
        member __.Clear : IHandler<unit, unit> = clear
    interface IFeature
    member this.AsPreferences = this :> IPreferences

(*
 * Generated: <Combo>
 *)
type SecureStorageProps (owner : IOwner, key : Key) =
    inherit WrapProperties<SecureStorageProps, IComboProperty> ()
    let target' = Properties.combo (owner, key)
    let root = target'.AddVar<(* SecureStorageProps *) string> (E.string, D.string, "root", "secure_storage", None)
    let secret = target'.AddVar<(* SecureStorageProps *) string> (E.string, D.string, "secret", "Iemohwai9iiY2phojael2och7quiex6Thohneothaek7eeghaebeewohghie9shu", None)
    do (
        base.Setup (target')
    )
    static member Create (o, k) = new SecureStorageProps (o, k)
    static member Create () = SecureStorageProps.Create (noOwner, NoKey)
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<SecureStorageProps> (SecureStorageProps.Create, key)
    override this.Self = this
    override __.Spawn (o, k) = SecureStorageProps.Create (o, k)
    override __.SyncTo t = target'.SyncTo t.Target
    member __.Root (* SecureStorageProps *) : IVarProperty<string> = root
    member __.Secret (* SecureStorageProps *) : IVarProperty<string> = secret

(*
 * Generated: <Context>
 *)
type ISecureStorage =
    inherit IFeature
    inherit IContext<SecureStorageProps>
    abstract SecureStorageProps : SecureStorageProps with get
    abstract Remove : IHandler<Luid, unit> with get
    abstract Clear : IHandler<unit, unit> with get
    abstract HasAsync : IAsyncHandler<Luid, bool> with get
    abstract GetAsync : IAsyncHandler<Luid, string option> with get
    abstract SetAsync : IAsyncHandler<SetTextReq, unit> with get

(*
 * Generated: <Context>
 *)
[<Literal>]
let SecureStorageKind = "SecureStorage"

[<AbstractClass>]
type BaseSecureStorage<'context when 'context :> ISecureStorage> (logging : ILogging) =
    inherit CustomContext<'context, ContextSpec<SecureStorageProps>, SecureStorageProps> (logging, new ContextSpec<SecureStorageProps>(SecureStorageKind, SecureStorageProps.Create))
    let remove = base.Handlers.Add<Luid, unit> (E.string, D.string, E.unit, D.unit, "remove")
    let clear = base.Handlers.Add<unit, unit> (E.unit, D.unit, E.unit, D.unit, "clear")
    let hasAsync = base.AsyncHandlers.Add<Luid, bool> (E.string, D.string, E.bool, D.bool, "has")
    let getAsync = base.AsyncHandlers.Add<Luid, string option> (E.string, D.string, (E.option E.string), (D.option D.string), "get")
    let setAsync = base.AsyncHandlers.Add<SetTextReq, unit> (SetTextReq.JsonEncoder, SetTextReq.JsonDecoder, E.unit, D.unit, "set")
    member this.SecureStorageProps : SecureStorageProps = this.Properties
    member __.Remove : IHandler<Luid, unit> = remove
    member __.Clear : IHandler<unit, unit> = clear
    member __.HasAsync : IAsyncHandler<Luid, bool> = hasAsync
    member __.GetAsync : IAsyncHandler<Luid, string option> = getAsync
    member __.SetAsync : IAsyncHandler<SetTextReq, unit> = setAsync
    interface ISecureStorage with
        member this.SecureStorageProps : SecureStorageProps = this.Properties
        member __.Remove : IHandler<Luid, unit> = remove
        member __.Clear : IHandler<unit, unit> = clear
        member __.HasAsync : IAsyncHandler<Luid, bool> = hasAsync
        member __.GetAsync : IAsyncHandler<Luid, string option> = getAsync
        member __.SetAsync : IAsyncHandler<SetTextReq, unit> = setAsync
    interface IFeature
    member this.AsSecureStorage = this :> ISecureStorage
[<AutoOpen>]
module Dap.Local.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type Version = {
    Major : (* Version *) int
    Minor : (* Version *) int
    Patch : (* Version *) int
    Commit : (* Version *) string
    Comment : (* Version *) string
} with
    static member Create
        (
            ?major : (* Version *) int,
            ?minor : (* Version *) int,
            ?patch : (* Version *) int,
            ?commit : (* Version *) string,
            ?comment : (* Version *) string
        ) : Version =
        {
            Major = (* Version *) major
                |> Option.defaultWith (fun () -> 0)
            Minor = (* Version *) minor
                |> Option.defaultWith (fun () -> 0)
            Patch = (* Version *) patch
                |> Option.defaultWith (fun () -> 0)
            Commit = (* Version *) commit
                |> Option.defaultWith (fun () -> "")
            Comment = (* Version *) comment
                |> Option.defaultWith (fun () -> "")
        }
    static member SetMajor ((* Version *) major : int) (this : Version) =
        {this with Major = major}
    static member SetMinor ((* Version *) minor : int) (this : Version) =
        {this with Minor = minor}
    static member SetPatch ((* Version *) patch : int) (this : Version) =
        {this with Patch = patch}
    static member SetCommit ((* Version *) commit : string) (this : Version) =
        {this with Commit = commit}
    static member SetComment ((* Version *) comment : string) (this : Version) =
        {this with Comment = comment}
    static member JsonEncoder : JsonEncoder<Version> =
        fun (this : Version) ->
            E.object [
                "major", E.int (* Version *) this.Major
                "minor", E.int (* Version *) this.Minor
                "patch", E.int (* Version *) this.Patch
                "commit", E.string (* Version *) this.Commit
                "comment", E.string (* Version *) this.Comment
            ]
    static member JsonDecoder : JsonDecoder<Version> =
        D.object (fun get ->
            {
                Major = get.Required.Field (* Version *) "major" D.int
                Minor = get.Required.Field (* Version *) "minor" D.int
                Patch = get.Required.Field (* Version *) "patch" D.int
                Commit = get.Required.Field (* Version *) "commit" D.string
                Comment = get.Required.Field (* Version *) "comment" D.string
            }
        )
    static member JsonSpec =
        FieldSpec.Create<Version> (Version.JsonEncoder, Version.JsonDecoder)
    interface IJson with
        member this.ToJson () = Version.JsonEncoder this
    interface IObj
    member this.WithMajor ((* Version *) major : int) =
        this |> Version.SetMajor major
    member this.WithMinor ((* Version *) minor : int) =
        this |> Version.SetMinor minor
    member this.WithPatch ((* Version *) patch : int) =
        this |> Version.SetPatch patch
    member this.WithCommit ((* Version *) commit : string) =
        this |> Version.SetCommit commit
    member this.WithComment ((* Version *) comment : string) =
        this |> Version.SetComment comment


type IVersion =
#if !FABLE_COMPILER
    inherit Dap.Platform.Cli.ICliHook
#endif
    abstract Major : int with get
    abstract Minor : int with get
    abstract Patch : int with get
    abstract Commit : string with get
    abstract Comment : string with get

[<AutoOpen>]
module IVersionExtensions =
    type IVersion with
        member this.SemVer =
            sprintf "%i.%i.%i" this.Major this.Minor this.Patch
        member this.DevVer =
            this.SemVer
            |> (fun x ->
                if System.String.IsNullOrEmpty this.Commit then
                    x
                else
                    sprintf "%s-%s" x this.Commit
            )
        member this.ToVersion () =
            Version.Create (
                major = this.Major,
                minor = this.Minor,
                patch = this.Patch,
                commit = this.Commit,
                comment = this.Comment
            )


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
    abstract Version : Version with get
    abstract Preferences : IPreferences with get
    abstract SecureStorage : ISecureStorage with get
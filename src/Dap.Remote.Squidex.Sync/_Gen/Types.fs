[<AutoOpen>]
module Dap.Remote.Squidex.Sync.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote.FSharpData
open Dap.Remote.Squidex

(*
 * Generated: <Record>
 *     IsJson
 *)
type SyncSnapshot = {
    Id : (* SyncSnapshot *) string
    Time : (* SyncSnapshot *) Instant
    Queries : (* SyncSnapshot *) Map<string, string>
    Contents : (* SyncSnapshot *) Map<string, ContentsWithTotalResult>
    Errors : (* SyncSnapshot *) Map<string, string>
} with
    static member Create
        (
            ?id : (* SyncSnapshot *) string,
            ?time : (* SyncSnapshot *) Instant,
            ?queries : (* SyncSnapshot *) Map<string, string>,
            ?contents : (* SyncSnapshot *) Map<string, ContentsWithTotalResult>,
            ?errors : (* SyncSnapshot *) Map<string, string>
        ) : SyncSnapshot =
        {
            Id = (* SyncSnapshot *) id
                |> Option.defaultWith (fun () -> "")
            Time = (* SyncSnapshot *) time
                |> Option.defaultWith (fun () -> (getNow' ()))
            Queries = (* SyncSnapshot *) queries
                |> Option.defaultWith (fun () -> Map.empty)
            Contents = (* SyncSnapshot *) contents
                |> Option.defaultWith (fun () -> Map.empty)
            Errors = (* SyncSnapshot *) errors
                |> Option.defaultWith (fun () -> Map.empty)
        }
    static member SetId ((* SyncSnapshot *) id : string) (this : SyncSnapshot) =
        {this with Id = id}
    static member SetTime ((* SyncSnapshot *) time : Instant) (this : SyncSnapshot) =
        {this with Time = time}
    static member SetQueries ((* SyncSnapshot *) queries : Map<string, string>) (this : SyncSnapshot) =
        {this with Queries = queries}
    static member SetContents ((* SyncSnapshot *) contents : Map<string, ContentsWithTotalResult>) (this : SyncSnapshot) =
        {this with Contents = contents}
    static member SetErrors ((* SyncSnapshot *) errors : Map<string, string>) (this : SyncSnapshot) =
        {this with Errors = errors}
    static member JsonEncoder : JsonEncoder<SyncSnapshot> =
        fun (this : SyncSnapshot) ->
            E.object [
                yield "id", E.string (* SyncSnapshot *) this.Id
                yield "time", E.instant (* SyncSnapshot *) this.Time
                yield "queries", (E.dict E.string) (* SyncSnapshot *) this.Queries
                yield "contents", (E.dict ContentsWithTotalResult.JsonEncoder) (* SyncSnapshot *) this.Contents
                yield "errors", (E.dict E.string) (* SyncSnapshot *) this.Errors
            ]
    static member JsonDecoder : JsonDecoder<SyncSnapshot> =
        D.object (fun get ->
            {
                Id = get.Required.Field (* SyncSnapshot *) "id" D.string
                Time = get.Required.Field (* SyncSnapshot *) "time" D.instant
                Queries = get.Required.Field (* SyncSnapshot *) "queries" (D.dict D.string)
                Contents = get.Required.Field (* SyncSnapshot *) "contents" (D.dict ContentsWithTotalResult.JsonDecoder)
                Errors = get.Required.Field (* SyncSnapshot *) "errors" (D.dict D.string)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<SyncSnapshot> (SyncSnapshot.JsonEncoder, SyncSnapshot.JsonDecoder)
    interface IJson with
        member this.ToJson () = SyncSnapshot.JsonEncoder this
    interface IObj
    member this.WithId ((* SyncSnapshot *) id : string) =
        this |> SyncSnapshot.SetId id
    member this.WithTime ((* SyncSnapshot *) time : Instant) =
        this |> SyncSnapshot.SetTime time
    member this.WithQueries ((* SyncSnapshot *) queries : Map<string, string>) =
        this |> SyncSnapshot.SetQueries queries
    member this.WithContents ((* SyncSnapshot *) contents : Map<string, ContentsWithTotalResult>) =
        this |> SyncSnapshot.SetContents contents
    member this.WithErrors ((* SyncSnapshot *) errors : Map<string, string>) =
        this |> SyncSnapshot.SetErrors errors

(*
 * Generated: <Union>
 *     IsJson
 *)
type SyncResult =
    | Succeed of snapshot : SyncSnapshot
    | Failed of error : string
with
    static member CreateSucceed snapshot : SyncResult =
        Succeed (snapshot)
    static member CreateFailed error : SyncResult =
        Failed (error)
    static member JsonSpec' : CaseSpec<SyncResult> list =
        [
            CaseSpec<SyncResult>.Create ("Succeed", [
                SyncSnapshot.JsonSpec
            ])
            CaseSpec<SyncResult>.Create ("Failed", [
                S.string
            ])
        ]
    static member JsonEncoder = E.union SyncResult.JsonSpec'
    static member JsonDecoder = D.union SyncResult.JsonSpec'
    static member JsonSpec =
        FieldSpec.Create<SyncResult> (SyncResult.JsonEncoder, SyncResult.JsonDecoder)
    interface IJson with
        member this.ToJson () = SyncResult.JsonEncoder this

(*
 * Generated: <Combo>
 *)
type SyncProps (owner : IOwner, key : Key) =
    inherit WrapProperties<SyncProps, IComboProperty> ()
    let target' = Properties.combo (owner, key)
    let config = target'.AddVar<(* SyncProps *) SquidexConfig> (SquidexConfig.JsonEncoder, SquidexConfig.JsonDecoder, "config", (SquidexConfig.Create ()), None)
    let reloadInterval = target'.AddVar<(* SyncProps *) Duration option> ((E.option E.duration), (D.option D.duration), "reload_interval", None, None)
    let snapshots = target'.AddDict<(* SyncProps *) SyncSnapshot> (SyncSnapshot.JsonEncoder, SyncSnapshot.JsonDecoder, "snapshots", (SyncSnapshot.Create ()), None)
    let loading = target'.AddVar<(* SyncProps *) bool> (E.bool, D.bool, "loading", false, None)
    let lastSnapshotId = target'.AddVar<(* SyncProps *) string> (E.string, D.string, "last_snapshot_id", "", None)
    let lastLoadedTime = target'.AddVar<(* SyncProps *) Instant> (E.instant, D.instant, "last_loaded_time", (getNow' ()), None)
    do (
        base.Setup (target')
    )
    static member Create (o, k) = new SyncProps (o, k)
    static member Create () = SyncProps.Create (noOwner, NoKey)
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<SyncProps> (SyncProps.Create, key)
    override this.Self = this
    override __.Spawn (o, k) = SyncProps.Create (o, k)
    override __.SyncTo t = target'.SyncTo t.Target
    member __.Config (* SyncProps *) : IVarProperty<SquidexConfig> = config
    member __.ReloadInterval (* SyncProps *) : IVarProperty<Duration option> = reloadInterval
    member __.Snapshots (* SyncProps *) : IDictProperty<IVarProperty<SyncSnapshot>> = snapshots
    member __.Loading (* SyncProps *) : IVarProperty<bool> = loading
    member __.LastSnapshotId (* SyncProps *) : IVarProperty<string> = lastSnapshotId
    member __.LastLoadedTime (* SyncProps *) : IVarProperty<Instant> = lastLoadedTime

(*
 * Generated: <Context>
 *)
type ISyncConfig =
    inherit IFeature
    inherit IContext<SyncProps>
    abstract SyncProps : SyncProps with get
    abstract OnLoaded : IChannel<SyncSnapshot> with get
    abstract GetNextSnapshotId : IHandler<unit, string> with get
    abstract ReloadAsync : IAsyncHandler<unit, SyncResult> with get

(*
 * Generated: <Context>
 *)
[<Literal>]
let SyncConfigKind = "SyncConfig"

[<AbstractClass>]
type BaseSyncConfig<'context when 'context :> ISyncConfig> (logging : ILogging) =
    inherit CustomContext<'context, ContextSpec<SyncProps>, SyncProps> (logging, new ContextSpec<SyncProps>(SyncConfigKind, SyncProps.Create))
    let onLoaded = base.Channels.Add<SyncSnapshot> (SyncSnapshot.JsonEncoder, SyncSnapshot.JsonDecoder, "on_loaded")
    let getNextSnapshotId = base.Handlers.Add<unit, string> (E.unit, D.unit, E.string, D.string, "get_next_snapshot_id")
    let reloadAsync = base.AsyncHandlers.Add<unit, SyncResult> (E.unit, D.unit, SyncResult.JsonEncoder, SyncResult.JsonDecoder, "reload")
    member this.SyncProps : SyncProps = this.Properties
    member __.OnLoaded : IChannel<SyncSnapshot> = onLoaded
    member __.GetNextSnapshotId : IHandler<unit, string> = getNextSnapshotId
    member __.ReloadAsync : IAsyncHandler<unit, SyncResult> = reloadAsync
    interface ISyncConfig with
        member this.SyncProps : SyncProps = this.Properties
        member __.OnLoaded : IChannel<SyncSnapshot> = onLoaded
        member __.GetNextSnapshotId : IHandler<unit, string> = getNextSnapshotId
        member __.ReloadAsync : IAsyncHandler<unit, SyncResult> = reloadAsync
    interface IFeature
    member this.AsSyncConfig = this :> ISyncConfig
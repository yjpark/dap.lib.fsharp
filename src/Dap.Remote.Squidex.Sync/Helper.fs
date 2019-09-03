[<AutoOpen>]
module Dap.Remote.Squidex.Sync.Helper

open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Platform
open Dap.Remote.Squidex
open Dap.Remote.Squidex.Query
module TickerTypes = Dap.Platform.Ticker.Types

type SyncSnapshot with
    member this.HasError = this.Errors.Count > 0

type ISyncPack with
    member this.SyncConfig = this.Sync.Context
    member this.SyncProps = this.SyncConfig.Properties

let private execQueryAsync (pack : ISyncPack) (query : ContentsQuery) = task {
    let config = pack.SyncProps.Config.Value
    let! response =
        ContentsWithTotal.Args.Create (config, query)
        |> ContentsWithTotal.queryAsync pack
    response.Result
    (*
    |> Result.iter (fun res ->
        logInfo pack "Squidex" "Query" ((SquidexItem.WrapContentsQuery true query).ToQuery ContentsWithTotal.getQueryName)
        logInfo pack "Squidex" "Succeed" (res.Total, res.Items |> List.map (encodeJson 4))
    )
    *)
    |> Result.iterError (fun err ->
        let query = (SquidexItem.WrapContentsQuery true query).ToQuery config ContentsWithTotal.getQueryName
        logError pack "Squidex" "Query_Squidex_Failed"
            (encodeJson 4 config, query, response, err)
    )
    return response.Result
}

let private loadSnapshotAsync (pack : ISyncPack) (queries : Map<string, ContentsQuery>) (snapshotId : string) = task {
    let mutable contents : Map<string, ContentsWithTotalResult> = Map.empty
    let mutable errors : Map<string, string> = Map.empty
    for kv in queries do
        let! content = execQueryAsync pack kv.Value
        match content with
        | Ok content ->
            contents <-
                contents
                |> Map.add kv.Key content
        | Error err ->
            errors <-
                errors
                |> Map.add kv.Key (err.ToString ())
    return SyncSnapshot.Create (
        id = snapshotId,
        contents = contents,
        errors = errors
    )
}

let reloadSyncSnapshotAsync (pack : ISyncPack) (queries : Map<string, ContentsQuery>) = task {
    if pack.SyncProps.Loading.Value then
        logError pack "Reload" "Loading_In_Progress" ()
        return false
    else
        let mutable result = false
        pack.SyncProps.Loading.SetValue (true)
        let snapshotId = pack.SyncConfig.GetNextSnapshotId.Handle ()
        try
            let! snapshot = loadSnapshotAsync pack queries snapshotId
            pack.SyncProps.Snapshots.Add snapshot.Id
            |> (fun prop -> prop.SetValue snapshot)
            if not snapshot.HasError then
                result <- true
            pack.SyncConfig.OnLoaded.FireEvent (snapshot)
        with e ->
            logException pack "Reload" "Exception_Raised" (snapshotId) e
        pack.SyncProps.Loading.SetValue (false)
        return result
}

type ISyncPack with
    member this.RunReloadTask queries =
        this.RunTask0 ignoreOnFailed (fun _runner ->
            reloadSyncSnapshotAsync this queries
            |> Task.map ignore
        )
    member this.SetupSyncing
        (
            config : SquidexConfig,
            queries : Map<string, ContentsQuery>,
            ?reloadInterval : Duration
        ) =
        this.SyncConfig.ReloadAsync.SetupHandler (fun () ->
            reloadSyncSnapshotAsync this queries
        )
        let props = this.SyncProps
        props.Config.SetValue (config)
        props.Config.Seal ()
        props.ReloadInterval.SetValue (reloadInterval)
        props.ReloadInterval.Seal ()
        reloadInterval
        |> Option.iter (fun x ->
            this.Ticker.Actor.OnEvent.AddWatcher this.SyncConfig "ISyncPack.Reload" (fun evt ->
                match evt with
                | TickerTypes.OnTick (time, b) ->
                    if (time - props.LastLoadedTime.Value > x) then
                        if not props.Loading.Value then
                            this.RunReloadTask queries
                | _ -> ()
            )
        )
        props.Snapshots.Clear ()
        props.Loading.SetValue (false)
        this.RunReloadTask queries

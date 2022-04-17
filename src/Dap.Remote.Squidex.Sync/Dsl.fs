module Dap.Remote.Squidex.Sync.Dsl

open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs
open Dap.WebSocket.Meta
open Dap.Remote.Meta
open Dap.Remote.Meta.Net
open Dap.Remote.Squidex.Dsl
let SyncSnapshot =
    combo {
        var (M.string "id")
        var (M.instant "time")
        dict (M.string "queries")
        dict (M.custom (<@ ContentsWithTotalResult @>, "contents"))
        dict (M.string "errors")
    }

let SyncProps =
    combo {
        var (M.custom (<@ SquidexConfig @>, "config"))
        option (M.duration ("reload_interval", NodaTime.Duration.FromMinutes(60L)))
        dict (M.custom (<@ SyncSnapshot @>, "snapshots"))
        var (M.bool "loading")
        var (M.string "last_snapshot_id")
        var (M.instant "last_loaded_time")
    }

let SyncResult =
    union {
        case "Succeed" (fields {
            var (M.custom (<@ SyncSnapshot @>, "snapshot"))
        })
        case "Failed" (fields {
            var (M.string "error")
        })
    }

let SyncConfig =
    context <@ SyncProps @> {
        kind "SyncConfig"
        channel (M.custom (<@ SyncSnapshot @>, "on_loaded"))
        handler (M.unit "get_next_snapshot_id") (M.string response)
        async_handler (M.unit "reload") (M.custom (<@ SyncResult @>, response))
    }

let ISyncPack =
    pack [ <@ ITickingPack @> ] {
        add (M.feature (<@ SyncConfig @>, "sync"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen" ; "Types.fs"],
            G.AutoOpenModule ("Dap.Remote.Squidex.Sync.Types",
                [
                    G.PlatformOpens
                    [ "open Dap.Remote.FSharpData" ]
                    [ "open Dap.Remote.Squidex" ]
                    G.JsonRecord <@ SyncSnapshot @>
                    G.JsonUnion <@ SyncResult @>
                    G.Combo <@ SyncProps @>
                    G.Feature (<@ SyncConfig @>)
                ]
            )
        )
        G.File (segments, ["_Gen" ; "Pack.fs"],
            G.AutoOpenModule ("Dap.Remote.Squidex.Sync.Pack",
                [
                    G.PackOpens
                    [ "open Dap.Remote.Squidex" ]
                    G.PackInterface <@ ISyncPack @>
                ]
            )
        )]

module Dap.Remote.Dashboard.Dsl.Pack

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs
open Dap.Remote.Meta.Net

open Dap.Remote.Dashboard.Meta

let IDashboardPack =
    pack [ <@ ITickingPack @> ] {
        register_pack <@ ITickingPack @> (M.packetConn (logTraffic = true))
        register_pack <@ ITickingPack @>  (M.operatorHub ())
        register (M.operatorHubGateway ())
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Pack.fs"],
            G.AutoOpenModule ("Dap.Remote.Dashboard.Pack",
                [
                    G.PackOpens
                    G.PackInterface <@ IDashboardPack @>
                ]
            )
        )
    ]
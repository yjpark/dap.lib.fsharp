module Dap.Local.Dashboard.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Platform.Dsl

let IDashboardPack =
    pack [] {
        nothing ()
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Dashboard.Types",
                [
                    G.PlatformOpens
                    //G.PackInterface <@ IDashboardPack @>
                ]
            )
        )
    ]
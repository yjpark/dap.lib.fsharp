module Dap.Local.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Platform.Dsl

let FileSystemArgs =
    combo {
        var (M.string ("app_data"))
        var (M.string ("app_cache"))
    }

let ILocalPack =
    pack [] {
        extra (M.jsonArgs ([], <@ FileSystemArgs @>, "file_system"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Types",
                [
                    G.PlatformOpens
                    G.LooseJsonRecord (<@ FileSystemArgs @>)
                    G.PackInterface <@ ILocalPack @>
                ]
            )
        )
        G.File (segments, ["_Gen"; "Builder.fs"],
            G.BuilderModule ("Dap.Local.Builder",
                [
                    G.PlatformOpens
                    G.ValueBuilder <@ FileSystemArgs @>
                ]
            )
        )
    ]
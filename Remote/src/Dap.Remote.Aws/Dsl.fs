module Dap.Remote.Aws.Dsl

open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs

let AwsToken =
    combo {
        var (M.string "key")
        var (M.string "secret")
        option (M.string "info")
    }

let compile segments =
    [
        G.File (segments, ["_Gen" ; "Types.fs"],
            G.AutoOpenModule ("Dap.Remote.Aws.Types",
                [
                    G.PlatformOpens
                    G.JsonRecord <@ AwsToken @>
                ]
            )
        )
    ]

module Dap.Remote.Web.Dsl

open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs

let InvalidBody =
    combo {
        var (M.string "url")
        var (M.string "body")
        var (M.string "error")
        list (M.json "samples")
    }

let compile segments =
    [
        G.File (segments, ["_Gen" ; "HttpTypes.fs"],
            G.AutoOpenQualifiedModule ("Dap.Remote.Web.HttpTypes",
                [
                    G.PlatformOpens
                    G.JsonRecord <@ InvalidBody @>
                ]
            )
        )
    ]

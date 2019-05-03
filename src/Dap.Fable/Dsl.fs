module Dap.Fable.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Platform.Dsl
open Dap.Local.Dsl

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Types",
                [
                    G.PlatformOpens
                    G.JsonRecord (<@ Version @>)
                    [ IVersion ]
                ]
            )
        )
    ]
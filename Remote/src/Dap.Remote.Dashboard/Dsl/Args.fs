module Dap.Remote.Dashboard.Dsl.Args

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Platform.Dsl.Dash

let OperatorArgs =
    combo {
        var (M.string "token")
        var (M.int ("history_size", 5))
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Args.fs"],
            G.AutoOpenModule ("Dap.Remote.Dashboard.Args",
                [
                    G.LooseJsonRecord <@ OperatorArgs @>
                ]
            )
        )
    ]
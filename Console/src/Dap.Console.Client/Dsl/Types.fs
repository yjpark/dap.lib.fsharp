module Dap.Console.Client.Dsl.Types

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator

(*
let Login =
    combo {
        var (M.string ("uri"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "ViewModels.fs"],
            G.AutoOpenModule ("Dap.Console.Client.ViewModels",
                [
                    G.FinalClass (<@ Login @>)
                ]
            )
        )
    ]
*)
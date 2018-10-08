module Dap.Forms.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Local.Dsl

let IFormsPack =
    pack [ <@ ILocalPack @> ] {
        extra (M.jsonArgs ([], <@ FileSystemArgs @>, "temp"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Forms.Types",
                [
                    G.PackOpens
                    [
                        "open Dap.Local"
                    ]
                    G.PackInterface <@ IFormsPack @>
                ]
            )
        )
        G.File (segments, ["_Gen"; "Builder/Types.fs"],
            G.AutoOpenModule ("Dap.Forms.Builder.Types",
                [
                    //G.ValueBuilder <@ FileSystemArgs @>
                ]
            )
        )
    ]

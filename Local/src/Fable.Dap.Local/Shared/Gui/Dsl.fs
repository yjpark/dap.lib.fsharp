module Dap.Local.Gui.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator

let label =
    combo {
        string "text" ""
    }

let button =
    combo {
        string "text" ""
        bool "clickable" true
    }

let compile segments =
    [
        G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Types.fs"],
            G.Module ("Dap.Local.Gui.Types",
                [
                    G.BaseClass ("Label", label)
                    G.BaseClass ("Button", button)
                ]
            )
        )
        G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Builder.fs"],
            G.BuilderModule ("Dap.Local.Gui.Builder",
                [
                    "open Dap.Local.Gui.Types"
                ], [
                    G.Builder ("Label", label)
                    G.Builder ("Button", button)
                ]
            )
        )
    ]
module Dap.Local.Gui.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator

let text =
    combo {
        string "text" ""
    }

let IText = Interface.CreateCombo "IText" text

let label = text

let button =
    extend text {
        bool "clickable" true
    }

let compile segments =
    [
        G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Types.fs"],
            G.Module ("Dap.Local.Gui.Types",
                [
                    G.Interface (IText)
                    G.BaseClass ("Label", [IText], label)
                    G.BaseClass ("Button", [IText], button)
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
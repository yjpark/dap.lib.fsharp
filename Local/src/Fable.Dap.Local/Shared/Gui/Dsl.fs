module Dap.Local.Gui.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator

let label =
    combo {
        string "text" "" None
    }

let compile segments =
    G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Types.fs"],
        G.Module ("Dap.Local.Gui.Types",
            [
                G.LooseJsonRecord ("Label", label)
                G.BaseClass ("LabelProperty", label)
            ]
        )
    )
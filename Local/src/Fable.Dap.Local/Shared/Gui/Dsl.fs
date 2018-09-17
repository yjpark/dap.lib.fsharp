module Dap.Local.Gui.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator

let Text =
    combo {
        var (M.string "text")
    }

let IText = Interface.CreateCombo "IText" Text

let Label = Text

let Button =
    extend [ <@ Text @> ] {
        var (M.bool ("clickable", true))
    }

let compile segments =
    [
        G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Gui.Types",
                [
                    G.Interface (IText)
                    G.BaseClass (<@ Label @>, [IText])
                    G.BaseClass (<@ Button @>, [IText])
                ]
            )
        )
        G.File (segments, ["Shared" ; "Gui" ; "_Gen" ; "Builder.fs"],
            G.BuilderModule ("Dap.Local.Gui.Builder",
                [
                    G.ComboBuilder <@ Label @>
                    G.ComboBuilder <@ Button @>
                ]
            )
        )
    ]

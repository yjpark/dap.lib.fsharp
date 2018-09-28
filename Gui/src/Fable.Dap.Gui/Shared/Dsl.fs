module Dap.Gui.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator

let IWidget =
    combo {
        var (M.string "prefab")
        list (M.string "styles")
    }

let IControl =
    extend [ <@ IWidget @> ] {
        var (M.bool "disabled")
    }

let IGroup =
    extend [ <@ IWidget @> ] {
        var (M.string "layout")
        prop (M.combo "children")
    }

let IText =
    extend [ <@ IWidget @> ] {
        var (M.string "text")
    }

let Label =
    extend [ <@ IText @> ] {
        nothing ()
    }

let Button =
    extend [ <@ IControl @> ; <@ IText @> ] {
        nothing ()
    }

let TextField =
    extend [ <@ IControl @> ; <@ IText @> ] {
        nothing ()
    }

let Group =
    extend [ <@ IGroup @> ] {
        nothing ()
    }

let compile segments =
    [
        G.File (segments, ["Shared" ; "_Gen" ; "Widgets.fs"],
            G.AutoOpenModule ("Dap.Gui.Widgets",
                [
                    G.ComboInterface (<@ IWidget @>, ["ICustomProperties"])
                    G.ComboInterface (<@ IControl @>)
                    G.ComboInterface (<@ IGroup @>)
                    G.ComboInterface (<@ IText @>)
                    G.BaseClass (<@ Group @>)
                    G.FinalClass (<@ Label @>)
                    G.FinalClass (<@ Button @>)
                    G.FinalClass (<@ TextField @>)
                ]
            )
        )
        G.File (segments, ["Shared" ; "_Gen" ; "Builder" ; "Widgets.fs"],
            G.BuilderModule ("Dap.Gui.Builder.Widgets",
                [
                    [
                        "open Dap.Gui"
                    ]
                    G.ComboBuilder <@ Label @>
                    G.ComboBuilder <@ Button @>
                    G.ComboBuilder <@ TextField @>
                ]
            )
        )
        G.File (segments, ["Shared" ; "_Gen" ; "Builder" ; "Internal" ; "Base.fs"],
            G.BuilderModule ("Dap.Gui.Builder.Internal.Base",
                [
                    [
                        "open Dap.Gui"
                    ]
                    G.ComboBuilder <@ Group @>
                ]
            )
        )
    ]

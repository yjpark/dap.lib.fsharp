[<AutoOpen>]
module Dap.Eto.Builder.Helper

open Dap.Prelude
open Dap.Context
open Dap.Gui.Builder

module LayoutConst = Dap.Eto.Layout.Const

type StackBuilder (layout : string) =
    inherit GroupBuilder ()
    override this.Zero () =
        base.Zero ()
        |> fun t -> this.Prefab (t, "stack")
        |> fun t -> this.Layout (t, layout)

let h_stack = new StackBuilder (LayoutConst.Horizontal_Stack)

let v_stack = new StackBuilder (LayoutConst.Vertical_Stack)

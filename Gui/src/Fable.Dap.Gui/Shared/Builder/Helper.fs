[<AutoOpen>]
module Dap.Gui.Builder.Helper

open Dap.Context
open Dap.Gui
module Base = Dap.Gui.Builder.Internal.Base

type GroupBuilder () =
    inherit Base.GroupBuilder ()
    [<CustomOperation("child")>]
    member __.Child (target : Group, key, prop : ICustomProperty) =
        target.Children.AddAny key prop.Clone0 |> ignore
        target

let group = new GroupBuilder ()

type StackBuilder (layout : string) =
    inherit GroupBuilder ()
    override this.Zero () =
        base.Zero ()
        |> fun t -> this.Prefab (t, "stack")
        |> fun t -> this.Layout (t, layout)

let h_stack = new StackBuilder (LayoutConst.Horizontal_Stack)

let v_stack = new StackBuilder (LayoutConst.Vertical_Stack)

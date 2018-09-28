[<AutoOpen>]
module Dap.Gui.Builder.Helper

open Dap.Context
open Dap.Context.Helper
open Dap.Gui
module Base = Dap.Gui.Builder.Internal.Base

type GroupBuilder () =
    inherit Base.GroupBuilder ()
    [<CustomOperation("child")>]
    member __.Child (target : Group, key, prop : ICustomProperty) =
        target.Children.AddAny key prop.Clone0 |> ignore
        target

let group = new GroupBuilder ()
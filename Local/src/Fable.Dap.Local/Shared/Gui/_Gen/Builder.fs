module Dap.Local.Gui.Builder

open Dap.Context
open Dap.Context.Builder

open Dap.Local.Gui.Types

(*
 * Generated: Builder<LabelBuilder>
    {
        "text": ""
    }
 *)
type LabelBuilder () =
    inherit ObjBuilder<LabelProperty> ()
    override __.Zero () = LabelProperty.Empty ()
    [<CustomOperation("text")>]
    member __.Text (this : LabelProperty, v) =
        this.Text.SetValue v |> ignore
        this

let label = LabelBuilder ()
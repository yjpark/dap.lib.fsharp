module Dap.Local.Gui.Builder

open Dap.Context
open Dap.Context.Builder

open Dap.Local.Gui.Types

(*
 * Generated: <Builder>
 *)
type LabelBuilder () =
    inherit ObjBuilder<Label> ()
    override __.Zero () = Label.Empty ()
    [<CustomOperation("text")>]
    member __.Text (target : Label, v) =
        target.Text.SetValue v
        target

let label = LabelBuilder ()

(*
 * Generated: <Builder>
 *)
type ButtonBuilder () =
    inherit ObjBuilder<Button> ()
    override __.Zero () = Button.Empty ()
    [<CustomOperation("text")>]
    member __.Text (target : Button, v) =
        target.Text.SetValue v
        target
    [<CustomOperation("clickable")>]
    member __.Clickable (target : Button, v) =
        target.Clickable.SetValue v
        target

let button = ButtonBuilder ()
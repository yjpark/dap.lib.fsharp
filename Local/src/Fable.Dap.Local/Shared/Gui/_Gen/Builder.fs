module Dap.Local.Gui.Builder

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder

(*
 * Generated: <ComboBuilder>
 *)
type LabelBuilder () =
    inherit ObjBuilder<Label> ()
    override __.Zero () = Label.Default ()
    [<CustomOperation("text")>]
    member __.Text (target : Label, text : string) =
        target.Text.SetValue text
        target

let label = LabelBuilder ()

(*
 * Generated: <ComboBuilder>
 *)
type ButtonBuilder () =
    inherit ObjBuilder<Button> ()
    override __.Zero () = Button.Default ()
    [<CustomOperation("text")>]
    member __.Text (target : Button, text : string) =
        target.Text.SetValue text
        target
    [<CustomOperation("clickable")>]
    member __.Clickable (target : Button, clickable : bool) =
        target.Clickable.SetValue clickable
        target

let button = ButtonBuilder ()
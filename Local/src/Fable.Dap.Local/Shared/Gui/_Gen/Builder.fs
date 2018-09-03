module Dap.Local.Gui.Builder

open Dap.Context
open Dap.Context.Builder

open Dap.Local.Gui.Types

(*
 * Generated: [Builder] <LabelBuilder>
    {
        "text": ""
    }
 *)
type LabelBuilder () =
    inherit ObjBuilder<Label> ()
    override __.Zero () = Label.Empty ()
    [<CustomOperation("text")>]
    member __.Text (target : Label, v) =
        target.Text.SetValue v |> ignore
        target

let label = LabelBuilder ()

(*
 * Generated: [Builder] <ButtonBuilder>
    {
        "clickable": true,
        "text": ""
    }
 *)
type ButtonBuilder () =
    inherit ObjBuilder<Button> ()
    override __.Zero () = Button.Empty ()
    [<CustomOperation("clickable")>]
    member __.Clickable (target : Button, v) =
        target.Clickable.SetValue v |> ignore
        target
    [<CustomOperation("text")>]
    member __.Text (target : Button, v) =
        target.Text.SetValue v |> ignore
        target

let button = ButtonBuilder ()
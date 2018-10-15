[<AutoOpen>]
module Dap.Gui.Builder.Widgets

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Gui

(*
 * Generated: <ComboBuilder>
 *)
type LabelBuilder () =
    inherit ObjBuilder<Label> ()
    override __.Zero () = Label.Default ()
    [<CustomOperation("prefab")>]
    member __.Prefab (target : Label, (* IWidget *) prefab : string) =
        target.Prefab.SetValue prefab
        target
    [<CustomOperation("styles")>]
    member __.Styles (target : Label, (* IWidget *) styles : string list) =
        styles
        |> List.iter (fun v ->
            let prop = target.Styles.Add ()
            prop.SetValue v
        )
        target
    [<CustomOperation("text")>]
    member __.Text (target : Label, (* IText *) text : string) =
        target.Text.SetValue text
        target

let label = LabelBuilder ()

(*
 * Generated: <ComboBuilder>
 *)
type ButtonBuilder () =
    inherit ObjBuilder<Button> ()
    override __.Zero () = Button.Default ()
    [<CustomOperation("prefab")>]
    member __.Prefab (target : Button, (* IWidget *) prefab : string) =
        target.Prefab.SetValue prefab
        target
    [<CustomOperation("styles")>]
    member __.Styles (target : Button, (* IWidget *) styles : string list) =
        styles
        |> List.iter (fun v ->
            let prop = target.Styles.Add ()
            prop.SetValue v
        )
        target
    [<CustomOperation("disabled")>]
    member __.Disabled (target : Button, (* IControl *) disabled : bool) =
        target.Disabled.SetValue disabled
        target
    [<CustomOperation("text")>]
    member __.Text (target : Button, (* IText *) text : string) =
        target.Text.SetValue text
        target

let button = ButtonBuilder ()

(*
 * Generated: <ComboBuilder>
 *)
type TextFieldBuilder () =
    inherit ObjBuilder<TextField> ()
    override __.Zero () = TextField.Default ()
    [<CustomOperation("prefab")>]
    member __.Prefab (target : TextField, (* IWidget *) prefab : string) =
        target.Prefab.SetValue prefab
        target
    [<CustomOperation("styles")>]
    member __.Styles (target : TextField, (* IWidget *) styles : string list) =
        styles
        |> List.iter (fun v ->
            let prop = target.Styles.Add ()
            prop.SetValue v
        )
        target
    [<CustomOperation("disabled")>]
    member __.Disabled (target : TextField, (* IControl *) disabled : bool) =
        target.Disabled.SetValue disabled
        target
    [<CustomOperation("text")>]
    member __.Text (target : TextField, (* IText *) text : string) =
        target.Text.SetValue text
        target

let text_field = TextFieldBuilder ()
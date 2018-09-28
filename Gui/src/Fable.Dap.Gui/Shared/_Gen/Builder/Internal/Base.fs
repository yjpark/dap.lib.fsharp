[<AutoOpen>]
module Dap.Gui.Builder.Internal.Base

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Context.Builder
open Dap.Gui

(*
 * Generated: <ComboBuilder>
 *)
type GroupBuilder () =
    inherit ObjBuilder<Group> ()
    override __.Zero () = Group.Default ()
    [<CustomOperation("prefab")>]
    member __.Prefab (target : Group, (* IWidget *) prefab : string) =
        target.Prefab.SetValue prefab
        target
    [<CustomOperation("styles")>]
    member __.Styles (target : Group, (* IWidget *) styles : string list) =
        styles
        |> List.iter (fun v ->
            let prop = target.Styles.Add ()
            prop.SetValue v
        )
        target
    [<CustomOperation("layout")>]
    member __.Layout (target : Group, (* IGroup *) layout : string) =
        target.Layout.SetValue layout
        target
    [<CustomOperation("children")>]
    member __.Children (target : Group, (* IGroup *) children : IComboProperty) =
        target.Children.SyncWith children
        target

let group = GroupBuilder ()
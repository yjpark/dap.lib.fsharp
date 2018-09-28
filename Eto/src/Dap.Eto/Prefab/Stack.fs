[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Eto.Prefab.Stack

open System
open Eto.Forms

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui

module LayoutConst = Dap.Eto.Layout.Const

type Model = Dap.Gui.Widgets.Group
type Widget = Eto.Forms.StackLayout

type Prefab (owner : IOwner, key : Key) =
    inherit Model (owner, key)
    let widget : Widget = new Widget ()
    do (
        let watcher = IOwner.Create "Stack"
        base.Layout.OnValueChanged.AddWatcher watcher "Prefab" (fun evt ->
            match evt.New with
            | LayoutConst.Horizontal_Stack ->
                widget.Orientation <- Orientation.Horizontal
            | LayoutConst.Vertical_Stack ->
                widget.Orientation <- Orientation.Vertical
            | _ ->
                logError owner "Stack" "Invalid_Layout" evt.New
        )
    )
    member __.AddChild (child : Control, expand : bool) =
        widget.Items.Add <| new StackLayoutItem (child, expand)
    member __.AddChild (child : Control) =
        widget.Items.Add <| new StackLayoutItem (child)
    //SILP: PREFAB_MIXIN
    static member Create o k = new Prefab (o, k)                      //__SILP__
    static member Default () = Prefab.Create noOwner NoKey            //__SILP__
    static member AddToCombo key (combo : IComboProperty) =           //__SILP__
        combo.AddCustom<Prefab> (Prefab.Create, key)                  //__SILP__
    member __.Widget = widget                                         //__SILP__
    member __.Widget' = widget :> Control                             //__SILP__
    interface IPrefab<Widget> with                                    //__SILP__
        member this.Widget = this.Widget                              //__SILP__
    member this.AsPrefab = this :> IPrefab<Widget>                    //__SILP__

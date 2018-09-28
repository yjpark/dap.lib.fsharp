[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Eto.Prefab.TextField

open System
open Eto.Forms

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui

type Model = Dap.Gui.Widgets.TextField
type Widget = Eto.Forms.TextBox

type Prefab (owner : IOwner, key : Key) as this =
    inherit Model (owner, key)
    let widget : Widget = new Widget ()
    do (
        let watcher = IOwner.Create "TextField"
        widget.TextBinding.Bind (
            Func<_> (fun () ->
                this.Text.Value
            ),
            Action<_> (fun v ->
                this.Text.SetValue v
            )
        )|> ignore
        this.Text.OnValueChanged.AddWatcher watcher "Prefab" (fun evt ->
            widget.Text <- evt.New
        )
    )
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

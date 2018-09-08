[<AutoOpen>]
module Dap.Local.Gui.Types

open Dap.Context

(*
 * Generated: <ComboInterface>
 *)
type IText =
    abstract Text : IVarProperty<string> with get

(*
 * Generated: <Class>
 *     IText
 *)
type Label (owner : IOwner, key : Key) =
    inherit WrapProperties<Label, IComboProperty> ()
    let target = Properties.combo owner key
    let text = target.AddVar<string> (E.string, D.string, "text", "", None)
    do (
        base.Setup (target)
    )
    static member Create o k = new Label (o, k)
    static member Empty () = Label.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Label>(Label.Create, key)
    override this.Self = this
    override __.Spawn o k = Label.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Text : IVarProperty<string> = text
    interface IText with
        member this.Text = this.Text

(*
 * Generated: <Class>
 *     IText
 *)
type Button (owner : IOwner, key : Key) =
    inherit WrapProperties<Button, IComboProperty> ()
    let target = Properties.combo owner key
    let text = target.AddVar<string> (E.string, D.string, "text", "", None)
    let clickable = target.AddVar<bool> (E.bool, D.bool, "clickable", true, None)
    do (
        base.Setup (target)
    )
    static member Create o k = new Button (o, k)
    static member Empty () = Button.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Button>(Button.Create, key)
    override this.Self = this
    override __.Spawn o k = Button.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Text : IVarProperty<string> = text
    member __.Clickable : IVarProperty<bool> = clickable
    interface IText with
        member this.Text = this.Text
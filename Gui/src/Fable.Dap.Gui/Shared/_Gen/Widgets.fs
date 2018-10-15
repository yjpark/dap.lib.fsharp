[<AutoOpen>]
module Dap.Gui.Widgets

open Dap.Prelude
open Dap.Context

(*
 * Generated: <ComboInterface>
 *     ICustomProperties
 *)
type IWidget =
    inherit ICustomProperties
    abstract Prefab : IVarProperty<string> with get
    abstract Styles : IListProperty<IVarProperty<string>> with get

(*
 * Generated: <ComboInterface>
 *)
type IControl =
    inherit IWidget
    abstract Disabled : IVarProperty<bool> with get

(*
 * Generated: <ComboInterface>
 *)
type IGroup =
    inherit IWidget
    abstract Layout : IVarProperty<string> with get
    abstract Children : IComboProperty with get

(*
 * Generated: <ComboInterface>
 *)
type IText =
    inherit IWidget
    abstract Text : IVarProperty<string> with get

(*
 * Generated: <Class>
 *)
type Group (owner : IOwner, key : Key) =
    inherit WrapProperties<Group, IComboProperty> ()
    let target = Properties.combo owner key
    let prefab = target.AddVar<(* IWidget *) string> (E.string, D.string, "prefab", "", None)
    let styles = target.AddList<(* IWidget *) string> (E.string, D.string, "styles", "", None)
    let layout = target.AddVar<(* IGroup *) string> (E.string, D.string, "layout", "", None)
    let children = target.AddCombo (* IGroup *) ("children")
    do (
        base.Setup (target)
    )
    static member Create o k = new Group (o, k)
    static member Default () = Group.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Group> (Group.Create, key)
    override this.Self = this
    override __.Spawn o k = Group.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Prefab (* IWidget *) : IVarProperty<string> = prefab
    member __.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = styles
    member __.Layout (* IGroup *) : IVarProperty<string> = layout
    member __.Children (* IGroup *) : IComboProperty = children
    interface IWidget with
        member this.Prefab (* IWidget *) : IVarProperty<string> = this.Prefab
        member this.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = this.Styles
    member this.AsWidget = this :> IWidget
    interface IGroup with
        member this.Layout (* IGroup *) : IVarProperty<string> = this.Layout
        member this.Children (* IGroup *) : IComboProperty = this.Children
    member this.AsGroup = this :> IGroup

(*
 * Generated: <Class>
 *     IsFinal
 *)
type Label (owner : IOwner, key : Key) =
    inherit WrapProperties<Label, IComboProperty> ()
    let target = Properties.combo owner key
    let prefab = target.AddVar<(* IWidget *) string> (E.string, D.string, "prefab", "", None)
    let styles = target.AddList<(* IWidget *) string> (E.string, D.string, "styles", "", None)
    let text = target.AddVar<(* IText *) string> (E.string, D.string, "text", "", None)
    do (
        target.SealCombo ()
        base.Setup (target)
    )
    static member Create o k = new Label (o, k)
    static member Default () = Label.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Label> (Label.Create, key)
    override this.Self = this
    override __.Spawn o k = Label.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Prefab (* IWidget *) : IVarProperty<string> = prefab
    member __.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = styles
    member __.Text (* IText *) : IVarProperty<string> = text
    interface IWidget with
        member this.Prefab (* IWidget *) : IVarProperty<string> = this.Prefab
        member this.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = this.Styles
    member this.AsWidget = this :> IWidget
    interface IText with
        member this.Text (* IText *) : IVarProperty<string> = this.Text
    member this.AsText = this :> IText

(*
 * Generated: <Class>
 *     IsFinal
 *)
type Button (owner : IOwner, key : Key) =
    inherit WrapProperties<Button, IComboProperty> ()
    let target = Properties.combo owner key
    let prefab = target.AddVar<(* IWidget *) string> (E.string, D.string, "prefab", "", None)
    let styles = target.AddList<(* IWidget *) string> (E.string, D.string, "styles", "", None)
    let disabled = target.AddVar<(* IControl *) bool> (E.bool, D.bool, "disabled", false, None)
    let text = target.AddVar<(* IText *) string> (E.string, D.string, "text", "", None)
    do (
        target.SealCombo ()
        base.Setup (target)
    )
    static member Create o k = new Button (o, k)
    static member Default () = Button.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Button> (Button.Create, key)
    override this.Self = this
    override __.Spawn o k = Button.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Prefab (* IWidget *) : IVarProperty<string> = prefab
    member __.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = styles
    member __.Disabled (* IControl *) : IVarProperty<bool> = disabled
    member __.Text (* IText *) : IVarProperty<string> = text
    interface IWidget with
        member this.Prefab (* IWidget *) : IVarProperty<string> = this.Prefab
        member this.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = this.Styles
    member this.AsWidget = this :> IWidget
    interface IControl with
        member this.Disabled (* IControl *) : IVarProperty<bool> = this.Disabled
    member this.AsControl = this :> IControl
    interface IText with
        member this.Text (* IText *) : IVarProperty<string> = this.Text
    member this.AsText = this :> IText

(*
 * Generated: <Class>
 *     IsFinal
 *)
type TextField (owner : IOwner, key : Key) =
    inherit WrapProperties<TextField, IComboProperty> ()
    let target = Properties.combo owner key
    let prefab = target.AddVar<(* IWidget *) string> (E.string, D.string, "prefab", "", None)
    let styles = target.AddList<(* IWidget *) string> (E.string, D.string, "styles", "", None)
    let disabled = target.AddVar<(* IControl *) bool> (E.bool, D.bool, "disabled", false, None)
    let text = target.AddVar<(* IText *) string> (E.string, D.string, "text", "", None)
    do (
        target.SealCombo ()
        base.Setup (target)
    )
    static member Create o k = new TextField (o, k)
    static member Default () = TextField.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<TextField> (TextField.Create, key)
    override this.Self = this
    override __.Spawn o k = TextField.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Prefab (* IWidget *) : IVarProperty<string> = prefab
    member __.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = styles
    member __.Disabled (* IControl *) : IVarProperty<bool> = disabled
    member __.Text (* IText *) : IVarProperty<string> = text
    interface IWidget with
        member this.Prefab (* IWidget *) : IVarProperty<string> = this.Prefab
        member this.Styles (* IWidget *) : IListProperty<IVarProperty<string>> = this.Styles
    member this.AsWidget = this :> IWidget
    interface IControl with
        member this.Disabled (* IControl *) : IVarProperty<bool> = this.Disabled
    member this.AsControl = this :> IControl
    interface IText with
        member this.Text (* IText *) : IVarProperty<string> = this.Text
    member this.AsText = this :> IText
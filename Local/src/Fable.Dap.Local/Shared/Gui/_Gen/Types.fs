[<AutoOpen>]
module Dap.Local.Gui.Types

open Dap.Context

(*
 * Generated: Class<Label>
    {
        "text": ""
    }
 *)
type Label (owner : IOwner, key : Key) =
    inherit WrapProperties<Label, IComboProperty> ("Label")
    let target = Properties.combo owner key
    let text = target.AddString "text" "" None
    do (
        base.Setup (target)
    )
    static member Create o k = new Label (o, k)
    static member Empty () = Label.Create noOwner NoKey
    override this.Self = this
    override __.Spawn o k = Label.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Text = text

(*
 * Generated: Class<Button>
    {
        "clickable": true,
        "text": ""
    }
 *)
type Button (owner : IOwner, key : Key) =
    inherit WrapProperties<Button, IComboProperty> ("Button")
    let target = Properties.combo owner key
    let clickable = target.AddBool "clickable" true None
    let text = target.AddString "text" "" None
    do (
        base.Setup (target)
    )
    static member Create o k = new Button (o, k)
    static member Empty () = Button.Create noOwner NoKey
    override this.Self = this
    override __.Spawn o k = Button.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Clickable = clickable
    member __.Text = text
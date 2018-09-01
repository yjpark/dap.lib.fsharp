[<AutoOpen>]
module Dap.Local.Gui.Types

open Dap.Context

(*
 * Generated: Record<Label>
 *     IsJson, IsLoose
    {
        "text": ""
    }
 *)
type Label = {
    Text : string
} with
    static member Create text
            : Label =
        {
            Text = text
        }
    static member Default () =
        Label.Create ""
    static member JsonEncoder : JsonEncoder<Label> =
        fun (this : Label) ->
            E.object [
                "text", E.string this.Text
            ]
    static member JsonDecoder : JsonDecoder<Label> =
        D.decode Label.Create
        |> D.optional "text" D.string ""
    interface IJson with
        member this.ToJson () = Label.JsonEncoder this
    member this.WithText text = {this with Text = text}

(*
 * Generated: Class<LabelProperty>
    {
        "text": ""
    }
 *)
type LabelProperty (owner : IOwner, key : Key) =
    inherit WrapProperties<LabelProperty, IComboProperty> ("LabelProperty")
    let target = Properties.combo owner key
    let text = target.AddString "text" "" None
    do (
        base.Setup (target)
    )
    static member Create o k = new LabelProperty (o, k)
    static member Empty () = LabelProperty.Create noOwner NoKey
    override this.Self = this
    override __.Spawn o k = LabelProperty.Create o k
    override __.SyncTo t = target.SyncTo t.Target
    member __.Text = text
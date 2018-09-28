[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Console.Client.Prefab.InputField

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Eto.Forms
open Dap.Eto
open Dap.Eto.Prefab

module LayoutConst = Dap.Eto.Layout.Const

let InputFieldJson = parseJson """
{
    "children": {
        "label": {
            "prefab": "",
            "styles": [],
            "text": "Label"
        },
        "value": {
            "disabled": false,
            "prefab": "",
            "styles": [],
            "text": ""
        }
    },
    "layout": "horizontal_stack",
    "prefab": "input_field",
    "styles": [
        "style3"
    ]
}
"""

type Prefab (owner : IOwner, key : Key) as this =
    inherit Stack.Prefab (owner, key)
    let label = Label.Prefab.AddToCombo "label" this.Children
    let value = TextField.Prefab.AddToCombo "value" this.Children
    do (
        this.AsProperty.WithJson InputFieldJson |> ignore
        this.AddChild (label.Widget)
        this.AddChild (value.Widget)
    )
    static member Create o k = new Prefab (o, k)
    static member Default () = Prefab.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Prefab> (Prefab.Create, key)
    member __.Label : Label.Prefab = label
    member __.Value : TextField.Prefab = value
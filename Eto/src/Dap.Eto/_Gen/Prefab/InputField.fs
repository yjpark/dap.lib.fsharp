[<AutoOpen>]
[<RequireQualifiedAccess>]
module LogViewer.Prefab.InputField

open Dap.Prelude
open Dap.Context
open Dap.Gui
open Eto.Forms
open Dap.Eto
open Dap.Eto.Prefab

[<Literal>]
let Kind = "InputField"

let Json = parseJson """
{
    "prefab": "input_field",
    "styles": [
        "style3"
    ],
    "layout": "horizontal_stack",
    "children": {
        "label": {
            "prefab": "",
            "styles": [],
            "text": "Label"
        },
        "value": {
            "prefab": "",
            "styles": [],
            "disabled": false,
            "text": ""
        }
    }
}
"""

type Model = Stack.Model
type Widget = Stack.Model

type Prefab (logging : ILogging) =
    inherit Stack.Prefab (logging)
    let label = Label.Prefab.AddToGroup logging "label" base.Model
    let value = TextField.Prefab.AddToGroup logging "value" base.Model
    do (
        base.Model.AsProperty.WithJson Json |> ignore
        base.AddChild (label.Widget)
        base.AddChild (value.Widget)
    )
    static member Create l = new Prefab (l)
    static member Create () = new Prefab (getLogging ())
    static member AddToGroup l key (group : IGroup) =
        let prefab = Prefab.Create l
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore
        prefab
    member __.Label : Label.Prefab = label
    member __.Value : TextField.Prefab = value
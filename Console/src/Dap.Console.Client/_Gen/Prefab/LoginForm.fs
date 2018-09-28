[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Console.Client.Prefab.LoginForm

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Eto.Forms
open Dap.Eto
open Dap.Eto.Prefab

module LayoutConst = Dap.Eto.Layout.Const

let LoginFormJson = parseJson """
{
    "children": {
        "login": {
            "disabled": false,
            "prefab": "",
            "styles": [],
            "text": "Login"
        },
        "name": {
            "children": {
                "label": {
                    "prefab": "",
                    "styles": [],
                    "text": "User Name"
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
            "styles": []
        },
        "password": {
            "children": {
                "label": {
                    "prefab": "",
                    "styles": [],
                    "text": "Password"
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
            "styles": []
        },
        "title": {
            "prefab": "",
            "styles": [],
            "text": "Login with your credential"
        }
    },
    "layout": "vertical_stack",
    "prefab": "login",
    "styles": [
        "style1",
        "style2"
    ]
}
"""

type Prefab (owner : IOwner, key : Key) as this =
    inherit Stack.Prefab (owner, key)
    let login = Button.Prefab.AddToCombo "login" this.Children
    let name = InputField.Prefab.AddToCombo "name" this.Children
    let password = InputField.Prefab.AddToCombo "password" this.Children
    let title = Label.Prefab.AddToCombo "title" this.Children
    do (
        this.AsProperty.WithJson LoginFormJson |> ignore
        this.AddChild (login.Widget)
        this.AddChild (name.Widget)
        this.AddChild (password.Widget)
        this.AddChild (title.Widget)
    )
    static member Create o k = new Prefab (o, k)
    static member Default () = Prefab.Create noOwner NoKey
    static member AddToCombo key (combo : IComboProperty) =
        combo.AddCustom<Prefab> (Prefab.Create, key)
    member __.Login : Button.Prefab = login
    member __.Name : InputField.Prefab = name
    member __.Password : InputField.Prefab = password
    member __.Title : Label.Prefab = title
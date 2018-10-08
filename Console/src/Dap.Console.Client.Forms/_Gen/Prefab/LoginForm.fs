[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Console.Client.Prefab.LoginForm

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui
open Xamarin.Forms
open Dap.Forms
open Dap.Forms.Prefab

[<Literal>]
let Kind = "LoginForm"

let Json = parseJson """
{
    "prefab": "login",
    "styles": [
        "style1",
        "style2"
    ],
    "layout": "vertical_stack",
    "children": {
        "title": {
            "prefab": "",
            "styles": [],
            "text": "Login with your credential"
        },
        "name": {
            "prefab": "input_field",
            "styles": [],
            "layout": "horizontal_stack",
            "children": {
                "label": {
                    "prefab": "",
                    "styles": [],
                    "text": "User Name"
                },
                "value": {
                    "prefab": "",
                    "styles": [],
                    "disabled": false,
                    "text": ""
                }
            }
        },
        "password": {
            "prefab": "input_field",
            "styles": [],
            "layout": "horizontal_stack",
            "children": {
                "label": {
                    "prefab": "",
                    "styles": [],
                    "text": "Password"
                },
                "value": {
                    "prefab": "",
                    "styles": [],
                    "disabled": false,
                    "text": ""
                }
            }
        },
        "login": {
            "prefab": "",
            "styles": [],
            "disabled": false,
            "text": "Login"
        }
    }
}
"""

type Model = Stack.Model
type Widget = Stack.Model

type Prefab (logging : ILogging) =
    inherit Stack.Prefab (logging)
    let title = Label.Prefab.AddToGroup logging "title" base.Model
    let name = InputField.Prefab.AddToGroup logging "name" base.Model
    let password = InputField.Prefab.AddToGroup logging "password" base.Model
    let login = Button.Prefab.AddToGroup logging "login" base.Model
    do (
        base.Model.AsProperty.WithJson Json |> ignore
        base.AddChild (title.Widget)
        base.AddChild (name.Widget)
        base.AddChild (password.Widget)
        base.AddChild (login.Widget)
    )
    static member Create l = new Prefab (l)
    static member Create () = new Prefab (getLogging ())
    static member AddToGroup l key (group : IGroup) =
        let prefab = Prefab.Create l
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore
        prefab
    member __.Title : Label.Prefab = title
    member __.Name : InputField.Prefab = name
    member __.Password : InputField.Prefab = password
    member __.Login : Button.Prefab = login
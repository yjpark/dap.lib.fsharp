module Dap.Console.Client.Forms.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator
open Dap.Platform
open Dap.Gui
open Dap.Gui.Builder

open Dap.Forms.Builder
open Dap.Forms.Generator
open Dap.Console.Client.Dsl.Prefabs

let compile segments =
    [
        G.PrefabFile (segments, ["_Gen" ; "Prefab" ; "InputField.fs"],
            "Dap.Console.Client.Prefab.InputField", <@ InputField @>
        )
        G.PrefabFile (segments, ["_Gen" ; "Prefab" ; "LoginForm.fs"],
            "Dap.Console.Client.Prefab.LoginForm", <@ LoginForm @>
        )
    ]

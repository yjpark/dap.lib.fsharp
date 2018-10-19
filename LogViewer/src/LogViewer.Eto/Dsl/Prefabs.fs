module LogViewer.Eto.Dsl.Prefabs

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator
open Dap.Platform
open Dap.Gui
open Dap.Gui.Builder

open Dap.Eto.Builder
open Dap.Eto.Generator
open LogViewer.Dsl.Prefabs

let compile segments =
    [
        G.PrefabFile (segments, ["_Gen" ; "Prefab" ; "LoginForm.fs"],
            "LogViewer.Prefab.LoginForm", <@ LoginForm @>
        )
    ]

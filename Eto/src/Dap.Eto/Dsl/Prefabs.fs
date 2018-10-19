module Dap.Eto.Dsl.Prefabs

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator
open Dap.Platform
open Dap.Gui
open Dap.Gui.Builder
open Dap.Gui.Dsl.Prefab

open Dap.Eto.Builder
open Dap.Eto.Generator

let compile segments =
    [
        G.PrefabFile (segments, ["_Gen" ; "Prefab" ; "InputField.fs"],
            "LogViewer.Prefab.InputField", <@ InputField @>
        )
    ]

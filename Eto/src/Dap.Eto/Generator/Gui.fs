module Dap.Eto.Generator.Gui

open Dap.Prelude
open Dap.Context.Generator
open Dap.Gui
open Dap.Gui.Generator
module LayoutConst = Dap.Eto.Layout.Const

type Gui = Gui
with
    interface IGui with
        member __.Opens =
            [
                "open Eto.Forms"
                "open Dap.Eto"
                "open Dap.Eto.Prefab"
            ]
        member __.Aliases =
            [
                "LayoutConst", "Dap.Eto.Layout.Const"
            ]
        member __.GetPrefab (widget : IWidget) (prefab : string) =
            match widget with
            | :? IGroup as group ->
                LayoutConst.getKind group.Layout.Value
                |> Union.getKind
            | _ ->
                prefab
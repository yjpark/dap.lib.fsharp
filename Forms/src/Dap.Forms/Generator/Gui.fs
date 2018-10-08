module Dap.Forms.Generator.Gui

open Dap.Prelude
open Dap.Context.Generator
open Dap.Gui
open Dap.Gui.Generator

type Gui = Gui
with
    interface IGui with
        member __.Opens =
            [
                "open Xamarin.Forms"
                "open Dap.Forms"
                "open Dap.Forms.Prefab"
            ]
        member __.Aliases =
            [
            ]
        member __.GetPrefab (widget : IWidget) (prefab : string) =
            match widget with
            | :? IGroup as group ->
                LayoutConst.getKind group.Layout.Value
                |> Union.getKind
            | _ ->
                prefab
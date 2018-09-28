[<AutoOpen>]
module Dap.Gui.Generator.Types

open Dap.Context.Generator
open Dap.Gui

type IGui =
    abstract Opens : Lines with get
    abstract Aliases : (string * string) list with get
    abstract GetPrefab : IWidget -> string -> string

type PrefabParam = {
    Name : string
} with
    static member Create name : PrefabParam =
        {
            Name = name
        }
    interface IParam with
        member __.Category = "Prefab"
        member this.Name = this.Name
        member this.Desc = ""
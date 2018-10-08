[<AutoOpen>]
module Dap.Forms.Generator.Helper

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context.Generator
open Dap.Gui
open Dap.Gui.Generator
open Dap.Forms.Generator.Gui

type G with
    static member Prefab<'widget when 'widget :> IWidget> (expr : Expr<'widget>) =
        G.Prefab (Gui, expr)
    static member PrefabFile<'widget when 'widget :> IWidget> (segments1 : string list, segments2 : string list, moduleName : string, expr : Expr<'widget>) =
        G.PrefabFile<'widget> (segments1, segments2, moduleName, Gui, expr)

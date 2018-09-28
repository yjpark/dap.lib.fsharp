[<AutoOpen>]
module Dap.Gui.Generator.Helper

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context.Generator
open Dap.Context.Meta.Util
open Dap.Gui

type G with
    static member Prefab (gui : IGui, param : PrefabParam, meta : IWidget) =
        new Prefab.Generator (gui, meta)
        :> IGenerator<PrefabParam>
        |> fun g -> g.Generate param
    static member Prefab (gui : IGui, name, meta) =
        let param = PrefabParam.Create name
        G.Prefab (gui, param, meta)
    static member Prefab<'widget when 'widget :> IWidget> (gui : IGui, expr : Expr<'widget>) =
        let (name, meta) = unquotePropertyGetExpr expr
        G.Prefab (gui, name, meta)

type G with
    static member PrefabFile<'widget when 'widget :> IWidget> (segments1 : string list, segments2 : string list, moduleName : string, gui : IGui, expr : Expr<'widget>) =
        G.File (segments1, segments2,
            G.AutoOpenQualifiedModule (moduleName,
                [
                    G.Prefab (gui, expr)
                ]
            )
        )

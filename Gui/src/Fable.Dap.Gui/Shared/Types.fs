[<AutoOpen>]
module Dap.Gui.Types

open Dap.Prelude
open Dap.Context

type IPrefab =
    inherit IWidget

type IPrefab<'widget> =
    inherit IPrefab
    abstract Widget : 'widget with get

type IView =
    inherit IContext

type IView<'widget when 'widget :> IWidget> =
    inherit IView
    inherit IContext<'widget>
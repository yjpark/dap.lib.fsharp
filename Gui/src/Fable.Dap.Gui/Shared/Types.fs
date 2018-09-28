[<AutoOpen>]
module Dap.Gui.Types

open Dap.Prelude
open Dap.Context

type IPrefab =
    inherit IContext

type IPrefab<'model when 'model :> IWidget> =
    inherit IPrefab
    inherit IContext<'model>

type IPrefab<'model, 'widget when 'model :> IWidget> =
    inherit IPrefab<'model>
    abstract Model : 'model with get
    abstract Widget : 'widget with get

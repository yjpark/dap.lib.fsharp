[<AutoOpen>]
module Dap.Gui.Internal.BasePrefab

open Dap.Prelude
open Dap.Context
open Dap.Gui

[<AbstractClass>]
type BasePrefab<'prefab, 'model, 'widget when 'prefab :> IContext and 'model :> IWidget>
        (logging : ILogging, kind : Kind, spawner, widget : 'widget) =
    inherit CustomContext<'prefab, ContextSpec<'model>, 'model> (logging, new ContextSpec<'model> (kind, spawner))
    member this.Model = this.Properties
    member __.Widget = widget
    interface IPrefab<'model, 'widget> with
        member this.Model = this.Model
        member this.Widget = this.Widget
    member this.AsPrefab = this :> IPrefab<'model, 'widget>
[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Eto.Prefab.Stack

open System
open Eto.Forms

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui

module LayoutConst = Dap.Eto.Layout.Const

[<Literal>]
let Kind = "Stack"

type Model = Dap.Gui.Widgets.Group
type Widget = Eto.Forms.StackLayout

//SILP: PREFAB_HEADER
type Prefab (logging : ILogging) =                                    //__SILP__
    inherit BasePrefab<Prefab, Model, Widget>                         //__SILP__
        (logging, Kind, Model.Create, new Widget ())                  //__SILP__
    do (                                                              //__SILP__
        let owner = base.AsOwner                                      //__SILP__
        let model = base.Model                                        //__SILP__
        let widget = base.Widget                                      //__SILP__
        model.Layout.OnValueChanged.AddWatcher owner Kind (fun evt ->
            match evt.New with
            | LayoutConst.Horizontal_Stack ->
                widget.Orientation <- Orientation.Horizontal
            | LayoutConst.Vertical_Stack ->
                widget.Orientation <- Orientation.Vertical
            | _ ->
                logError owner "Stack" "Invalid_Layout" evt.New
        )
    )
    member this.AddChild (child : Control, expand : bool) =
        this.Widget.Items.Add <| new StackLayoutItem (child, expand)
    member this.AddChild (child : Control) =
        this.Widget.Items.Add <| new StackLayoutItem (child)
    //SILP: PREFAB_FOOTER
    static member Create l = new Prefab (l)                           //__SILP__
    static member Create () = new Prefab (getLogging ())              //__SILP__
    static member AddToGroup l key (group : IGroup) =                 //__SILP__
        let prefab = Prefab.Create l                                  //__SILP__
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore   //__SILP__
        prefab                                                        //__SILP__
    override this.Self = this                                         //__SILP__
    override __.Spawn l = Prefab.Create l                             //__SILP__

[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Forms.Prefab.Stack

open Xamarin.Forms

open Dap.Prelude
open Dap.Context
open Dap.Gui
open Dap.Gui.Internal

[<Literal>]
let Kind = "Label"

type Model = Dap.Gui.Widgets.Group
type Widget = Xamarin.Forms.StackLayout

//SILP: PREFAB_HEADER_MIDDLE
type Prefab (logging : ILogging) =                                    //__SILP__
    inherit BasePrefab<Prefab, Model, Widget>                         //__SILP__
        (logging, Kind, Model.Create, new Widget ())                  //__SILP__
    do (                                                              //__SILP__
        let owner = base.AsOwner                                      //__SILP__
        let model = base.Model                                        //__SILP__
        let widget = base.Widget                                      //__SILP__
        model.Layout.OnChanged.AddWatcher owner Kind (fun evt ->
            match evt.New with
            | LayoutConst.Horizontal_Stack ->
                widget.Orientation <- StackOrientation.Horizontal
            | LayoutConst.Vertical_Stack ->
                widget.Orientation <- StackOrientation.Vertical
            | _ ->
                logError owner "Stack" "Invalid_Layout" evt.New
        )
    )
    member this.AddChild (child : View) =
        this.Widget.Children.Add child
    //SILP: PREFAB_FOOTER
    static member Create l = new Prefab (l)                           //__SILP__
    static member Create () = new Prefab (getLogging ())              //__SILP__
    static member AddToGroup l key (group : IGroup) =                 //__SILP__
        let prefab = Prefab.Create l                                  //__SILP__
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore   //__SILP__
        prefab                                                        //__SILP__
    override this.Self = this                                         //__SILP__
    override __.Spawn l = Prefab.Create l                             //__SILP__

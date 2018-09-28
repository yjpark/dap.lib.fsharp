[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Eto.Prefab.TextField

open System
open Eto.Forms

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui

[<Literal>]
let Kind = "TextField"

type Model = Dap.Gui.Widgets.TextField
type Widget = Eto.Forms.TextBox

//SILP: PREFAB_HEADER
type Prefab (logging : ILogging) =                                    //__SILP__
    inherit BasePrefab<Prefab, Model, Widget>                         //__SILP__
        (logging, Kind, Model.Create, new Widget ())                  //__SILP__
    do (                                                              //__SILP__
        let owner = base.AsOwner                                      //__SILP__
        let model = base.Model                                        //__SILP__
        let widget = base.Widget                                      //__SILP__
        widget.TextBinding.Bind (
            Func<_> (fun () ->
                model.Text.Value
            ),
            Action<_> (fun v ->
                model.Text.SetValue v
            )
        )|> ignore
        model.Text.OnValueChanged.AddWatcher owner Kind (fun evt ->
            widget.Text <- evt.New
        )
    )
    //SILP: PREFAB_FOOTER
    static member Create l = new Prefab (l)                           //__SILP__
    static member Create () = new Prefab (getLogging ())              //__SILP__
    static member AddToGroup l key (group : IGroup) =                 //__SILP__
        let prefab = Prefab.Create l                                  //__SILP__
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore   //__SILP__
        prefab                                                        //__SILP__
    override this.Self = this                                         //__SILP__
    override __.Spawn l = Prefab.Create l                             //__SILP__

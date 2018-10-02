[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Eto.Prefab.Button

open Eto.Forms

open Dap.Prelude
open Dap.Context
open Dap.Gui
open Dap.Gui.Internal

[<Literal>]
let Kind = "Button"

type Model = Dap.Gui.Widgets.Button
type Widget = Eto.Forms.Button

//SILP: PREFAB_HEADER
type Prefab (logging : ILogging) =                                    //__SILP__
    inherit BasePrefab<Prefab, Model, Widget>                         //__SILP__
        (logging, Kind, Model.Create, new Widget ())                  //__SILP__
    let onClick = Model.AddChannels base.Channels
    //SILP: PREFAB_MIDDLE
    do (                                                              //__SILP__
        let owner = base.AsOwner                                      //__SILP__
        let model = base.Model                                        //__SILP__
        let widget = base.Widget                                      //__SILP__
        model.Text.OnChanged.AddWatcher owner Kind (fun evt ->
            widget.Text <- evt.New
        )
        widget.Click.Add (fun _ ->
            onClick.FireEvent ()
        )
    )
    member __.OnClick : IChannel<unit> = onClick
    //SILP: PREFAB_FOOTER
    static member Create l = new Prefab (l)                           //__SILP__
    static member Create () = new Prefab (getLogging ())              //__SILP__
    static member AddToGroup l key (group : IGroup) =                 //__SILP__
        let prefab = Prefab.Create l                                  //__SILP__
        group.Children.AddLink<Model> (prefab.Model, key) |> ignore   //__SILP__
        prefab                                                        //__SILP__
    override this.Self = this                                         //__SILP__
    override __.Spawn l = Prefab.Create l                             //__SILP__

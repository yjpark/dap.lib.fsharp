[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.React.App.WithView

open Fable.Core

open Dap.Prelude
open Dap.Platform
open Dap.Local.App

open Dap.React
module ViewTypes = Dap.React.View.Types
module ViewLogic = Dap.React.View.Logic

type Args<'route, 'model, 'msg, 'parts
            when 'route :> IRoute and 'model : not struct and 'msg :> IMsg> = {
    Base : Simple.Args
    NewParts : Simple.Model -> 'parts
    NewView : 'parts -> Simple.Model -> ViewTypes.View<'route, 'model, 'msg>
} with
    static member Create (base' : Simple.Args) newParts newView =
        {
            Base = base'
            NewParts = newParts
            NewView = newView
        }

and Model<'route, 'model, 'msg, 'parts
            when 'route :> IRoute and 'model : not struct and 'msg :> IMsg> = {
    Args : Args<'route, 'model, 'msg, 'parts>
    Base : Simple.Model
    Parts : 'parts
    View : ViewTypes.View<'route, 'model, 'msg>
} with
    static member Create args (base' : Simple.Model) parts view =
        {
            Args = args
            Base = base'
            Parts = parts
            View = view
        }
    member this.Boot = this.Base.Boot
    member this.Env = this.Base.Env

[<PassGenericsAttribute>]
let init<'route, 'model, 'msg, 'parts
            when 'route :> IRoute and 'model : not struct and 'msg :> IMsg>
        (args : Args<'route, 'model, 'msg, 'parts>)
            : Model<'route, 'model, 'msg, 'parts> =
    let app = Simple.init args.Base
    let parts = args.NewParts app
    let view = args.NewView parts app
    Model<'route, 'model, 'msg, 'parts>.Create args app parts view

let [<PassGenericsAttribute>] newView<'route, 'model, 'msg
            when 'route :> IRoute and 'model : not struct and 'msg :> IMsg>
            (args : ViewTypes.Args<'route, 'model, 'msg>) (app : Simple.Model) =
    let spec = ViewLogic.spec args
    app.Env
    |> Env.spawn spec "View" NoKey
    :?> ViewTypes.View<'route, 'model, 'msg>
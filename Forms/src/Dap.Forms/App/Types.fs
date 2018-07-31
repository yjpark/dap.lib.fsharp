module Dap.Forms.App.Types

open System.Threading.Tasks
open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform

type IRoute =
    abstract Url : string with get

type Widget = ViewElement

type AppIniter<'model, 'msg
            when 'model : not struct and 'msg :> IMsg> =
    IAgent<Msg<'model, 'msg>>

and AppView<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg> =
    'runner -> 'model -> Widget

and AppLogic<'runner, 'model, 'msg
            when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg> =
    Logic<AppIniter<'model, 'msg>, 'runner, unit, 'model, 'msg>

and Args<'runner, 'model, 'msg
            when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg> = {
    Logic : AppLogic<'runner, 'model, 'msg>
    View : AppView<'runner, 'model, 'msg>
    SetupAsync : IEnv -> Task<unit>
    Application : Application
} with
    static member Create init update subscribe view setupAsync application =
        {
            Logic =
                {
                    Init = init
                    Update = update
                    Subscribe = subscribe
                }
            View = view
            SetupAsync = setupAsync
            Application = application
        }

and Model<'model, 'msg
            when 'model : not struct and 'msg :> IMsg> = {

    Round : int
    mutable App : 'model
    Program : Program<'model, 'msg, 'model -> ('msg -> unit) -> Widget>
}

and Req =
    | DoRun of Callback<unit>
with interface IReq

and Evt = NoEvt

and Msg<'model, 'msg
            when 'model : not struct and 'msg :> IMsg> =
    | AppReq of Req
    | AppEvt of Evt
with interface IMsg

and
    [<AbstractClass>]
    App<'runner, 'model, 'msg
            when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg> (param) =
    inherit BaseAgent<'runner, Args<'runner, 'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt> (param)
    let mutable react : ('msg -> unit) option = None
    let mutable formsRunner : obj option = None
    member this.AsApp = this
    member this.Program = this.Actor.State.Program
    member this.AppState = this.Actor.State.App
    member _this.SetFormsRunner' runner =
        formsRunner <- Some runner
    member _this.SetReact' react' =
        react <- Some react'
    member _this.React (msg : 'msg) =
        react
        |> Option.iter (fun d -> d msg)

let castEvt<'model, 'msg when 'model : not struct and 'msg :> IMsg>
                : CastEvt<Msg<'model, 'msg>, Evt> =
    function
    | AppEvt evt -> Some evt
    | _ -> None

let isMockForms () =
    try
        Device.Info = null
    with _ ->
        true

let isRealForms () =
    not <| isMockForms ()
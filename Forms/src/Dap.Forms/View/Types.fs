module Dap.Forms.View.Types

open System.Threading.Tasks
open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform

type Widget = ViewElement

type Initer<'model, 'msg when 'model : not struct and 'msg :> IMsg> =
    IAgent<Msg<'model, 'msg>>

and Render<'model, 'msg when 'model : not struct and 'msg :> IMsg> =
    View<'model, 'msg> -> 'model -> Widget

and ViewLogic<'model, 'msg when 'model : not struct and 'msg :> IMsg> =
    Logic<Initer<'model, 'msg>, View<'model, 'msg>, unit, 'model, 'msg>

and Args<'model, 'msg when 'model : not struct and 'msg :> IMsg> = {
    Logic : ViewLogic<'model, 'msg>
    Render : Render<'model, 'msg>
    Application : Application
} with
    static member Create init update subscribe render application =
        {
            Logic =
                {
                    Init = init
                    Update = update
                    Subscribe = subscribe
                }
            Render = render
            Application = application
        }

and Model<'model, 'msg
            when 'model : not struct and 'msg :> IMsg> = {

    Round : int
    mutable View : 'model
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

and View<'model, 'msg when 'model : not struct and 'msg :> IMsg> (param) =
    inherit BaseAgent<View<'model, 'msg>, Args<'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt> (param)
    let mutable react : ('msg -> unit) option = None
    let mutable formsRunner : obj option = None
    static member Spawn (param) = new View<'model, 'msg> (param)
    override this.Runner = this
    member this.Program = this.Actor.State.Program
    member this.ViewState = this.Actor.State.View
    member this.Application = this.Actor.Args.Application
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

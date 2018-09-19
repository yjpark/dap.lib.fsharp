module Dap.Forms.View.Types

open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Local

[<Literal>]
let Kind = "FormsView"

type Widget = ViewElement

type Initer<'model, 'msg when 'model : not struct and 'msg :> IMsg> =
    IAgent<Msg<'model, 'msg>>

and Render<'pack, 'model, 'msg
            when 'pack :> IPack and 'model : not struct and 'msg :> IMsg> =
    View<'pack, 'model, 'msg> -> 'model -> Widget

and ViewLogic<'pack, 'model, 'msg
            when 'pack :> IPack and 'model : not struct and 'msg :> IMsg> =
    Logic<Initer<'model, 'msg>, View<'pack, 'model, 'msg>, unit, 'model, 'msg>

and Args<'pack, 'model, 'msg
            when 'pack :> IPack and 'model : not struct and 'msg :> IMsg> = {
    Logic : ViewLogic<'pack, 'model, 'msg>
    Render : Render<'pack, 'model, 'msg>
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

and View<'pack, 'model, 'msg when 'pack :> IPack and 'model : not struct and 'msg :> IMsg> (pack : 'pack, param) =
    inherit PackAgent<'pack, View<'pack, 'model, 'msg>, Args<'pack, 'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt> (pack, param)
    let mutable react : ('msg -> unit) option = None
    let mutable formsRunner : obj option = None
    static member Spawn pack' param' = new View<'pack, 'model, 'msg> (pack', param')
    override this.Runner = this
    member this.Program = this.Actor.State.Program
    member this.ViewState = this.Actor.State.View
    member this.Application = this.Actor.Args.Application
    member _this.HasFormsRunner = formsRunner.IsSome
    member _this.SetFormsRunner' runner =
        formsRunner <- Some runner
    member _this.SetReact' react' =
        react <- Some react'
    member _this.React (msg : 'msg) =
        react
        |> Option.iter (fun d -> d msg)
    member this.Run (app : 'app) (onAppStart : 'app -> unit) : unit =
        let onRun = fun () -> onAppStart app
        this.Post <| DoRun (callback this onRun)

let castEvt<'model, 'msg when 'model : not struct and 'msg :> IMsg>
                : CastEvt<Msg<'model, 'msg>, Evt> =
    function
    | AppEvt evt -> Some evt
    | _ -> None

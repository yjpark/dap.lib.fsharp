module Dap.React.View.Types

open Elmish
open Elmish.React
open Elmish.Browser
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import

open Dap.Prelude
open Dap.Platform
open Dap.React

type Initer<'route, 'model, 'msg when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    IAgent<Msg<'route, 'model, 'msg>>

and Render<'route, 'model, 'msg when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    View<'route, 'model, 'msg> -> 'model -> Widget

and ViewLogic<'route, 'model, 'msg
            when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    Logic<Initer<'route, 'model, 'msg>, View<'route, 'model, 'msg>, unit, 'model, 'msg>

and Args<'route, 'model, 'msg
            when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> = {
    Parse : Parser<'route -> 'route, 'route>
    Logic : ViewLogic<'route, 'model, 'msg>
    Render : Render<'route, 'model, 'msg>
    Root : string
    UseHMR : bool
    UseDebugger : bool
} with
    static member Create root parse init update subscribe render =
        {
            Parse = parse
            Logic =
                {
                    Init = init
                    Update = update
                    Subscribe = subscribe
                }
            Render = render
            Root = root
#if DEBUG
            UseHMR = true
            UseDebugger = true
#else
            UseHMR = false
            UseDebugger = false
#endif
        }

and Model<'route, 'model, 'msg
            when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> = {

    Route : 'route option
    Round : int
    mutable View : 'model
    Program : Program<unit, 'model, Navigable<'msg>, Widget>
}

and Req<'route> =
    | DoRoute of 'route
with interface IReq

and Evt =
    | OnReloaded of int
with interface IEvt

and InternalEvt<'route> =
    | SetRoute of 'route
    | RunProgram

and Msg<'route, 'model, 'msg when 'route :> IRoute and 'model : not struct and 'msg :> IMsg> =
    | AppReq of Req<'route>
    | AppEvt of Evt
    | InternalEvt of InternalEvt<'route>
with interface IMsg

and View<'route, 'model, 'msg
            when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> (param) =
    inherit BaseAgent<View<'route, 'model, 'msg>, Args<'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>, Req<'route>, Evt> (param)
    let mutable react : ('msg -> unit) option = None
    static member Spawn (param) = new View<'route, 'model, 'msg> (param)
    override this.Runner = this
    member this.Program = this.Actor.State.Program
    member this.ViewState = this.Actor.State.View
    member this.Route = this.Actor.State.Route |> Option.get
    member this.DoRoute route = this.Deliver <| AppReq ^<| DoRoute route
    member _this.SetReact' react' =
        react <- Some react'
    member _this.React (msg : 'msg) =
        react
        |> Option.iter (fun d -> d msg)

let castEvt<'route, 'model, 'msg when 'route :> IRoute and 'model : not struct and 'msg :> IMsg>
                : CastEvt<Msg<'route, 'model, 'msg>, Evt> =
    function
    | AppEvt evt -> Some evt
    | _ -> None
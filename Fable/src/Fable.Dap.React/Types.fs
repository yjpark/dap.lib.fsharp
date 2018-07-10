module Dap.App.React.Types

open Elmish
open Elmish.React
open Elmish.Browser
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Import

open Dap.Prelude
open Dap.Platform

type IRoute =
    abstract Url : string with get

type Widget = Fable.Import.React.ReactElement

type AppIniter<'route, 'model, 'msg
            when 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    IAgent<Msg<'route, 'model, 'msg>>

and AppView<'runner, 'route, 'model, 'msg when 'runner :> App<'runner, 'route, 'model, 'msg>
                and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    'runner -> 'model -> Widget

and AppLogic<'runner, 'route, 'model, 'msg
            when 'runner :> App<'runner, 'route, 'model, 'msg>
                and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    Logic<AppIniter<'route, 'model, 'msg>, 'runner, unit, 'model, 'msg>

and Args<'runner, 'route, 'model, 'msg
            when 'runner :> App<'runner, 'route, 'model, 'msg>
                and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> = {
    Parse : Parser<'route -> 'route, 'route>
    Logic : AppLogic<'runner, 'route, 'model, 'msg>
    View : AppView<'runner, 'route, 'model, 'msg>
    Root : string
    UseHMR : bool
    UseDebugger : bool
} with
    static member Create root parse init update subscribe view =
        {
            Parse = parse
            Logic =
                {
                    Init = init
                    Update = update
                    Subscribe = subscribe
                }
            View = view
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
    mutable App : 'model
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

and
    [<AbstractClass>]
    App<'runner, 'route, 'model, 'msg
            when 'runner :> App<'runner, 'route, 'model, 'msg>
                and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> (param) =
    inherit BaseAgent<'runner, Args<'runner, 'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>, Req<'route>, Evt> (param)
    let mutable dispatch : ('msg -> unit) option = None
    member this.AsApp = this
    member this.Program = this.Actor.State.Program
    member this.Route = this.Actor.State.Route |> Option.get
    member this.DoRoute route = this.Deliver <| AppReq ^<| DoRoute route
    member _this.SetDispatch' dispatch' =
        dispatch <- Some dispatch'
    member _this.Dispatch (msg : 'msg) =
        dispatch
        |> Option.iter (fun d -> d msg)
    abstract Setup : AppIniter<'route, 'model, 'msg> -> unit

let castEvt<'route, 'model, 'msg when 'route :> IRoute and 'model : not struct and 'msg :> IMsg>
                : CastEvt<Msg<'route, 'model, 'msg>, Evt> =
    function
    | AppEvt evt -> Some evt
    | _ -> None
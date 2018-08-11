[<RequireQualifiedAccess>]
module Dap.Forms.View.Logic

open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Forms

open Dap.Forms.View.Types

type ActorOperate<'model, 'msg when 'model : not struct and 'msg :> IMsg> =
    Operate<View<'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>>

let private doRun req (callback: Callback<unit>) : ActorOperate<'model, 'msg> =
    fun runner (model, cmd) ->
        if isRealForms () then
            runner.RunUiFunc (fun _ ->
                model.Program
                |> Program.runWithDynamicView runner.Actor.Args.Application
                |> runner.SetFormsRunner'
                reply runner callback <| ack req ()
            )
        else
            logError runner "Forms_App" "Is_Mock_Forms" ()
        (model, cmd)

let private handleReq req : ActorOperate<'model, 'msg> =
    match req with
    | DoRun a -> doRun req a

let private update : ActorUpdate<View<'model, 'msg>, Args<'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt> =
    fun runner msg model ->
        match msg with
        | AppReq req -> handleReq req
        | AppEvt _evt -> noOperation
        <| runner <| (model, [])

let private initProgram (initer : Initer<'model, 'msg>) (args : Args<'model, 'msg>) ((initModel, initCmd) : 'model * Cmd<'msg>) =
    let init = fun () ->
        (initModel, initCmd)
    let runner = initer :?> View<'model, 'msg>
    let update = fun (msg : 'msg) (model : 'model) ->
        let (model, cmd) = args.Logic.Update runner msg model
        runner.Actor.State.View <- model
        (model, cmd)
    let mutable firstView = true
    let view = fun (model : 'model) (dispatch : 'msg -> unit) ->
        if (firstView) then
            runner.SetReact' dispatch
            firstView <- false
        args.Render runner model
    let subscribe = fun (model : 'model) ->
        args.Logic.Subscribe runner model
    Program.mkProgram init update view
    |> Program.withSubscription subscribe

let private init : ActorInit<Args<'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>> =
    fun initer args ->
        let (model, cmd) = args.Logic.Init initer ()
        let program = initProgram initer args (model, cmd)
        ({
            Round = 1
            View = model
            Program = program
        }, noCmd)

let spec<'model, 'msg when 'model : not struct and 'msg :> IMsg>
    (args : Args<'model, 'msg>) =
    new ActorSpec<View<'model, 'msg>, Args<'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt>
        (View<'model, 'msg>.Spawn, args, AppReq, castEvt<'model, 'msg>, init, update)

[<RequireQualifiedAccess>]
module Dap.Forms.App.Logic

open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Forms

open Dap.Forms.App.Types

type ActorOperate<'runner, 'model, 'msg
            when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg> =
    Operate<'runner, Model<'model, 'msg>, Msg<'model, 'msg>>

let private runProgram : ActorOperate<'runner, 'model, 'msg> =
    fun runner (model, cmd) ->
        runner.RunUiFunc (fun _ ->
            model.Program |> Program.runWithDynamicView runner.Actor.Args.Application
            |> ignore
        )
        (model, cmd)

let private handleInternalEvt evt : ActorOperate<'runner, 'model, 'msg> =
    match evt with
    | RunProgram ->
        if isRealForms () then
            runProgram
        else
            fun runner (model, cmd) ->
                logError runner "Forms_App" "Is_Mock_Forms" ()
                (model, cmd)

let private update : ActorUpdate<'runner, Args<'runner, 'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt> =
    fun runner model msg ->
        match msg with
        | AppReq _req -> noOperation
        | AppEvt _evt -> noOperation
        | InternalEvt evt -> handleInternalEvt evt
        <| runner <| (model, [])

let private initProgram (initer : IAgent<Msg<'model, 'msg>>) (args : Args<'runner, 'model, 'msg>) ((initModel, initCmd) : 'model * Cmd<'msg>) =
    let init = fun () ->
        (initModel, initCmd)
    let runner = initer :?> 'runner
    let update = fun (msg : 'msg) (model : 'model) ->
        let (model, cmd) = args.Logic.Update runner model msg
        runner.Actor.State.App <- model
        (model, cmd)
    let mutable firstView = true
    let view = fun (model : 'model) (dispatch : 'msg -> unit) ->
        if (firstView) then
            runner.SetReact' dispatch
            firstView <- false
        args.View runner model
    let subscribe = fun (model : 'model) ->
        args.Logic.Subscribe runner model
    Program.mkProgram init update view
    |> Program.withSubscription subscribe

let private init : ActorInit<Args<'runner, 'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>> =
    fun initer args ->
        let (model, cmd) = args.Logic.Init initer ()
        let program = initProgram initer args (model, cmd)
        let model =
            {
                Round = 1
                App = model
                Program = program
            }
        (initer, model, [])
        |=|> addSubCmd InternalEvt RunProgram

let spec<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg>
    (spawn : Spawner<'runner>) (args : Args<'runner, 'model, 'msg>) =
    new ActorSpec<'runner, Args<'runner, 'model, 'msg>, Model<'model, 'msg>, Msg<'model, 'msg>, Req, Evt>
        (spawn, args, AppReq, castEvt<'model, 'msg>, init, update)


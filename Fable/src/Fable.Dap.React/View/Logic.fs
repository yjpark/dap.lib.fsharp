[<RequireQualifiedAccess>]
module Dap.React.View.Logic

open Fable.Core
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.Debug
open Elmish.HMR

open Dap.Prelude
open Dap.Platform
open Dap.React

open Dap.React.View.Types

type ActorOperate<'pack, 'route, 'model, 'msg
            when 'pack :> IPack and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg> =
    Operate<View<'pack, 'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>>

let inline handleReq (req : Req<'route>) : ActorOperate<'pack, 'route, 'model, 'msg> =
    match req with
    | DoRoute route ->
        let dispatch = fun _ -> ()
        (Navigation.modifyUrl route.Url)
        |> List.iter (fun cmd -> cmd dispatch)
        addSubCmd InternalEvt ^<| SetRoute route

let inline runProgram () : ActorOperate<'pack, 'route, 'model, 'msg> =
    fun runner (model, cmd) ->
        let args = runner.Actor.Args
        if args.UseHMR then
            let program = model.Program |> Program.withHMR
            if args.UseDebugger then
                program |> Program.withDebugger |> Program.run
            else
                program |> Program.run
        else
            let program = model.Program
            if args.UseDebugger then
                program |> Program.withDebugger |> Program.run
            else
                program |> Program.run
        (model, cmd)

let inline handleInternalEvt evt : ActorOperate<'pack, 'route, 'model, 'msg> =
    match evt with
    | SetRoute route ->
        updateModel (fun m -> {m with Route = Some route})
    | RunProgram ->
        runProgram ()

let inline update () : ActorUpdate<View<'pack, 'route, 'model, 'msg>, Args<'pack, 'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>, Req<'route>, Evt> =
    fun runner msg model ->
        match msg with
        | AppReq req -> handleReq req
        | AppEvt _evt -> noOperation
        | InternalEvt evt -> handleInternalEvt evt
        <| runner <| (model, [])


let inline initProgram (initer : Initer<'route, 'model, 'msg>) (args : Args<'pack, 'route, 'model, 'msg>) ((initModel, initCmd) : 'model * Cmd<'msg>) =
    let init = fun (route : 'route option) ->
        route
        |> Option.iter (initer.Deliver << InternalEvt << SetRoute)
        (initModel, initCmd)
    let runner = initer :?> View<'pack, 'route, 'model, 'msg>
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
    let route = fun (route : 'route option) (model : 'model) ->
        match route with
        | Some route ->
            runner.Deliver <| InternalEvt ^<| SetRoute route
        | None ->
            logError runner "Route" "Invalid_Route" runner.Actor.State.Route
        (model, noCmd)
    Program.mkProgram init update view
    |> Program.withSubscription subscribe
    |> Program.toNavigable (parseHash args.Parse) route
    |> Program.withReact args.Root

let inline init () : ActorInit<Args<'pack, 'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>> =
    fun initer args ->
        let (model, cmd) = args.Logic.Init initer ()
        let program = initProgram initer args (model, cmd)
        let model =
            {
                Route = None
                Round = 1
                View = model
                Program = program
            }
        (initer, model, [])
        |=|> addSubCmd InternalEvt RunProgram

let inline spec<'pack, 'route, 'model, 'msg
            when 'pack :> IPack and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg>
    pack (args : Args<'pack, 'route, 'model, 'msg>) =
    new ActorSpec<View<'pack, 'route, 'model, 'msg>, Args<'pack, 'route, 'model, 'msg>, Model<'route, 'model, 'msg>, Msg<'route, 'model, 'msg>, Req<'route>, Evt>
        (View<'pack, 'route, 'model, 'msg>.Spawn pack, args, AppReq, castEvt<'route, 'model, 'msg>, init (), update ())

module Dap.Local.Storage.Base.Logic

open Dap.Prelude
open Dap.Platform

open Dap.Local.Storage.Base.Types
open Dap.Local.Storage.Base.Tasks

let private tryLoad req (luid, callback) : ActorOperate<'content, 'v> =
    fun runner (model, cmd) ->
        replyAsync runner req callback (onLoadFailed luid) <| (tryLoadAsync luid)
        (model, cmd)

let private doLoad req (luid, callback) : ActorOperate<'content, 'v> =
    fun runner (model, cmd) ->
        replyAsync runner req callback (onLoadFailed luid) <| (doLoadAsync luid)
        (model, cmd)

let private doSave req (luid, content, callback) : ActorOperate<'content, 'v> =
    fun runner (model, cmd) ->
        let param = SaveParam<'v>.Create luid content
        replyAsync runner req callback (onSaveFailed param) <| (doSaveAsync param)
        (model, cmd)

let private doSaveNew req (luid, content, callback) : ActorOperate<'content, 'v> =
    fun runner (model, cmd) ->
        let param = SaveParam<'v>.CreateForNew luid content
        replyAsync runner req callback (onSaveFailed param) <| (doSaveAsync param)
        (model, cmd)

let private handleReq req : ActorOperate<'content, 'v> =
    fun runner (model, cmd) ->
        match req with
        | TryLoad (a, b) -> tryLoad req (a, b)
        | DoLoad (a, b) -> doLoad req (a, b)
        | DoSave (a, b, c) -> doSave req (a, b, c)
        | DoSaveNew (a, b, c) -> doSaveNew req (a, b, c)
        <| runner <| (model, cmd)

let private update : Update<Agent<'content, 'v>, Model, Msg<'v>> =
    fun runner msg model ->
        match msg with
        | StorageReq req ->
            handleReq req
        | StorageEvt _evt ->
            noOperation
        <| runner <| (model, [])

let private init : Init<IAgent<Msg<'v>>, Args<'content, 'v>, Model, Msg<'v>> =
    fun _runner _args ->
        (NoModel, noCmd)

let spec<'content, 'v> (args : Args<'content, 'v>) =
    new ActorSpec<Agent<'content, 'v>, Args<'content, 'v>, Model, Msg<'v>, Req<'v>, Evt<'v>>
        (Agent<'content, 'v>.Spawn, args, StorageReq, castEvt<'v>, init, update)
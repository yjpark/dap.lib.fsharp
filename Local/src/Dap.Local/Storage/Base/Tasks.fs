module Dap.Local.Storage.Base.Tasks

open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Platform

open Dap.Local.Storage.Base.Types

let internal onLoadFailed (luid : Luid) : OnReplyFailed<Agent<'centent, 'v>, 'res> =
    fun req callback runner e ->
        runner.Deliver <| StorageEvt ^<| OnLoad (luid, Error e)
        nakOnFailed req callback runner e

let private doLoadAsync' (runner : Agent<'content, 'v>) (luid : Luid) = task {
    let! content = runner.Actor.Args.Provider.LoadAsync luid
    return
        content
        |> Option.map (fun content -> runner.Actor.Args.Decode content)
        |> Option.map (fun v ->
            runner.Deliver <| StorageEvt ^<| OnLoad (luid, Ok v)
            v
        )
}

let internal tryLoadAsync (luid : Luid) : GetReplyTask<Agent<'content, 'v>, 'v option> =
    fun req callback runner -> task {
        let! v = doLoadAsync' runner luid
        reply runner callback <| ack req v
    }

let internal doLoadAsync (luid : Luid) : GetReplyTask<Agent<'content, 'v>, 'v> =
    fun req callback runner -> task {
        let! v = doLoadAsync' runner luid
        let v = v |> Option.get
        reply runner callback <| ack req v
    }

let internal onSaveFailed (param : SaveParam<'v>) : OnReplyFailed<Agent<'content, 'v>, 'res> =
    fun req callback runner e ->
        runner.Deliver <| StorageEvt ^<| OnSave (param, Error e)
        nakOnFailed req callback runner e

let internal doSaveAsync (param : SaveParam<'v>) : GetReplyTask<Agent<'content, 'v>, bool> =
    fun req callback runner -> task {
        let content = runner.Actor.Args.Encode param.Value
        let contentParam = SaveParam<'content>.Create' param.Luid content param.AllowOverwrite
        let! isNew = runner.Actor.Args.Provider.SaveAsync contentParam
        runner.Deliver <| StorageEvt ^<| OnSave (param, Ok isNew)
        reply runner callback <| ack req isNew
    }

module Dap.Local.Storage.Base.Types

open System.Threading.Tasks

open Dap.Prelude
open Dap.Context
open Dap.Platform

type SaveParam<'v> = {
    Luid : Luid
    Value : 'v
    AllowOverwrite : bool
} with
    static member Create' luid v allowOverwrite =
        {
            Luid = luid
            Value = v
            AllowOverwrite = allowOverwrite
        }
    static member Create luid v =
        SaveParam<'v>.Create' luid v true
    static member CreateForNew luid v =
        SaveParam<'v>.Create' luid v false

type IProvider<'content> =
    abstract LoadAsync : Luid -> Task<'content option>
    abstract SaveAsync : SaveParam<'content> -> Task<bool>

type Args<'content, 'v> = {
    Provider : IProvider<'content>
    Encode : 'v -> 'content
    Decode : 'content -> 'v
}

and Model = NoModel

and Req<'v> =
    | TryLoad of Luid * Callback<'v option>
    | DoLoad of Luid * Callback<'v>
    | DoSave of Luid * 'v * Callback<bool>
    | DoSaveNew of Luid * 'v * Callback<bool>
with interface IReq

and Evt<'v> =
    | OnLoad of Luid * Result<'v, exn>
    | OnSave of SaveParam<'v> * Result<bool, exn>
with interface IEvt

and Msg<'v> =
    | StorageReq of Req<'v>
    | StorageEvt of Evt<'v>
with interface IMsg

let castEvt<'v> : CastEvt<Msg<'v>, Evt<'v>> =
    function
    | StorageEvt evt -> Some evt
    | _ -> None

let TryLoad path callback =
    TryLoad (path, callback)

let DoLoad path callback =
    DoLoad (path, callback)

let DoSave path content callback =
    DoSave (path, content, callback)

let DoSaveNew path content callback =
    DoSaveNew (path, content, callback)

type Agent<'content, 'v> (param) =
    inherit BaseAgent<Agent<'content, 'v>, Args<'content, 'v>, Model, Msg<'v>, Req<'v>, Evt<'v>> (param)
    override this.Runner = this
    static member Spawn (param) = new Agent<'content, 'v> (param)

type ActorOperate<'content, 'v> = ActorOperate<Agent<'content, 'v>, Args<'content, 'v>, Model, Msg<'v>, Req<'v>, Evt<'v>>

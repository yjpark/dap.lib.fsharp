module Dap.Local.Storage.Json.Service

open Dap.Context
open Dap.Platform

module BaseTypes = Dap.Local.Storage.Base.Types
module Logic = Dap.Local.Storage.Base.Logic

[<Literal>]
let Kind = "JsonStorage"

type Req<'v when 'v :> IJson> = BaseTypes.Req<'v>
type Evt<'v when 'v :> IJson> = BaseTypes.Evt<'v>
type Service<'v when 'v :> IJson> = IAgent<Req<'v>, Evt<'v>>

type SaveParam<'v when 'v :> IJson> = BaseTypes.SaveParam<'v>
type IProvider = BaseTypes.IProvider<string>
type Args<'v when 'v :> IJson> = BaseTypes.Args<string, 'v>

let TryLoad = BaseTypes.TryLoad
let DoLoad = BaseTypes.DoLoad
let DoSave = BaseTypes.DoSave
let DoSaveNew = BaseTypes.DoSaveNew

let addAsync'<'v when 'v :> IJson> kind key provider (indent : int)
                (encoder : JsonEncoder<'v>) (decoder : JsonDecoder<'v>) =
    let encode = fun (v : 'v) -> E.encodeJson indent v
    let decode = fun (content : string) -> decodeJson decoder content
    let args : Args<'v> =
        {
            Provider = provider
            Encode = encode
            Decode = decode
        }
    Env.addServiceAsync (Logic.spec args) kind key

let get'<'v when 'v :> IJson> kind key env =
    env |> Env.getService kind key :?> Service<'v>

let addAsync<'v when 'v :> IJson> key = addAsync'<'v> Kind key
let get<'v when 'v :> IJson> key = get'<'v> Kind key
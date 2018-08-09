[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Forms.View.Service

open Dap.Platform
open Dap.Forms.View.Types

[<Literal>]
let Kind = "View"

let addAsync'<'model, 'msg when 'model : not struct and 'msg :> IMsg> kind key args =
    Env.addServiceAsync (Logic.spec<'model, 'msg> args) kind key

let get'<'model, 'msg when 'model : not struct and 'msg :> IMsg> kind key env =
    env |> Env.getService kind key :?> View<'model, 'msg>

let addAsync<'model, 'msg when 'model : not struct and 'msg :> IMsg> key = addAsync'<'model, 'msg> Kind key
let get<'model, 'msg when 'model : not struct and 'msg :> IMsg> key = get'<'model, 'msg> Kind key
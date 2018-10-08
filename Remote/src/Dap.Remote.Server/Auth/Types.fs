[<AutoOpen>]
module Dap.Remote.Server.Auth.Types

open Dap.Context
open Dap.Remote

type AuthReq = JsonString

type AuthError =
    | InvalidToken
with
    static member JsonEncoder = E.kind<AuthError> ()
    static member JsonDecoder = D.kind<AuthError> ()
    interface IError with
        member this.ToJson () = AuthError.JsonEncoder this


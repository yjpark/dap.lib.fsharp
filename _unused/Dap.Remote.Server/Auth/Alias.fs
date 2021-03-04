[<AutoOpen>]
module Dap.Remote.Server.Auth.Alias

open Dap.Platform

type Token = Token.Record

type UserAuth = UserAuth.Record

let calcCryptoKey (salt : string) (password : string) =
    Sha256.ofText2 password salt

let calcPassHash (salt : string) (password : string) =
    Sha256.ofText2 password salt

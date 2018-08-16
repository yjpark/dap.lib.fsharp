[<AutoOpen>]
module Dap.Remote.Server.Auth.Alias

open Dap.Platform

type Token = Token.Record

type UserAuth = UserAuth.Record

let calcCryptoKey (salt : string) (password : string) =
    calcSha256SumWithSalt salt password

let calcPassHash (salt : string) (password : string) =
    calcSha256SumWithSalt salt password

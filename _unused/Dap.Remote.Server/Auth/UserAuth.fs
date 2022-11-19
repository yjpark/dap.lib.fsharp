[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Server.Auth.UserAuth

open System.Threading.Tasks

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local.Farango
open Dap.Local.Farango.Util

[<Literal>]
let Collection = "user_auth"

type Token = Token.Record

type Record = {
    UserKey : Luid
    UserGuid : Guid
    PassHash : string
    Tokens : Token list
} with
    static member Create userKey userGuid passHash tokens = {
        UserKey = userKey
        UserGuid = userGuid
        PassHash = passHash
        Tokens = tokens
    }
    static member JsonDecoder =
        D.object (fun get ->
            {
                UserKey = get.Required.Field "_key" D.string
                UserGuid = get.Required.Field "user_guid" D.string
                PassHash = get.Required.Field "pass_hash" D.string
                Tokens = get.Optional.Field "tokens" Tokens.JsonDecoder
                    |> Option.defaultValue []
            }
        )
    static member JsonEncoder (this : Record) =
        E.object [
            "_key", E.string this.UserKey
            "user_guid", E.string this.UserGuid
            "pass_hash", E.string this.PassHash
            "tokens", Tokens.JsonEncoder true this.Tokens
        ]
    interface IJson with
        member this.ToJson () = Record.JsonEncoder this
    member this.Key = this.UserKey
    member this.WithTokens tokens = {this with Tokens = tokens}

let getByUserKeyAsync' (collection : string) (userKey : string) (pack : IDbPack) : Task<Result<Record, string>> =
    pack.Db |> Document.getAsync collection userKey Record.JsonDecoder

let getByUserKeyAsync userKey app = getByUserKeyAsync' Collection userKey app
let addTokenAsync token (record : Record) app = Tokens.addTokenAsync Collection token record app
let removeTokenAsync token (record : Record) app = Tokens.removeTokenAsync Collection token record app
let checkToken token (record : Record) app = Tokens.checkToken token record app


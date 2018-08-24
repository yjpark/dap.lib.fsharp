[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Server.Auth.UserAuth

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Platform
open Dap.Remote
open Dap.Local.Farango

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
        D.decode Record.Create
        |> D.required "_key" D.string
        |> D.required "user_guid" D.string
        |> D.required "pass_hash" D.string
        |> D.optional "tokens" Tokens.JsonDecoder []
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

let getByUserKeyAsync' (collection : string) (userKey : string) (app : DbApp) : Task<Result<Record, string>> = task {
    let! doc =
        getDocument app.Db.Conn collection userKey
        |> Async.StartAsTask
    return doc
    |> Result.bind (D.decodeString Record.JsonDecoder)
}

let getByUserKeyAsync userKey app = getByUserKeyAsync' Collection userKey app
let addTokenAsync token (record : Record) app = Tokens.addTokenAsync Collection token record app
let removeTokenAsync token (record : Record) app = Tokens.removeTokenAsync Collection token record app
let checkToken token (record : Record) app = Tokens.checkToken token record app


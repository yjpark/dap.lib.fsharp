[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Server.Auth.Tokens

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Context
open Dap.Local.Farango

type Token = Token.Record

let JsonDecoder =
    D.list Token.JsonDecoder

let JsonEncoder (forPersistent : bool) (this : Token List) =
    this
    |> List.map (fun t ->
        if forPersistent then
            t.ForPersistent
        else
            t
    )|> List.map Token.JsonEncoder
    |> E.list

let inline updateTokens (collection : string) (record : ^record) (app : DbApp) : Task<Result<string, string>> = task {
    let key = (^record : (member Key : string) record)
    let tokens = (^record : (member Tokens : Token list) record)
    let updates =
        E.object [
            "tokens", JsonEncoder true tokens
        ]
    return!
        updateDocument app.Db.Conn collection key <| E.encode 0 updates
        |> Async.StartAsTask
}

let inline addTokenAsync (collection : string) (token : Token) (record : ^record) (app : DbApp) : Task<Result< ^record * Token, string>> = task {
    let tokens = (^record : (member Tokens : Token list) record)
    let tokens = token :: tokens
    let record = (^record : (member WithTokens : Token list -> ^record) (record, tokens))
    let! doc = app |> updateTokens collection record
    return doc |> Result.map (fun _ -> (record, token))
}

let inline removeTokenAsync (collection : string) (token : Token) (record : ^record) (app : DbApp) : Task<Result< ^record * bool, string>> = task {
    let tokens = (^record : (member Tokens : Token list) record)
    let length = tokens.Length
    let tokens =
        tokens
        |> List.filter (fun t -> t.Guid <> token.Guid)
    if (tokens.Length = length) then
        return Ok (record, false)
    else
        let record = (^record : (member WithTokens : Token list -> ^record) (record, tokens))
        let! doc = app |> updateTokens collection record
        return doc
        |> Result.map (fun _ -> (record, true))
}

let inline checkToken (token : Token) (record : ^record) (app : DbApp) : Result< ^record * Token, string> =
    let tokens = (^record : (member Tokens : Token list) record)
    try
        tokens
        |> List.find (fun t -> t.Guid = token.Guid)
        |> fun t -> {t with CryptoKey = token.CryptoKey}
        |> Token.check app.Env
        |> Result.map (fun token -> (record, token))
        |> Result.mapError (fun err ->
            logError app.Env "Auth" "Invalid_Token" (token, err)
            err
        )
    with e ->
        logError app.Env "Auth" "Token_Not_Found" (token, tokens.Length, e)
        Error <| "Token_Not_Found"


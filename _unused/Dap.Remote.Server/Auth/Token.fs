[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Server.Auth.Token

open Jose

open Dap.Prelude
open Dap.Context
open Dap.Platform

[<StructuredFormatDisplay("{AsDisplay}")>]
type Record = {
    Guid : Guid
    OwnerKey : string
    CryptoKey : string
    Data : Json
    Time : int64
    Expiration : int64
} with
    static member Create id ownerKey cryptoKey data time expiration = {
        Guid = id
        OwnerKey = ownerKey
        CryptoKey = cryptoKey
        Data = data
        Time = time
        Expiration = expiration
    }
    static member JsonDecoder =
        D.object (fun get ->
            {
                Guid = get.Required.Field "jti" D.string
                OwnerKey = get.Required.Field "sub" D.string
                CryptoKey = get.Required.Field "key" D.string
                Data = get.Optional.Field "dat" D.value
                    |> Option.defaultValue E.nil
                Time = get.Required.Field "iat" D.long
                Expiration = get.Required.Field "exp" D.long
            }
        )
    static member JsonEncoder (this : Record) =
        E.object [
            "jti", E.string this.Guid
            "sub", E.string this.OwnerKey
            "key", E.string this.CryptoKey
            "dat", this.Data
            "iat", E.long this.Time
            "exp", E.long this.Expiration
        ]
    member this.AsDisplay = (this.Guid, this.OwnerKey, this.Time, this.Expiration, this.Data)
    member this.ForPersistent = {this with CryptoKey = ""}
    interface IJson with
        member this.ToJson () = Record.JsonEncoder this

let create (runner : IRunner) (ownerKey : string) (cryptoKey : string) (data : Json) (validFor : Duration) : Record =
    let now = runner.Clock.Now
    let exp = now + validFor
    Record.Create (newGuid ()) ownerKey cryptoKey data (now.ToUnixTimeSeconds()) (exp.ToUnixTimeSeconds())

let toJwt (token : Record) =
    JWT.Encode (token.EncodeJson 0, (unbox null), JwsAlgorithm.none)

let decodeJwt (runner : IRunner) (jwt : string) : Result<Record, string> =
    try
        JWT.Decode (jwt, (unbox null), JwsAlgorithm.none)
        |> D.fromString Record.JsonDecoder
    with e ->
        logError runner "JWT" "Decode_Failed" e
        Error e.Message

let check (runner : IRunner) (token : Record) : Result<Record, string> =
    if token.Guid.Trim() = "" then
        Error "Invalid_Guid"
    elif token.OwnerKey.Trim() = "" then
        Error "Invalid_OwnerKey"
    else
        let now = runner.Clock.Now
        let exp = Instant.FromUnixTimeSeconds token.Expiration
        if now >= exp then
            Error "Token_Expired"
        else
            Ok token
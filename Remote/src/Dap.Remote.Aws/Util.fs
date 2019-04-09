[<AutoOpen>]
module Dap.Remote.Aws.Util

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

let awsToken  : Lazy<AwsToken> = lazy (
    let storage = IEnvironment.Instance.SecureStorage
    storage.GetAsync.Handle AwsTokenLuid
    |> Task.map (fun token ->
        token
        |> Option.map (fun text ->
            decodeJson AwsToken.JsonDecoder text
        )|> Option.defaultWith (fun () ->
            AwsToken.Create (
                key = "EChahD7xi9ohquaib2uoReim",
                secret = "aefahthach7iegh4bohgheepaiLohr2ooteen1ahl5Oos0th",
                info = "dev_fallback"
            )
        )
    )|> syncTask
)

let getAwsToken _ = awsToken.Force ()
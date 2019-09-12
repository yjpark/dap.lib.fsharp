[<AutoOpen>]
module Dap.Remote.Aws.Util

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

let getAwsTokenFromPreferences (luid : Luid) =
    IEnvironment.Instance.Preferences.Get.Handle luid
    |> Option.get
    |> decodeJson AwsToken.JsonDecoder

let getAwsTokenFromSecureStorage (luid : Luid) =
    IEnvironment.Instance.SecureStorage.GetAsync.Handle luid
    |> Task.map (fun content ->
        content
        |> Option.get
        |> decodeJson AwsToken.JsonDecoder
    )|> syncTask

let getS3Config (config : AwsS3Config) =
    //https://docs.min.io/docs/how-to-use-aws-sdk-for-net-with-minio-server.html
    let result = new Amazon.S3.AmazonS3Config ()
    result.RegionEndpoint <- Amazon.RegionEndpoint.GetBySystemName (config.Region)
    result.ForcePathStyle <- true
    result.LogResponse <- true
    config.ServerUrl
    |> Option.iter (fun serverUrl ->
        result.ServiceURL <- serverUrl
    )
    result

let getS3ConfigFromPreferences (luid : Luid) =
    IEnvironment.Instance.Preferences.Get.Handle luid
    |> Option.get
    |> decodeJson AwsS3Config.JsonDecoder
    |> getS3Config

let getS3ConfigFromSecureStorage (luid : Luid) =
    IEnvironment.Instance.SecureStorage.GetAsync.Handle luid
    |> Task.map (fun content ->
        content
        |> Option.get
        |> decodeJson AwsS3Config.JsonDecoder
        |> getS3Config
    )|> syncTask

[<AutoOpen>]
module Dap.Remote.Aws.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type AwsToken = {
    Key : (* AwsToken *) string
    Secret : (* AwsToken *) string
    Info : (* AwsToken *) string option
} with
    static member Create
        (
            ?key : (* AwsToken *) string,
            ?secret : (* AwsToken *) string,
            ?info : (* AwsToken *) string
        ) : AwsToken =
        {
            Key = (* AwsToken *) key
                |> Option.defaultWith (fun () -> "")
            Secret = (* AwsToken *) secret
                |> Option.defaultWith (fun () -> "")
            Info = (* AwsToken *) info
        }
    static member SetKey ((* AwsToken *) key : string) (this : AwsToken) =
        {this with Key = key}
    static member SetSecret ((* AwsToken *) secret : string) (this : AwsToken) =
        {this with Secret = secret}
    static member SetInfo ((* AwsToken *) info : string option) (this : AwsToken) =
        {this with Info = info}
    static member JsonEncoder : JsonEncoder<AwsToken> =
        fun (this : AwsToken) ->
            E.object [
                "key", E.string (* AwsToken *) this.Key
                "secret", E.string (* AwsToken *) this.Secret
                "info", (E.option E.string) (* AwsToken *) this.Info
            ]
    static member JsonDecoder : JsonDecoder<AwsToken> =
        D.object (fun get ->
            {
                Key = get.Required.Field (* AwsToken *) "key" D.string
                Secret = get.Required.Field (* AwsToken *) "secret" D.string
                Info = get.Required.Field (* AwsToken *) "info" (D.option D.string)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<AwsToken> (AwsToken.JsonEncoder, AwsToken.JsonDecoder)
    interface IJson with
        member this.ToJson () = AwsToken.JsonEncoder this
    interface IObj
    member this.WithKey ((* AwsToken *) key : string) =
        this |> AwsToken.SetKey key
    member this.WithSecret ((* AwsToken *) secret : string) =
        this |> AwsToken.SetSecret secret
    member this.WithInfo ((* AwsToken *) info : string option) =
        this |> AwsToken.SetInfo info

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type AwsS3Config = {
    Region : (* AwsS3Config *) string
    ForcePathStyle : (* AwsS3Config *) bool
    LogResponse : (* AwsS3Config *) bool
    ServerUrl : (* AwsS3Config *) string option
} with
    static member Create
        (
            ?region : (* AwsS3Config *) string,
            ?forcePathStyle : (* AwsS3Config *) bool,
            ?logResponse : (* AwsS3Config *) bool,
            ?serverUrl : (* AwsS3Config *) string
        ) : AwsS3Config =
        {
            Region = (* AwsS3Config *) region
                |> Option.defaultWith (fun () -> "us-west-1")
            ForcePathStyle = (* AwsS3Config *) forcePathStyle
                |> Option.defaultWith (fun () -> true)
            LogResponse = (* AwsS3Config *) logResponse
                |> Option.defaultWith (fun () -> true)
            ServerUrl = (* AwsS3Config *) serverUrl
        }
    static member SetRegion ((* AwsS3Config *) region : string) (this : AwsS3Config) =
        {this with Region = region}
    static member SetForcePathStyle ((* AwsS3Config *) forcePathStyle : bool) (this : AwsS3Config) =
        {this with ForcePathStyle = forcePathStyle}
    static member SetLogResponse ((* AwsS3Config *) logResponse : bool) (this : AwsS3Config) =
        {this with LogResponse = logResponse}
    static member SetServerUrl ((* AwsS3Config *) serverUrl : string option) (this : AwsS3Config) =
        {this with ServerUrl = serverUrl}
    static member JsonEncoder : JsonEncoder<AwsS3Config> =
        fun (this : AwsS3Config) ->
            E.object [
                "region", E.string (* AwsS3Config *) this.Region
                "force_path_style", E.bool (* AwsS3Config *) this.ForcePathStyle
                "log_response", E.bool (* AwsS3Config *) this.LogResponse
                "server_url", (E.option E.string) (* AwsS3Config *) this.ServerUrl
            ]
    static member JsonDecoder : JsonDecoder<AwsS3Config> =
        D.object (fun get ->
            {
                Region = get.Optional.Field (* AwsS3Config *) "region" D.string
                    |> Option.defaultValue "us-west-1"
                ForcePathStyle = get.Optional.Field (* AwsS3Config *) "force_path_style" D.bool
                    |> Option.defaultValue true
                LogResponse = get.Optional.Field (* AwsS3Config *) "log_response" D.bool
                    |> Option.defaultValue true
                ServerUrl = get.Optional.Field (* AwsS3Config *) "server_url" D.string
            }
        )
    static member JsonSpec =
        FieldSpec.Create<AwsS3Config> (AwsS3Config.JsonEncoder, AwsS3Config.JsonDecoder)
    interface IJson with
        member this.ToJson () = AwsS3Config.JsonEncoder this
    interface IObj
    member this.WithRegion ((* AwsS3Config *) region : string) =
        this |> AwsS3Config.SetRegion region
    member this.WithForcePathStyle ((* AwsS3Config *) forcePathStyle : bool) =
        this |> AwsS3Config.SetForcePathStyle forcePathStyle
    member this.WithLogResponse ((* AwsS3Config *) logResponse : bool) =
        this |> AwsS3Config.SetLogResponse logResponse
    member this.WithServerUrl ((* AwsS3Config *) serverUrl : string option) =
        this |> AwsS3Config.SetServerUrl serverUrl
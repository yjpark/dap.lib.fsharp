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
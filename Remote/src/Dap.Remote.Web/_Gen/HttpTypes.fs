[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Web.HttpTypes

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type InvalidBody = {
    Url : (* InvalidBody *) string
    Body : (* InvalidBody *) string
    Error : (* InvalidBody *) string
    Samples : (* InvalidBody *) Json list
} with
    static member Create
        (
            ?url : (* InvalidBody *) string,
            ?body : (* InvalidBody *) string,
            ?error : (* InvalidBody *) string,
            ?samples : (* InvalidBody *) Json list
        ) : InvalidBody =
        {
            Url = (* InvalidBody *) url
                |> Option.defaultWith (fun () -> "")
            Body = (* InvalidBody *) body
                |> Option.defaultWith (fun () -> "")
            Error = (* InvalidBody *) error
                |> Option.defaultWith (fun () -> "")
            Samples = (* InvalidBody *) samples
                |> Option.defaultWith (fun () -> [])
        }
    static member SetUrl ((* InvalidBody *) url : string) (this : InvalidBody) =
        {this with Url = url}
    static member SetBody ((* InvalidBody *) body : string) (this : InvalidBody) =
        {this with Body = body}
    static member SetError ((* InvalidBody *) error : string) (this : InvalidBody) =
        {this with Error = error}
    static member SetSamples ((* InvalidBody *) samples : Json list) (this : InvalidBody) =
        {this with Samples = samples}
    static member JsonEncoder : JsonEncoder<InvalidBody> =
        fun (this : InvalidBody) ->
            E.object [
                "url", E.string (* InvalidBody *) this.Url
                "body", E.string (* InvalidBody *) this.Body
                "error", E.string (* InvalidBody *) this.Error
                "samples", (E.list E.json) (* InvalidBody *) this.Samples
            ]
    static member JsonDecoder : JsonDecoder<InvalidBody> =
        D.object (fun get ->
            {
                Url = get.Required.Field (* InvalidBody *) "url" D.string
                Body = get.Required.Field (* InvalidBody *) "body" D.string
                Error = get.Required.Field (* InvalidBody *) "error" D.string
                Samples = get.Required.Field (* InvalidBody *) "samples" (D.list D.json)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<InvalidBody> (InvalidBody.JsonEncoder, InvalidBody.JsonDecoder)
    interface IJson with
        member this.ToJson () = InvalidBody.JsonEncoder this
    interface IObj
    member this.WithUrl ((* InvalidBody *) url : string) =
        this |> InvalidBody.SetUrl url
    member this.WithBody ((* InvalidBody *) body : string) =
        this |> InvalidBody.SetBody body
    member this.WithError ((* InvalidBody *) error : string) =
        this |> InvalidBody.SetError error
    member this.WithSamples ((* InvalidBody *) samples : Json list) =
        this |> InvalidBody.SetSamples samples
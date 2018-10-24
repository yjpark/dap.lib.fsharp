[<AutoOpen>]
module Dap.Local.Farango.Types

open Dap.Prelude
open Dap.Context

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type DbArgs = {
    Uri : (* DbArgs *) string
} with
    static member Create
        (
            ?uri : string
        ) : DbArgs =
        {
            Uri = (* DbArgs *) uri
                |> Option.defaultWith (fun () -> "")
        }
    static member Default () =
        DbArgs.Create (
            "" (* DbArgs *) (* uri *)
        )
    static member SetUri ((* DbArgs *) uri : string) (this : DbArgs) =
        {this with Uri = uri}
    static member JsonEncoder : JsonEncoder<DbArgs> =
        fun (this : DbArgs) ->
            E.object [
                "uri", E.string (* DbArgs *) this.Uri
            ]
    static member JsonDecoder : JsonDecoder<DbArgs> =
        D.object (fun get ->
            {
                Uri = get.Optional.Field (* DbArgs *) "uri" D.string
                    |> Option.defaultValue ""
            }
        )
    static member JsonSpec =
        FieldSpec.Create<DbArgs> (DbArgs.JsonEncoder, DbArgs.JsonDecoder)
    interface IJson with
        member this.ToJson () = DbArgs.JsonEncoder this
    interface IObj
    member this.WithUri ((* DbArgs *) uri : string) =
        this |> DbArgs.SetUri uri
[<AutoOpen>]
module Dap.Local.Farango.Types

open Dap.Prelude
open Dap.Context

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type DbArgs = {
    Uri : string
} with
    static member Create uri
            : DbArgs =
        {
            Uri = uri
        }
    static member Default () =
        DbArgs.Create
            ""
    static member JsonEncoder : JsonEncoder<DbArgs> =
        fun (this : DbArgs) ->
            E.object [
                "uri", E.string this.Uri
            ]
    static member JsonDecoder : JsonDecoder<DbArgs> =
        D.decode DbArgs.Create
        |> D.optional "uri" D.string ""
    static member JsonSpec =
        FieldSpec.Create<DbArgs>
            DbArgs.JsonEncoder DbArgs.JsonDecoder
    interface IJson with
        member this.ToJson () = DbArgs.JsonEncoder this
    interface IObj
    member this.WithUri (uri : string) = {this with Uri = uri}
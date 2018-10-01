[<AutoOpen>]
module Dap.Local.Farango.Types

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type DbArgs = {
    Uri : (* DbArgs *) string
} with
    static member Create uri
            : DbArgs =
        {
            Uri = (* DbArgs *) uri
        }
    static member Default () =
        DbArgs.Create
            "" (* DbArgs *) (* uri *)
    static member SetUri ((* DbArgs *) uri : string) (this : DbArgs) =
        {this with Uri = uri}
    static member UpdateUri ((* DbArgs *) update : string -> string) (this : DbArgs) =
        this |> DbArgs.SetUri (update this.Uri)
    static member JsonEncoder : JsonEncoder<DbArgs> =
        fun (this : DbArgs) ->
            E.object [
                "uri", E.string (* DbArgs *) this.Uri
            ]
    static member JsonDecoder : JsonDecoder<DbArgs> =
        D.decode DbArgs.Create
        |> D.optional (* DbArgs *) "uri" D.string ""
    static member JsonSpec =
        FieldSpec.Create<DbArgs>
            DbArgs.JsonEncoder DbArgs.JsonDecoder
    interface IJson with
        member this.ToJson () = DbArgs.JsonEncoder this
    interface IObj
    member this.WithUri ((* DbArgs *) uri : string) =
        this |> DbArgs.SetUri uri
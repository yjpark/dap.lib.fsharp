[<AutoOpen>]
module Dap.Remote.Squidex.QueryTypes

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Union>
 *)
type ContentField =
    | NoField
    | SimpleValue of key : string * spec : FieldSpec
    | InvariantValue of key : string * spec : FieldSpec
    | LocalizedValue of key : string * spec : FieldSpec
    | SimpleChild of key : string * fields : ContentField list
    | InvariantChild of key : string * fields : ContentField list
    | LocalizedChild of key : string * fields : ContentField list
    | SimpleArray of key : string * fields : ContentField list
    | InvariantArray of key : string * fields : ContentField list
    | LocalizedArray of key : string * fields : ContentField list
    | SimpleLinks of key : string * fields : ContentField list
    | InvariantLinks of key : string * fields : ContentField list
    | LocalizedLinks of key : string * fields : ContentField list
with
    static member CreateNoField () : ContentField =
        NoField
    static member CreateSimpleValue key spec : ContentField =
        SimpleValue (key, spec)
    static member CreateInvariantValue key spec : ContentField =
        InvariantValue (key, spec)
    static member CreateLocalizedValue key spec : ContentField =
        LocalizedValue (key, spec)
    static member CreateSimpleChild key fields : ContentField =
        SimpleChild (key, fields)
    static member CreateInvariantChild key fields : ContentField =
        InvariantChild (key, fields)
    static member CreateLocalizedChild key fields : ContentField =
        LocalizedChild (key, fields)
    static member CreateSimpleArray key fields : ContentField =
        SimpleArray (key, fields)
    static member CreateInvariantArray key fields : ContentField =
        InvariantArray (key, fields)
    static member CreateLocalizedArray key fields : ContentField =
        LocalizedArray (key, fields)
    static member CreateSimpleLinks key fields : ContentField =
        SimpleLinks (key, fields)
    static member CreateInvariantLinks key fields : ContentField =
        InvariantLinks (key, fields)
    static member CreateLocalizedLinks key fields : ContentField =
        LocalizedLinks (key, fields)

(*
 * Generated: <Record>
 *)
type ContentsQuery = {
    Schema : (* ContentsQuery *) string
    Fields : (* ContentsQuery *) ContentField list
    Top : (* ContentsQuery *) int
    Skip : (* ContentsQuery *) int
    Lang : (* ContentsQuery *) string option
    Filter : (* ContentsQuery *) string option
    Orderby : (* ContentsQuery *) string option
} with
    static member Create
        (
            ?schema : (* ContentsQuery *) string,
            ?fields : (* ContentsQuery *) ContentField list,
            ?top : (* ContentsQuery *) int,
            ?skip : (* ContentsQuery *) int,
            ?lang : (* ContentsQuery *) string,
            ?filter : (* ContentsQuery *) string,
            ?orderby : (* ContentsQuery *) string
        ) : ContentsQuery =
        {
            Schema = (* ContentsQuery *) schema
                |> Option.defaultWith (fun () -> "")
            Fields = (* ContentsQuery *) fields
                |> Option.defaultWith (fun () -> [])
            Top = (* ContentsQuery *) top
                |> Option.defaultWith (fun () -> 200)
            Skip = (* ContentsQuery *) skip
                |> Option.defaultWith (fun () -> 0)
            Lang = (* ContentsQuery *) lang
            Filter = (* ContentsQuery *) filter
            Orderby = (* ContentsQuery *) orderby
        }
    static member SetSchema ((* ContentsQuery *) schema : string) (this : ContentsQuery) =
        {this with Schema = schema}
    static member SetFields ((* ContentsQuery *) fields : ContentField list) (this : ContentsQuery) =
        {this with Fields = fields}
    static member SetTop ((* ContentsQuery *) top : int) (this : ContentsQuery) =
        {this with Top = top}
    static member SetSkip ((* ContentsQuery *) skip : int) (this : ContentsQuery) =
        {this with Skip = skip}
    static member SetLang ((* ContentsQuery *) lang : string option) (this : ContentsQuery) =
        {this with Lang = lang}
    static member SetFilter ((* ContentsQuery *) filter : string option) (this : ContentsQuery) =
        {this with Filter = filter}
    static member SetOrderby ((* ContentsQuery *) orderby : string option) (this : ContentsQuery) =
        {this with Orderby = orderby}
    member this.WithSchema ((* ContentsQuery *) schema : string) =
        this |> ContentsQuery.SetSchema schema
    member this.WithFields ((* ContentsQuery *) fields : ContentField list) =
        this |> ContentsQuery.SetFields fields
    member this.WithTop ((* ContentsQuery *) top : int) =
        this |> ContentsQuery.SetTop top
    member this.WithSkip ((* ContentsQuery *) skip : int) =
        this |> ContentsQuery.SetSkip skip
    member this.WithLang ((* ContentsQuery *) lang : string option) =
        this |> ContentsQuery.SetLang lang
    member this.WithFilter ((* ContentsQuery *) filter : string option) =
        this |> ContentsQuery.SetFilter filter
    member this.WithOrderby ((* ContentsQuery *) orderby : string option) =
        this |> ContentsQuery.SetOrderby orderby

(*
 * Generated: <Record>
 *     IsJson
 *)
type ContentsWithTotalResult = {
    Total : (* ContentsWithTotalResult *) int
    Items : (* ContentsWithTotalResult *) SquidexItem list
} with
    static member Create
        (
            ?total : (* ContentsWithTotalResult *) int,
            ?items : (* ContentsWithTotalResult *) SquidexItem list
        ) : ContentsWithTotalResult =
        {
            Total = (* ContentsWithTotalResult *) total
                |> Option.defaultWith (fun () -> 0)
            Items = (* ContentsWithTotalResult *) items
                |> Option.defaultWith (fun () -> [])
        }
    static member SetTotal ((* ContentsWithTotalResult *) total : int) (this : ContentsWithTotalResult) =
        {this with Total = total}
    static member SetItems ((* ContentsWithTotalResult *) items : SquidexItem list) (this : ContentsWithTotalResult) =
        {this with Items = items}
    static member JsonEncoder : JsonEncoder<ContentsWithTotalResult> =
        fun (this : ContentsWithTotalResult) ->
            E.object [
                "total", E.int (* ContentsWithTotalResult *) this.Total
                "items", (E.list SquidexItem.JsonEncoder) (* ContentsWithTotalResult *) this.Items
            ]
    static member JsonDecoder : JsonDecoder<ContentsWithTotalResult> =
        D.object (fun get ->
            {
                Total = get.Required.Field (* ContentsWithTotalResult *) "total" D.int
                Items = get.Required.Field (* ContentsWithTotalResult *) "items" (D.list SquidexItem.JsonDecoder)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<ContentsWithTotalResult> (ContentsWithTotalResult.JsonEncoder, ContentsWithTotalResult.JsonDecoder)
    interface IJson with
        member this.ToJson () = ContentsWithTotalResult.JsonEncoder this
    interface IObj
    member this.WithTotal ((* ContentsWithTotalResult *) total : int) =
        this |> ContentsWithTotalResult.SetTotal total
    member this.WithItems ((* ContentsWithTotalResult *) items : SquidexItem list) =
        this |> ContentsWithTotalResult.SetItems items
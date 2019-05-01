[<AutoOpen>]
module Dap.Remote.Dashboard.Args

open Dap.Prelude
open Dap.Context

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type OperatorArgs = {
    Token : (* OperatorArgs *) string
    HistorySize : (* OperatorArgs *) int
} with
    static member Create
        (
            ?token : (* OperatorArgs *) string,
            ?historySize : (* OperatorArgs *) int
        ) : OperatorArgs =
        {
            Token = (* OperatorArgs *) token
                |> Option.defaultWith (fun () -> "")
            HistorySize = (* OperatorArgs *) historySize
                |> Option.defaultWith (fun () -> 5)
        }
    static member SetToken ((* OperatorArgs *) token : string) (this : OperatorArgs) =
        {this with Token = token}
    static member SetHistorySize ((* OperatorArgs *) historySize : int) (this : OperatorArgs) =
        {this with HistorySize = historySize}
    static member JsonEncoder : JsonEncoder<OperatorArgs> =
        fun (this : OperatorArgs) ->
            E.object [
                "token", E.string (* OperatorArgs *) this.Token
                "history_size", E.int (* OperatorArgs *) this.HistorySize
            ]
    static member JsonDecoder : JsonDecoder<OperatorArgs> =
        D.object (fun get ->
            {
                Token = get.Optional.Field (* OperatorArgs *) "token" D.string
                    |> Option.defaultValue ""
                HistorySize = get.Optional.Field (* OperatorArgs *) "history_size" D.int
                    |> Option.defaultValue 5
            }
        )
    static member JsonSpec =
        FieldSpec.Create<OperatorArgs> (OperatorArgs.JsonEncoder, OperatorArgs.JsonDecoder)
    interface IJson with
        member this.ToJson () = OperatorArgs.JsonEncoder this
    interface IObj
    member this.WithToken ((* OperatorArgs *) token : string) =
        this |> OperatorArgs.SetToken token
    member this.WithHistorySize ((* OperatorArgs *) historySize : int) =
        this |> OperatorArgs.SetHistorySize historySize
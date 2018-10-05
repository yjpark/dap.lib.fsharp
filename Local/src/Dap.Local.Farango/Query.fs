[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Query

open FSharp.Control.Tasks.V2
module Connection = Farango.Connection
module Cursor = Farango.Cursor

open Dap.Prelude
open Dap.Context
open Dap.Local.Farango

type Query = {
    Query : string
    BatchSize : int option
    BindVars : Json option
} with
    override this.ToString () =
        this.BindVars
        |> Option.map (fun bindVars ->
            sprintf " %s" <| E.encode 0 bindVars
            |> (fun s ->
                if not bindVars.IsObject then
                    sprintf "%s (Invalid)" s
                else
                    s
            )
        )|> Option.defaultValue ""
        |> sprintf "%s%s" this.Query
    member this.FailIfInvalid () =
        this.BindVars
        |> Option.iter (fun bindVars ->
            if not bindVars.IsObject then
                failWith "BindVars_Is_Not_Json_Object" <| E.encode 0 bindVars
        )
    static member Create q s v =
        {
            Query = q
            BatchSize = s
            BindVars = v
        }
    static member JsonDecoder : JsonDecoder<Query> =
        D.object (fun get ->
            {
                Query = get.Required.Field "query" D.string
                BatchSize = get.Required.Field "batchSize" (D.option D.int)
                BindVars = get.Required.Field "bindVars" (D.option D.value)
            }
        )
    static member JsonEncoder : JsonEncoder<Query> =
        fun this ->
            E.object [
                yield "query", E.string this.Query
                if this.BatchSize.IsSome then
                    yield "batchSize", E.int this.BatchSize.Value
                if this.BindVars.IsSome then
                    yield "bindVars", this.BindVars.Value
            ]
    interface IJson with
        member this.ToJson () = Query.JsonEncoder this

let executeAsync' (query : Query) (db : Db) = async {
    query.FailIfInvalid ()
    let! firstResult =
        E.encodeJson 0 query
        |> Cursor.getFirstResult Connection.post db.Conn db.CursorUrl
    db.LogResult (sprintf "Execute_Query: %s" (query.ToString ())) firstResult
    return! Cursor.moreResults db.Conn firstResult
}

let executeAsync query db =
    Async.StartAsTask <| executeAsync' query db


type Db.Model with
    member this.GetCollection name = Collection.Create this name
    member this.ExecuteQueryAsync query = executeAsync query this


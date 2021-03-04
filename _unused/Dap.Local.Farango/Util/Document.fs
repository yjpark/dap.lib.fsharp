[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.Document

open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Context
open Dap.Local.Farango

let createAsync' (collection : string) (key : string) (body : string) (db : Db) =
    fun (conn : Connection) -> async {
        let! result = createDocument conn collection body
        db.LogResult (sprintf "Create_Document: %s %s" collection key) result
        return result
    }|> db.ExecAsync'

let createAsync collection key body db =
    Async.StartAsTask <| createAsync' collection key body db

let getAsync'<'res> (collection : string) (key : string) (decoder : JsonDecoder<'res>) (db : Db) =
    fun (conn : Connection) -> async {
        let! doc = getDocument conn collection key
        return doc
        |> Result.bind (D.fromString decoder)
    }|> db.ExecAsync'

let getAsync collection key decoder db =
    Async.StartAsTask <| getAsync' collection key decoder db

let deleteAsync' (collection : string) (key : string) (db : Db) =
    fun (conn : Connection) -> async {
        return! deleteDocument conn collection key
    }|> db.ExecAsync'

let deleteAsync collection key db =
    Async.StartAsTask <| deleteAsync' collection key db

let replaceAsync' (collection : string) (key : string) (body : string) (db : Db) =
    fun (conn : Connection) -> async {
        return! replaceDocument conn collection key body
    }|> db.ExecAsync'

let replaceAsync collection key body db =
    Async.StartAsTask <| replaceAsync' collection key body db

let patchAsync' (collection : string) (key : string) (body : string) (db : Db) =
    fun (conn : Connection) -> async {
        return! updateDocument conn collection key body
    }|> db.ExecAsync'

let patchAsync collection key body db =
    Async.StartAsTask <| patchAsync' collection key body db


[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.Document

open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Context
open Dap.Local.Farango

let createAsync' (collection : string) (key : string) (body : string) (db : Db) = async {
    let! result = createDocument db.Conn collection body
    db.LogResult (sprintf "Create_Document: %s %s" collection key) result
    return result
}

let createAsync collection key body db =
    Async.StartAsTask <| createAsync' collection key body db

let getAsync'<'res> (collection : string) (key : string) (decoder : JsonDecoder<'res>) (db : Db) = async {
    let! doc = getDocument db.Conn collection key
    return doc
    |> Result.bind (D.fromString decoder)
}

let getAsync collection key decoder db =
    Async.StartAsTask <| getAsync' collection key decoder db

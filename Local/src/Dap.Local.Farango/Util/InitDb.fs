[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.InitDb

open FSharp.Control.Tasks.V2
open Farango.Collections

open Dap.Prelude
open Dap.Local.Farango

let createIndexAsync' (collection : string) (def : IndexDef) (db : Db) = async {
    let! result =
        match def.Kind with
        | Hash (unique, sparse, deduplicate) ->
            createHashIndex' db.Conn collection def.Fields unique sparse deduplicate
        | Skiplist (unique, sparse, deduplicate) ->
            createSkiplistIndex' db.Conn collection def.Fields unique sparse deduplicate
        | Persistent (unique, sparse) ->
            createPersistentIndex' db.Conn collection def.Fields unique sparse
    db.LogResult (sprintf "Create_Index: %s - %A" collection def.Fields) result
    return result
}

let createIndexAsync collection def db =
    Async.StartAsTask <| createIndexAsync' collection def db

let createCollectionAsync' (dropFirst : bool) (def : CollectionDef) (db : Db) = async {
    if (dropFirst) then
        let! result = dropCollection db.Conn def.Name
        db.LogResult (sprintf "Drop_Collection: %s" def.Name) result
    let! result = createCollection db.Conn def.Name
    db.LogResult (sprintf "Create_Collection: %s" def.Name) result
    let isNew = result.IsOk
    for index in def.Indexes do
        let! _result = db |> createIndexAsync' def.Name index
        ()
    return isNew
}

let createCollectionAsync dropFirst def db =
    Async.StartAsTask <| createCollectionAsync' dropFirst def db
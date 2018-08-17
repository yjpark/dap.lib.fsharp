[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.InitDb

open FSharp.Control.Tasks.V2
open Farango.Collections

open Dap.Prelude
open Dap.Local.Farango

type Db = Dap.Local.Farango.Db.Model

let createIndexAsync' (collection : string) (def : IndexDef) (db : Db) = async {
    let! result = createIndex db.Conn collection def
    db.LogResult (sprintf "Create_Index: %A - %A" collection def) result
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

let createCollectionsAsync' (dropFirst : bool) (defs : CollectionDef list) (db : Db) = async {
    let mutable hasNew = false
    for def in defs do
        let! isNew = db |> createCollectionAsync' dropFirst def
        if isNew then
            hasNew <- true
    return hasNew
}

let createCollectionsAsync dropFirst defs db =
    Async.StartAsTask <| createCollectionsAsync' dropFirst defs db
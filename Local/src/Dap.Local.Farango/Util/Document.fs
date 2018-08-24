[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.Document

open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Local.Farango

type Db = Dap.Local.Farango.Db.Model

let createAsync' (collection : string) (key : string) (body : string) (db : Db) = async {
    let! result = createDocument db.Conn collection body
    db.LogResult (sprintf "Create_Document: %s %s" collection key) result
    return result
}

let createAsync collection key body db =
    Async.StartAsTask <| createAsync' collection key body db

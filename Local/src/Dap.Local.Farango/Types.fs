[<AutoOpen>]
module Dap.Local.Farango.Types

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

type DbArgs = Dap.Local.Farango.Db.Args

type Db = Dap.Local.Farango.Db.Model

type IndexIsUnique = bool
type IndexIsSparse = bool
type IndexIsDeduplicate = bool

type IndexKind =
    | Hash of IndexIsUnique * IndexIsSparse * IndexIsDeduplicate
    | Skiplist of IndexIsUnique * IndexIsSparse * IndexIsDeduplicate
    | Persistent of IndexIsUnique * IndexIsSparse

type IndexDef = {
    Fields : string list
    Kind : IndexKind
}

type CollectionDef = {
    Name : string
    Indexes : IndexDef list
}

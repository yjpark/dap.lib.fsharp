[<AutoOpen>]
module Dap.Local.Farango.Types

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

type IndexIsUnique = bool
type IndexIsSparse = bool
type IndexIsDeduplicate = bool

type IndexDef = Farango.Collections.IndexSetting

type CollectionDef = {
    Name : string
    Indexes : IndexDef list
} with
    static member Create name indexes =
        {
            Name = name
            Indexes = indexes
        }

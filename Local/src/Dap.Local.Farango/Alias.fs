[<AutoOpen>]
module Dap.Local.Farango.Alias

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

type DbArgs = Dap.Local.Farango.Db.Args
type Db = Dap.Local.Farango.Db.Model

type DbAppArgs = Dap.Local.Farango.App.WithDb.Args
type DbApp = Dap.Local.Farango.App.WithDb.Model
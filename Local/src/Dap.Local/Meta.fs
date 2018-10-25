module Dap.Local.Meta

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Util
open Dap.Context.Generator.Util
open Dap.Platform.Meta

type M with
    static member preferences (?name : string, ?spawner : string, ?kind : Kind, ?key : Key, ?aliases : ModuleAlias list) =
        let name = defaultArg name "IPreferences"
        M.context (name, ?spawner = spawner, ?kind = kind, ?key = key, ?aliases = aliases)

type M with
    static member secureStorage (?name : string, ?spawner : string, ?kind : Kind, ?key : Key, ?aliases : ModuleAlias list) =
        let name = defaultArg name "ISecureStorage"
        M.context (name, ?spawner = spawner, ?kind = kind, ?key = key, ?aliases = aliases)

module Dap.React.Meta

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Util
open Dap.Platform.Meta

type M with
    static member reactView (packRouteModelMsg : string, args : string, ?kind : Kind, ?key : Key, ?aliases : ModuleAlias list) =
        let kind = defaultArg kind Dap.React.View.Types.Kind
        let aliases = defaultArg aliases []
        let alias = "ReactViewTypes", "Dap.React.View.Types"
        let args = CodeArgs (sprintf "ReactViewTypes.Args<%s>" packRouteModelMsg) args
        let type' = sprintf "ReactViewTypes.View<%s>" packRouteModelMsg
        let spec = "Dap.React.View.Logic.spec"
        M.agent (args, type', spec, kind, ?key = key, aliases = alias :: aliases)

module Dap.React.Meta

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Util
open Dap.Platform.Meta

type M with
    static member reactViewService (aliases : ModuleAlias list, packRouteModelMsg : string, args : string, kind : Kind, key : Key) =
        let alias = "ReactViewTypes", "Dap.React.View.Types"
        let args = CodeArgs (sprintf "ReactViewTypes.Args<%s>" packRouteModelMsg) args
        let type' = sprintf "ReactViewTypes.View<%s>" packRouteModelMsg
        let spec = "Dap.React.View.Logic.spec"
        M.service (alias :: aliases, args, type', spec, kind, key)
    static member reactViewService (aliases : ModuleAlias list, packRouteModelMsg : string, args : string, key : Key) =
        M.reactViewService (aliases, packRouteModelMsg, args, Dap.React.View.Types.Kind, key)
    static member reactViewService (aliases : ModuleAlias list, packRouteModelMsg : string, args : string) =
        M.reactViewService (aliases, packRouteModelMsg, args, NoKey)


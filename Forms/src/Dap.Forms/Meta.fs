module Dap.Forms.Meta

open Microsoft.FSharp.Quotations

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Util
open Dap.Platform.Meta
open Dap.Forms.Provider

type M with
    static member preferencesService (aliases : ModuleAlias list, name : string, kind : Kind, key : Key) =
        let alias = "Preferences", "Dap.Forms.Provider.Preferences"
        let args = sprintf "Preferences.Args<%s>" name
        let args =
            sprintf "(Preferences.args %s.JsonEncoder %s.JsonDecoder)" name name
            |> CodeArgs args
        let type' = sprintf "Preferences.Service<%s>" name
        let spec = "Dap.Local.Storage.Base.Logic.spec"
        M.service (alias :: aliases, args, type', spec, kind, key)
    static member preferencesService (aliases : ModuleAlias list, name : string, key : Key) =
        M.preferencesService (aliases, name, Preferences.Kind, key)
    static member preferencesService (aliases : ModuleAlias list, name : string) =
        M.preferencesService (aliases, name, NoKey)
    static member preferencesService (aliases : ModuleAlias list, expr : Expr<'obj>, kind : Kind, key : Key) =
        let (name, _meta) = unquotePropertyGetExpr expr
        M.preferencesService (aliases, name, kind, key)
    static member preferencesService (aliases : ModuleAlias list, expr : Expr<'obj>, key : Key) =
        M.preferencesService (aliases, expr, Preferences.Kind, key)
    static member preferencesService (aliases : ModuleAlias list, expr : Expr<'obj>) =
        M.preferencesService (aliases, expr, NoKey)

type M with
    static member secureStorageService (aliases : ModuleAlias list, name : string, kind : Kind, key : Key) =
        let alias = "SecureStorage", "Dap.Forms.Provider.SecureStorage"
        let args = sprintf "SecureStorage.Args<%s>" name
        let args =
            sprintf "(SecureStorage.args %s.JsonEncoder %s.JsonDecoder)" name name
            |> CodeArgs args
        let type' = sprintf "SecureStorage.Service<%s>" name
        let spec = "Dap.Local.Storage.Base.Logic.spec"
        M.service (alias :: aliases, args, type', spec, kind, key)
    static member secureStorageService (aliases : ModuleAlias list, name : string, key : Key) =
        M.secureStorageService (aliases, name, "SecureStorage", key)
    static member secureStorageService (aliases : ModuleAlias list, name : string) =
        M.secureStorageService (aliases, name, NoKey)
    static member secureStorageService (aliases : ModuleAlias list, expr : Expr<'obj>, kind : Kind, key : Key) =
        let (name, _meta) = unquotePropertyGetExpr expr
        M.secureStorageService (aliases, name, kind, key)
    static member secureStorageService (aliases : ModuleAlias list, expr : Expr<'obj>, key : Key) =
        M.secureStorageService (aliases, expr, SecureStorage.Kind, key)
    static member secureStorageService (aliases : ModuleAlias list, expr : Expr<'obj>) =
        M.secureStorageService (aliases, expr, NoKey)

type M with
    static member formsViewService (aliases : ModuleAlias list, packModelMsg : string, args : string, kind : Kind, key : Key) =
        let alias = "FormsViewTypes", "Dap.Forms.View.Types"
        let args = CodeArgs (sprintf "FormsViewTypes.Args<%s>" packModelMsg) args
        let type' = sprintf "FormsViewTypes.View<%s>" packModelMsg
        let spec = "Dap.Forms.View.Logic.spec"
        M.service (alias :: aliases, args, type', spec, kind, key)
    static member formsViewService (aliases : ModuleAlias list, packModelMsg : string, args : string, key : Key) =
        M.formsViewService (aliases, packModelMsg, args, Dap.Forms.View.Types.Kind, key)
    static member formsViewService (aliases : ModuleAlias list, packModelMsg : string, args : string) =
        M.formsViewService (aliases, packModelMsg, args, NoKey)

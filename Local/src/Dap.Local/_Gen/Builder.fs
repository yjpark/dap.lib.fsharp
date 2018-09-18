module Dap.Local.Builder

open Dap.Context.Builder
open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <ValueBuilder>
 *)
type FileSystemArgsBuilder () =
    inherit ObjBuilder<FileSystemArgs> ()
    override __.Zero () = FileSystemArgs.Default ()
    [<CustomOperation("app_data")>]
    member __.AppData (target : FileSystemArgs, appData : string) =
        target.WithAppData appData
    [<CustomOperation("app_cache")>]
    member __.AppCache (target : FileSystemArgs, appCache : string) =
        target.WithAppCache appCache

let fileSystemArgs = FileSystemArgsBuilder ()
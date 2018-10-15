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
    member __.AppData (target : FileSystemArgs, (* FileSystemArgs *) appData : string) =
        target.WithAppData appData
    [<CustomOperation("app_cache")>]
    member __.AppCache (target : FileSystemArgs, (* FileSystemArgs *) appCache : string) =
        target.WithAppCache appCache

let file_system_args = FileSystemArgsBuilder ()
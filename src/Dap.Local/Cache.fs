[<RequireQualifiedAccess>]
module Dap.Local.Cache

open System.IO

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local
open Dap.Local.Feature

let getPath (relPath : string) : string =
    let environment = Environment.getInstance ()
    let cacheDirectory = environment.Properties.CacheDirectory.Value
    Path.Combine (cacheDirectory, relPath)

let has (relPath : string) : bool =
    FileSystem.fileExists (getPath relPath)

let ensure (relPath : string) (create : unit -> Bytes) : unit =
    let path = getPath relPath
    if not (FileSystem.fileExists path) then
        create ()
        |> BinaryFile.save path
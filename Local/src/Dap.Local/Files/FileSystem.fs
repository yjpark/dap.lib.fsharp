[<RequireQualifiedAccess>]
module Dap.Local.FileSystem

open System.IO
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Platform

let mutable private logger' = None
let getLogger () =
    if logger'.IsNone then
        logger' <- Some <| getLogger "[Local.FileSystem]"
    logger' |> Option.get

let checkDirectory (path : string) =
    let dirInfo = (new FileInfo (path)).Directory;
    if not dirInfo.Exists then
        dirInfo.Create();
        logInfo (getLogger ()) "checkDirectory" "Directory_Created" dirInfo

let fileExists (path : string) =
    File.Exists (path)

let tryCreateFromPath<'v> (path : string) (create : string -> 'v) =
    if fileExists path then
        try
            Some <| create path
        with e ->
            logException (getLogger ()) "tryCreateFromPath" "Exception_Raised" path e
            None
    else
        None

let tryCreateFromStream<'v> (path : string) (create : System.IO.Stream -> 'v) =
    if fileExists path then
        try
            use stream = new FileStream (path, FileMode.Open, FileAccess.Read)
            Some <| create stream
        with e ->
            logException (getLogger ()) "tryCreateFromStream" "Exception_Raised" path e
            None
    else
        None

let deleteFile (path : string) =
    if fileExists path then
        File.Delete path
        true
    else
        false

let folderExists (path : string) =
    Directory.Exists (path)

let deleteFolder (path : string) =
    if folderExists path then
        Directory.Delete path
        true
    else
        false
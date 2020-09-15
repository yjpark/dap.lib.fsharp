[<RequireQualifiedAccess>]
module Dap.Local.FileSystem

open System.IO
open System.Text
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
        logInfo (getLogger ()) "tryCreateFromPath" "File_Not_Exist" path
        None

let tryOpenStream (path : string) =
    if fileExists path then
        try
            let stream = new FileStream (path, FileMode.Open, FileAccess.Read)
            Some stream
        with e ->
            logException (getLogger ()) "tryOpenStream" "Exception_Raised" path e
            None
    else
        logInfo (getLogger ()) "tryOpenStream" "File_Not_Exist" path
        None

let tryCreateFromStream<'v> (path : string) (create : System.IO.Stream -> 'v) =
    tryOpenStream path
    |> Option.bind (fun stream ->
        try
            let result = Some <| create stream
            stream.Close ()
            result
        with e ->
            logException (getLogger ()) "tryCreateFromStream" "Exception_Raised" path e
            None
    )

let private decodeFromStream<'v> (decoder : JsonDecoder<'v>) (stream : Stream) =
    use reader = new StreamReader (stream, Encoding.UTF8)
    reader.ReadToEnd ()
    |> decodeJson decoder

let decodeMultiple<'v> (path : string) (decoder : JsonDecoder<'v>) : (string * 'v) array =
    let dirInfo = new DirectoryInfo (path);
    if dirInfo.Exists then
        dirInfo.GetFiles ()
        |> Array.choose (fun file ->
            tryCreateFromStream<'v> file.FullName <| decodeFromStream decoder
            |> Option.map (fun v ->
                file.Name, v)
        )
    else
        logInfo (getLogger ()) "decodeMultiple" "Directory_Not_Exist" dirInfo
        [| |]


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

let rec traverseFolder (callback : FileInfo -> unit) (dir : DirectoryInfo) =
    dir.GetFiles ()
    |> Array.iter ^<| callback
    dir.GetDirectories ()
    |> Array.iter ^<| traverseFolder callback

let traverse (callback : FileInfo -> unit) (path : string) =
    let root = new DirectoryInfo (path)
    if root.Exists then
        traverseFolder callback root
    else
        logError (getLogger ()) "traverse" "Directory_Not_Exist" path


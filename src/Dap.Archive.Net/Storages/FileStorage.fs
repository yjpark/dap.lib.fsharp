[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Archive.Storages.FileStorage

open System
open System.IO
open System.Threading.Tasks
open FSharp.Control.Tasks
open Dap.Prelude
open Dap.Platform
open Dap.Remote
open Dap.Archive

[<Literal>]
let MetaExtension = ".json"

[<Literal>]
let FramesExtension = ".bytes"

type Param = {
    Root : string
}

type Storage<'extra> when 'extra :> JsonRecord (param') =
    let param : Param = param'
    member _this.Param with get () = param
    interface IStorage<'extra> with
        member _this.OpenFramesStream (runner : IRunner) (relPath : string) =
            let path = Path.Combine (param.Root, relPath, FramesExtension)
            new FileStream (path, FileMode.Open, FileAccess.Read) :> Stream

type Param' = {
    Root : string
    CalcRelPath : string -> string
}

type Storage'<'extra> when 'extra :> JsonRecord (param') =
    let param : Param' = param'
    let checkDirectory (runner : IRunner) (path : string) =
        let dirInfo = (new FileInfo (path)).Directory;
        if not dirInfo.Exists then
            logInfo runner "FileStorage" "Create_Directory" dirInfo
            dirInfo.Create();
    member _this.Param with get () = param
    interface IStorage'<'extra> with
        member _this.WriteMetaAsync (meta : Meta<'extra>) =
            fun runner -> task {
                let relPath = param.CalcRelPath meta.Key
                let path = Path.Combine (param.Root, relPath)
                checkDirectory runner path
                use stream = new FileStream (path + MetaExtension, FileMode.CreateNew, FileAccess.Write)
                use writer = new StreamWriter (stream)
                let json = (meta :> JsonRecord).ToJsonString 4
                do! writer.WriteAsync (json)
            }
        member _this.NewFramesStream (runner : IRunner) (key : string) =
            let relPath = param.CalcRelPath key
            let path = Path.Combine (param.Root, relPath)
            checkDirectory runner path
            new FileStream (path + FramesExtension, FileMode.CreateNew, FileAccess.Write) :> Stream

let calRelPathWithPrefixes (prefixes : int list) (key : string) =
    let len = key.Length
    (prefixes
    |> List.filter (fun p -> p < len)
    |> List.map (fun p -> key.Substring (0, p))
    ) @ [ key ]
    |> List.toArray
    |> Path.Combine

let create'<'extra when 'extra :> JsonRecord> (prefixes : int list) (root : string) =
    let param : Param' =
        {
            Root = root
            CalcRelPath = calRelPathWithPrefixes prefixes
        }
    new Storage'<'extra> (param)
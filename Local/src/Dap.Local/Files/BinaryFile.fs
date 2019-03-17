[<RequireQualifiedAccess>]
module Dap.Local.BinaryFile

open System.IO
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local
open Dap.Local.Storage.Base.Types

let load (path : string) =
    FileSystem.tryCreateFromPath path (fun path ->
        File.ReadAllBytes path
    )

let save (path : string) (content : Bytes) =
    FileSystem.checkDirectory path
    File.WriteAllBytes (path, content)

type Provider (root : string) =
    interface IProvider<Bytes> with
        member __.LoadAsync (luid : Luid) = task {
            return load <| Path.Combine (root, luid)
        }
        member __.SaveAsync (param : SaveParam<Bytes>) = task {
            let path = Path.Combine (root, param.Luid)
            let exist = FileSystem.fileExists path
            if exist && not param.AllowOverwrite then
                failWith "SecureStorage" "Already_Exist"
            save path param.Value
            return not exist
        }


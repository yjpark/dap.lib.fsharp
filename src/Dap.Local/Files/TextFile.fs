[<RequireQualifiedAccess>]
module Dap.Local.TextFile

open System.IO

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local
open Dap.Local.Storage.Base.Types

let load (path : string) =
    FileSystem.tryCreateFromPath path (fun path ->
        File.ReadAllText path
    )

let save (path : string) (content : string) =
    FileSystem.checkDirectory path
    File.WriteAllText (path, content)

type Provider (root : string) =
    interface IProvider<string> with
        member __.LoadAsync (luid : Luid) = task {
            return load <| Path.Combine (root, luid)
        }
        member __.SaveAsync (param : SaveParam<string>) = task {
            let path = Path.Combine (root, param.Luid)
            let exist = FileSystem.fileExists path
            if exist && not param.AllowOverwrite then
                failWith "SecureStorage" "Already_Exist"
            save path param.Value
            return not exist
        }


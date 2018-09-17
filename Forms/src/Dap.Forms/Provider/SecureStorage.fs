[<RequireQualifiedAccess>]
module Dap.Forms.Provider.SecureStorage

open System.IO
open FSharp.Control.Tasks.V2
open Xamarin.Essentials

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local
open Dap.Local.Storage.Base.Types
module BaseTypes = Dap.Local.Storage.Base.Types
module JsonStorage = Dap.Local.Storage.Json.Service
module TextFile = Dap.Local.Provider.TextFile

open Dap.Forms

[<Literal>]
let Kind = "SecureStorage"

type Args<'v when 'v :> IJson> = BaseTypes.Args<string, 'v>
type Req<'v when 'v :> IJson> = BaseTypes.Req<'v>
type Evt<'v when 'v :> IJson> = BaseTypes.Evt<'v>
type Service<'v when 'v :> IJson> = BaseTypes.Agent<string, 'v>

[<Literal>]
let RootFolder = "secure_storage"

let root = Path.Combine (FileSystem.getAppDataFolder (), RootFolder)

let getPath (luid : Luid) =
    Path.Combine (root, luid + ".bytes")

let mutable private secret = "Iemohwai9iiY2phojael2och7quiex6Thohneothaek7eeghaebeewohghie9shu"

let setSecret secret' =
    secret <- secret'

let mutable private logger' = None
let getLogger () =
    if logger'.IsNone then
        logger' <- Some <| getLogger "[Forms.SecureStorage]"
    logger' |> Option.get

let encrypt (content : string) =
    Des.encrypt secret content

let decrypt (content : string) : string option =
    Des.decrypt (getLogger ()) secret content

let getAsync (luid : Luid) = task {
    if hasEssentials () then
        let! content = SecureStorage.GetAsync (luid)
        if System.String.IsNullOrEmpty (content) then
            return None
        else
            return Some content
    else
        return
            TextFile.load <| getPath luid
            |> Option.bind (fun content -> decrypt content)
}

let setAsync (luid : Luid) (v : string) = task {
    if hasEssentials () then
        do! SecureStorage.SetAsync (luid, v)
    else
        TextFile.save (getPath luid) (encrypt v)
}

let hasAsync (luid : Luid) = task {
    if hasEssentials () then
        let! content = getAsync luid
        return content.IsSome
    else
        return FileSystem.fileExists <| getPath luid
}

let remove (luid : Luid) : bool =
    if hasEssentials () then
        SecureStorage.Remove luid
    else
        FileSystem.deleteFile <| getPath luid

let removeAll () : unit =
    if hasEssentials () then
        SecureStorage.RemoveAll ()
    else
        FileSystem.deleteFolder root
        |> ignore

type Provider = Provider with
    interface IProvider<string> with
        member __.LoadAsync (luid : Luid) = getAsync luid
        member __.SaveAsync (param : SaveParam<string>) = task {
            let! exist = hasAsync param.Luid
            if exist && not param.AllowOverwrite then
                failWith "SecureStorage" "Already_Exist"
            do! setAsync param.Luid param.Value
            return not exist
        }

let args<'v when 'v :> IJson> encoder decoder = JsonStorage.args<'v> Provider 4 encoder decoder
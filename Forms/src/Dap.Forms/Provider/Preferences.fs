[<RequireQualifiedAccess>]
module Dap.Forms.Provider.Preferences

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
let Kind = "Preferences"

type Args<'v when 'v :> IJson> = BaseTypes.Args<string, 'v>
type Req<'v when 'v :> IJson> = BaseTypes.Req<'v>
type Evt<'v when 'v :> IJson> = BaseTypes.Evt<'v>
type Service<'v when 'v :> IJson> = BaseTypes.Agent<string, 'v>

[<Literal>]
let RootFolder = "preferences"

let root = Path.Combine (FileSystem.getAppDataFolder (), RootFolder)

let getPath (luid : Luid) =
    Path.Combine (root, luid + ".json")

let has (luid : Luid) =
    if hasEssentials () then
        Preferences.ContainsKey luid
    else
        FileSystem.fileExists <| getPath luid

let get (luid : Luid) =
    if hasEssentials () then
        Preferences.Get (luid, "")
    else
        TextFile.load <| getPath luid
        |> Option.defaultValue ""

let set (luid : Luid) (v : string) =
    if hasEssentials () then
        Preferences.Set (luid, v)
    else
        TextFile.save (getPath luid) v

let remove (luid : Luid) : unit =
    if hasEssentials () then
        Preferences.Remove luid
    else
        FileSystem.deleteFile <| getPath luid
        |> ignore

let clear () : unit =
    if hasEssentials () then
        Preferences.Clear ()
    else
        FileSystem.deleteFolder root
        |> ignore

type Provider = Provider with
    interface IProvider<string> with
        member __.LoadAsync (luid : Luid) = task {
            if has luid then
                return Some <| get luid
            else
                return None
        }
        member __.SaveAsync (param : SaveParam<string>) = task {
            let exist = has param.Luid
            if exist && not param.AllowOverwrite then
                failWith "Preferences" "Already_Exist"
            set param.Luid param.Value
            return not exist
        }

let args<'v when 'v :> IJson> encoder decoder = JsonStorage.args<'v> Provider 4 encoder decoder
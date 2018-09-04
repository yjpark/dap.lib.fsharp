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
module JsonStorage = Dap.Local.Storage.Json.Service
module TextFile = Dap.Local.Provider.TextFile

open Dap.Forms

[<Literal>]
let RootFolder = "preferences"

let mutable root' = None
let getRoot () =
    if root'.IsNone then
        root' <- Some <| Path.Combine (Dap.Local.App.Boot.getDocFolder (), RootFolder)
    root' |> Option.get

let getPath (luid : Luid) =
    Path.Combine (getRoot (), luid + ".json")

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
        FileSystem.deleteFolder <| getRoot ()
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

module Service =
    [<Literal>]
    let Kind = "Preferences"

    let addAsync'<'v when 'v :> IJson> kind key indent encoder decoder =
        JsonStorage.addAsync'<'v> kind key Provider indent encoder decoder

    let get'<'v when 'v :> IJson> kind key env =
        JsonStorage.get'<'v> kind key env

    let addAsync<'v when 'v :> IJson> key = addAsync'<'v> Kind key
    let get<'v when 'v :> IJson> key = get'<'v> Kind key
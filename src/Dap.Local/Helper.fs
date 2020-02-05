[<AutoOpen>]
module Dap.Local.Helper

open System.IO

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local.Feature

type IEnvironment with
    static member Instance = Environment.getInstance ()
    static member PrintVersion () : unit =
        noLogging (fun () ->
            printfn "%s" IEnvironment.Instance.Version.DevVer
        )
    member this.PreferencesProps = this.Preferences.Properties
    member this.SecureStorageProps = this.SecureStorage.Properties
    member __.TryGetVariable (key : string) : string option =
        let v = System.Environment.GetEnvironmentVariable key
        if v = null then
            None
        else
            Some v
    member this.TryLoadConfigFromData<'config>
            (decoder : JsonDecoder<'config>, name : string, ?folder : string)
            : Result<'config, string> =
        let folder = defaultArg folder "config"
        let path = Path.Combine (this.Properties.DataDirectory.Value, folder, name)
        path
        |> TextFile.load
        |> function
        | Some text ->
            match tryDecodeJson decoder text with
            | Ok param ->
                Ok param
            | Error err ->
                Error err
        | None ->
            Error <| sprintf "File_Not_Exist: %s" path
    member this.LoadConfigFromData<'config> (decoder : JsonDecoder<'config>, name : string, ?folder : string) : 'config =
        this.TryLoadConfigFromData<'config> (decoder, name, ?folder = folder)
        |> function
        | Ok config ->
            config
        | Error err ->
            err
            |> failWith <| sprintf "LoadConfigFromData<%A>" typeof<'config>

type FileSinkArgs with
    member this.WithTimestamp () =
        let timestamp = getNow' () |> instantToText
        let timestamp = timestamp.Replace (":", "_")
        System.IO.Path.Combine (this.Folder, timestamp, this.Filename)
        |> this.WithPath

type System.IO.Stream with
    member this.ReadAllBytes () : Bytes =
        let stream =
            match this with
            | :? System.IO.MemoryStream as stream ->
                stream
            | _ ->
                use stream = new System.IO.MemoryStream ()
                this.CopyTo stream
                stream
        stream.ToArray ()

let checkLocalEnvironment () =
    logWarn IEnvironment.Instance "Init" "Props" (encodeJson 4 IEnvironment.Instance.Properties)

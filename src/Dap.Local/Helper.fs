[<AutoOpen>]
module Dap.Local.Helper

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

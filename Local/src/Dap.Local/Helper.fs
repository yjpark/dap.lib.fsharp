[<AutoOpen>]
module Dap.Local.Helper

open Dap.Prelude
open Dap.Platform
open Dap.Local.Feature

type IEnvironment with
    static member Instance = Environment.getInstance ()
    member this.PreferencesProp = this.Preferences.Properties
    member this.SecureStorageProp = this.SecureStorage.Properties

type FileSinkArgs with
    member this.WithTimestamp () =
        let timestamp = getNow' () |> instantToText
        let timestamp = timestamp.Replace (":", "_")
        System.IO.Path.Combine (this.Folder, timestamp, this.Filename)
        |> this.WithPath

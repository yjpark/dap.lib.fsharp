[<AutoOpen>]
module Dap.Local.Helper

open Dap.Prelude
open Dap.Platform

type FileSinkArgs with
    member this.WithTimestamp () =
        let timestamp = getNow' () |> instantToText
        let timestamp = timestamp.Replace (":", "_")
        System.IO.Path.Combine (this.Folder, timestamp, this.Filename)
        |> this.WithPath

type IAppPack with
    member this.EnvironmentProps = this.Environment.Context.Properties
    member this.PreferencesProps = this.Preferences.Context.Properties
    member this.SecureStorageProps = this.SecureStorage.Context.Properties
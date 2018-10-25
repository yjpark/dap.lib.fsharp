[<AutoOpen>]
module Dap.Local.Helper

open Dap.Prelude
open Dap.Platform

type FileSinkArgs with
    static member LocalCreate (root : string, filename : string, ?minLevel : LogLevel, ?rolling : RollingInterval) =
        let timestamp = getNow' () |> instantToText
        let timestamp = timestamp.Replace (":", "_")
        let path = System.IO.Path.Combine (root, timestamp, filename)
        FileSinkArgs.Create (path, ?minLevel = minLevel, ?rolling = rolling)

type LoggingArgs with
    static member LocalCreate (?consoleLogLevel : LogLevel, ?filename : string, ?fileLogLevel : LogLevel, ?rolling : RollingInterval) =
        let consoleSink =
            consoleLogLevel
            |> Option.map (fun consoleLogLevel ->
                ConsoleSinkArgs.Create (minLevel = consoleLogLevel)
            )
        let fileSink =
            filename
            |> Option.map (fun filename ->
                FileSinkArgs.LocalCreate ("log", filename, ?minLevel = fileLogLevel, ?rolling = rolling)
            )
        LoggingArgs.Create (?console = consoleSink, ?file = fileSink)

type IAppPack with
    member this.PreferencesProps = this.Preferences.Context.Properties
    member this.SecureStorageProps = this.SecureStorage.Context.Properties
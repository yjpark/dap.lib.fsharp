[<AutoOpen>]
module Dap.Forms.Helper

open Xamarin.Forms

open Dap.Prelude
open Dap.Platform
open Dap.Local
open Dap.Forms.Provider

type FileSystemArgs with
    static member FormsDefault () =
        FileSystemArgs.Create (FileSystem.getAppDataFolder (), FileSystem.getCacheFolder ())

type ConsoleSinkArgs with
    static member FormsDefault () =
        if isRealForms () then
            let device = Device.RuntimePlatform
            not (device = Device.macOS || device = Device.iOS || device = Device.Android)
        else
            true
        |> function
            | true -> Some <| ConsoleSinkArgs.Default ()
            | false -> None

type LoggingArgs with
    static member FormsDefault (filename : string) =
        let consoleSink = ConsoleSinkArgs.FormsDefault ()
        let fileSink = FileSinkArgs.LocalDefault (FileSystemArgs.FormsDefault ()) filename
        LoggingArgs.Create (consoleSink, Some fileSink)
    static member FormsCreate (filename : string, consoleLogLevel : LogLevel) =
        LoggingArgs.FormsDefault filename
        |> fun l -> l.WithConsoleMinLevel consoleLogLevel
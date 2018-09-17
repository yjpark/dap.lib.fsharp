[<AutoOpen>]
module Dap.Local.Helper

open Dap.Prelude
open Dap.Platform

type FileSystemArgs with
    static member LocalDefault () =
        FileSystemArgs.Create "" ""


type FileSinkArgs with
    static member LocalDefault (args : FileSystemArgs) (filename : string) =
        let timestamp = getNow' () |> instantToText
        let timestamp = timestamp.Replace (":", "_")
        let path = System.IO.Path.Combine (args.AppCache, "log", timestamp, filename)
        FileSinkArgs.Create LogLevelInformation path (Some RollingInterval.Daily)

type LoggingArgs with
    static member LocalDefault (filename : string) =
        let consoleSink = ConsoleSinkArgs.Default ()
        let fileSink = FileSinkArgs.LocalDefault (FileSystemArgs.LocalDefault ()) filename
        LoggingArgs.Create (Some consoleSink) <| Some fileSink

let createLocalLogging logFile consoleLogLevel =
    LoggingArgs.LocalDefault logFile
    |> fun l -> l.WithConsoleMinLevel consoleLogLevel
    |> fun l -> l.CreateLogging ()
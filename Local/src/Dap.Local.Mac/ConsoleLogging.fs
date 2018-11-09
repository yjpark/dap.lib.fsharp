module Dap.Local.Mac.ConsoleLogging

open System
open System.IO

open Serilog.Core
open Serilog.Events
open Serilog.Formatting

open Dap.Prelude
open Dap.Context
open Dap.Platform

type ConsoleSink (textFormatter : ITextFormatter) =
    interface ILogEventSink with
        member __.Emit (evt : Serilog.Events.LogEvent) =
            let output = new StringWriter ()
            textFormatter.Format (evt, output)
            Console.WriteLine (output.ToString ())

type ConsoleSinkArgs with
    static member ConsoleProvider (this : ConsoleSinkArgs) : AddSink =
        fun config ->
            let formatter = new  Serilog.Formatting.Display.MessageTemplateTextFormatter (TextOutputTemplate, null)
            let sink = new ConsoleSink (formatter)
            config.WriteTo.Sink(sink, this.MinLevel.ToSerilogLevel)

type ConsoleLoggingProvider (logging : ILogging) =
    inherit BaseLoggingProvider (logging)
    override this.CreateLogging (args : LoggingArgs) =
        //TODO: get proper log folder
        let root = Path.Combine ("..", "log")
        let newArgs = args.WithFolder(root)
        let logging = newArgs.ToSerilogLogging (consoleProvider = ConsoleSinkArgs.ConsoleProvider)
        logInfo logging "ConsoleLoggingProvider" "CreateLogging" (encodeJson 4 newArgs)
        if newArgs.File.IsSome then
            logInfo logging "ConsoleLoggingProvider" "Folder_Updated" (sprintf "%s -> %s", args.File.Value.Folder, newArgs.File.Value.Folder)
        logging :> ILogging

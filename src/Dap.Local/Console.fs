[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Console

open System
open System.Threading
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Argu

open Dap.Prelude
open Dap.Platform

exception ParseException of string
exception ExecuteException of string

[<Literal>]
let ReturnCode_OtherError = 500

[<Literal>]
let ReturnCode_ParseFailed = 501

[<Literal>]
let ReturnCode_ExecuteFailed = 502

let onExit (cleanup : unit -> unit) (env : IEnv) (exited : AutoResetEvent) =
    fun (_sender : obj) (cancelArgs : ConsoleCancelEventArgs) ->
        logWarn env "Console" "Quitting ..." cancelArgs
        cleanup ()
        env.Logging.Close ()
        exited.Set() |> ignore

let waitForExit' (cleanup : unit -> unit) (env : IEnv) =
    let exited = new AutoResetEvent(false)
    let onExit' = new ConsoleCancelEventHandler (onExit cleanup env exited)
    Console.CancelKeyPress.AddHandler onExit'
    exited.WaitOne() |> ignore

let waitForExit (env : IEnv) =
    waitForExit' ignore env

let executeAndWaitForExit (env : IEnv) (task : Task<unit>) =
    try
        Async.AwaitTask task
        |> Async.RunSynchronously
        waitForExit env
    with e ->
        logError env "Execute" "Exception_Raised" (task.ToString (), e)

let newLoggingArgs (logFile : string) (verbose : bool) =
    let consoleMinLevel = if verbose then LogLevelInformation else LogLevelWarning
    LoggingArgs.CreateBoth (logFile, consoleMinLevel = consoleMinLevel)

let initLogging (logFile : string) (verbose : bool) =
    newLoggingArgs logFile verbose
    |> Feature.createLogging

let main<'args when 'args :> IArgParserTemplate> (program : string) (execute : ParseResults<'args> -> int) argv =
    let parser = ArgumentParser.Create<'args> (programName = program)
    try
        execute <| parser.ParseCommandLine argv
    with
    | ParseException m ->
        let usage = parser.PrintUsage()
        printfn "ERROR: %s\n%s" m usage
        ReturnCode_ParseFailed
    | ExecuteException m as e ->
        printfn "ERROR: %s\n%s" m e.StackTrace
        ReturnCode_ExecuteFailed
    | e ->
        printfn "%s" e.Message
        ReturnCode_OtherError

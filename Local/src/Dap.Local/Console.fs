[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Console

open System
open System.Threading
open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Platform

let onExit (env : IEnv) (exited : AutoResetEvent) =
    fun (_sender : obj) (cancelArgs : ConsoleCancelEventArgs) ->
        logWarn env "Console" "Quiting ..." cancelArgs
        env.Logging.Close ()
        exited.Set() |> ignore

let waitForExit (env : IEnv) =
    let exited = new AutoResetEvent(false)
    let onExit' = new ConsoleCancelEventHandler (onExit env exited)
    Console.CancelKeyPress.AddHandler onExit'
    exited.WaitOne() |> ignore

let executeAndWaitForExit (env : IEnv) (task : Task<unit>) =
    try
        Async.AwaitTask task
        |> Async.RunSynchronously
        waitForExit env
    with e ->
        logError env "Execute" "Exception_Raised" (task.ToString (), e)

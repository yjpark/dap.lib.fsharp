[<AutoOpen>]
module Dap.Forms.Runner

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Xamarin.Forms

open Dap.Prelude
open Dap.Platform

[<Literal>]
let DefaultTimeout = 0.1<second>

type IRunner<'runner when 'runner :> IRunner> with
    member this.RunUiFunc (func : Func<'runner, unit>) : unit =
        Device.BeginInvokeOnMainThread (fun () ->
            runFunc' this.Runner func
            |> ignore
        )
    member this.GetUiTask' (timeout : float<second>)
                            (onFailed : OnFailed<'runner>)
                            (getTask : GetTask<'runner, unit>)
                            : GetTask<'runner, unit> =
        fun runner -> task {
            Device.BeginInvokeOnMainThread (fun () ->
                try
                    let task = getTask runner
                    let timeout' = System.TimeSpan.FromMilliseconds (1000.0 * float timeout)
                    if (task.Wait (timeout')) then
                        failWith "RunUiTask" "Timeout" (task, timeout)
                with e ->
                    onFailed runner e
            )
        }
    member this.GetUiTask (onFailed : OnFailed<'runner>)
                            (getTask : GetTask<'runner, unit>)
                            : GetTask<'runner, unit> =
        this.GetUiTask' DefaultTimeout onFailed getTask
    member this.AddUiTask' : float<second> -> OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun timeout onFailed getTask ->
            this.AddTask onFailed <| this.GetUiTask' timeout onFailed getTask
    member this.AddUiTask : OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun onFailed getTask ->
            this.AddTask onFailed <| this.GetUiTask onFailed getTask
    member this.RunUiTask' : float<second> -> OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun timeout onFailed getTask ->
            this.RunTask onFailed <| this.GetUiTask' timeout onFailed getTask
    member this.RunUiTask : OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun onFailed getTask ->
            this.RunTask onFailed <| this.GetUiTask onFailed getTask
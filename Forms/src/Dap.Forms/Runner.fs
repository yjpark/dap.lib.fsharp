[<AutoOpen>]
module Dap.Forms.Runner

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Xamarin.Forms

open Dap.Prelude
open Dap.Platform

type IRunner<'runner when 'runner :> IRunner> with
    member this.RunUiFunc (func : Func<'runner, unit>) : unit =
        Device.BeginInvokeOnMainThread (fun () ->
            runFunc' this.Runner func
            |> ignore
        )
    member this.GetUiTask (onFailed : OnFailed<'runner>)
                            (getTask : GetTask<'runner, unit>)
                            : GetTask<'runner, unit> =
        fun runner -> task {
            Device.BeginInvokeOnMainThread (fun () ->
                try
                    let task = getTask runner
                    task.Wait ()
                with e ->
                    onFailed runner e
            )
        }
    member this.AddUiTask : OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun onFailed getTask ->
            this.AddTask onFailed <| this.GetUiTask onFailed getTask
    member this.RunUiTask : OnFailed<'runner> -> GetTask<'runner, unit> -> unit =
        fun onFailed getTask ->
            this.RunTask onFailed <| this.GetUiTask onFailed getTask
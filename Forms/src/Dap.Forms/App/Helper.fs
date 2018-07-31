[<RequireQualifiedAccess>]
module Dap.Forms.App.Helper

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Xamarin.Forms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Archive
open Dap.Forms

open Dap.Forms.App.Types
module Logic = Dap.Forms.App.Logic

let env (scope : Scope) (consoleLogLevel) (logFile : string) =
    let timestamp = Profile.perMinute.CalcVolumeKey <| getNow' ()
    let ident = timestamp.Replace(":", "_")
    let filePath = sprintf "%s/%s/%s" Const.LogFolder ident logFile
    let logging =
        let isReal = isRealForms ()
        if isReal && Device.RuntimePlatform = Device.UWP then
            setupSerilog
                [
                    addConsoleSink <| Some consoleLogLevel
                    addDailyFileSink filePath
                ]
        elif isReal && (Device.RuntimePlatform = Device.macOS || Device.RuntimePlatform = Device.iOS) then
            setupSerilog
                [
                    addDailyFileSink filePath
                ]
        else
            setupSerilog
                [
                    addConsoleSink <| Some consoleLogLevel
                    addDailyFileSink filePath
                    //addSeqSink "http://localhost:5341"
                ]
    Env.live MailboxPlatform logging scope

let initAsync<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg>
            (spawn : Spawner<'runner>)
            (args : Args<'runner, 'model, 'msg>)
            (env : IEnv) : Task<'runner> = task {
    do! args.SetupAsync env
    let spec = Logic.spec spawn args
    let! app = env |> Env.addServiceAsync spec "App" noKey
    do! app.PostAsync DoRun
    return app
}

let init<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg>
            (spawn : Spawner<'runner>)
            (args : Args<'runner, 'model, 'msg>)
            (callback : 'runner -> unit)
            (env : IEnv) =
    env.RunTask0 ignoreOnFailed (fun runner -> task {
        let! app = initAsync<'runner, 'model, 'msg> spawn args env
        callback app
        return ()
    })

let newApplication (env : IEnv) =
    if isRealForms () then
        let application = new Application ()
        let emptyPage = View.ContentPage (content = View.Label (text = "TEST"))
        let page = emptyPage.Create ()
        application.MainPage <- page :?> Page
        Some application
    else
        None
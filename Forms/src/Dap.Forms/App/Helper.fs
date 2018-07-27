[<RequireQualifiedAccess>]
module Dap.Forms.App.Helper

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Xamarin.Forms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Archive

open Dap.Forms.App.Types
module Logic = Dap.Forms.App.Logic

let createAsync'<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg>
            (spawn : Spawner<'runner>)
            (args : Args<'runner, 'model, 'msg>)
            (env : IEnv) : Task<'runner> = task {
    let spec = Logic.spec spawn args
    let! app = env |> Env.addServiceAsync spec "App" noKey
    return app
}

let createAsync<'runner, 'model, 'msg when 'runner :> App<'runner, 'model, 'msg>
                and 'model : not struct and 'msg :> IMsg>
            (spawn : Spawner<'runner>)
            (args : Args<'runner, 'model, 'msg>)
            (scope : Scope) (consoleLogLevel) (logFile : string) = task {
    let timestamp = Profile.perMinute.CalcVolumeKey <| getNow' ()
    let ident = timestamp.Replace(":", "_")
    let logging =
        if Device.RuntimePlatform = Device.UWP then
            setupSerilog
                [
                    addConsoleSink <| Some consoleLogLevel
                ]
        else
            setupSerilog
                [
                    addConsoleSink <| Some consoleLogLevel
                    addDailyFileSink <| sprintf "log/%s/%s" ident logFile
                    //addSeqSink "http://localhost:5341"
                ]
    let env = Env.live MailboxPlatform logging scope
    do! args.SetupAsync env
    let! app = env |> createAsync'<'runner, 'model, 'msg> spawn args
    return app
}

let newApplication () =
    let application = new Application ()
    let emptyPage = View.ContentPage (content = View.Label (text = ""))
    let page = emptyPage.Create ()
    application.MainPage <- page :?> Page
    application
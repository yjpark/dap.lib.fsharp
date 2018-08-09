[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Forms.App.WithView

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Xamarin.Forms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform
open Dap.Local.App

open Dap.Forms
module ViewTypes = Dap.Forms.View.Types

[<Literal>]
let Kind = "Forms"

[<Literal>]
let UWP_LogFolderTip = """
For UWP, need to change the log folder before init:
    var cacheFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
    Dap.Local.App.Boot.setLogFolder(cacheFolder + "/log");
"""

type Args<'model, 'msg, 'parts
            when 'model : not struct and 'msg :> IMsg> = {
    Base : Simple.Args
    NewPartsAsync : Simple.Model -> Task<'parts>
    NewViewAsync : Simple.Model -> 'parts -> Task<ViewTypes.View<'model, 'msg>>
} with
    static member Create (base' : Simple.Args) newPartsAsync newViewAsync =
        {
            Base = base'
            NewPartsAsync = newPartsAsync
            NewViewAsync = newViewAsync
        }

and Model<'model, 'msg, 'parts
            when 'model : not struct and 'msg :> IMsg> = {
    Args : Args<'model, 'msg, 'parts>
    Base : Simple.Model
    Parts : 'parts
    View : ViewTypes.View<'model, 'msg>
} with
    static member Create args (base' : Simple.Model) parts view =
        {
            Args = args
            Base = base'
            Parts = parts
            View = view
        }
    member this.Boot = this.Base.Boot
    member this.Env = this.Base.Env

let private fixBootLogging () =
    if isRealForms () then
        let device = Device.RuntimePlatform
        if (device = Device.macOS || device = Device.iOS) then
            Boot.setLogConsole false
        elif device = Device.UWP then
            if Boot.getLogFolder () = Boot.DefaultLogFolder then
                failWith "Invalid_LogFolder" UWP_LogFolderTip

(*
    Have to run the async logic in RunTask, otherwise might block
    if calling form UI thread
 *)
let init<'model, 'msg, 'parts
            when 'model : not struct and 'msg :> IMsg>
        (args : Args<'model, 'msg, 'parts>)
        (onApp : Model<'model, 'msg, 'parts> -> unit) : Simple.Model =
    fixBootLogging ()
    let app = Simple.init args.Base
    app.Env.RunTask0 ignoreOnFailed (fun runner -> task {
        let! parts = args.NewPartsAsync app
        let! view = args.NewViewAsync app parts
        let app = Model<'model, 'msg, 'parts>.Create args app parts view
        let onRun = fun () -> onApp app
        view.Post <| ViewTypes.DoRun ^<| callback app.Env onRun
    })
    app
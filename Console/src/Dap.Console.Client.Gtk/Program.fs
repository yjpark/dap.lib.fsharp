// Learn more about F# at http://fsharp.org

open System
open Eto.Forms
open Eto.Drawing
open System.Runtime.CompilerServices

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Eto.Prefab
open Dap.Console.Client.Prefab

let logging = setupConsole LogLevelWarning
let env = Env.live MailboxPlatform logging "Console"

type TestForm () =
    inherit Form ()
    do (
        base.Title <- "TEST"
        base.ClientSize <- Size (800, 600)
        (*
        let label = new Label.Prefab (env :> IOwner, "Test")
        base.Content <- label.Widget //new Label (Text = "TODO")
        label.Text.SetValue "AAAAAAAAAAAAAAAAAAAAAA"
        *)
        let prefab = new LoginForm.Prefab (env :> IOwner, "Test")
        logWarn env "JSON" "NEW" <| E.encodeJson 4 prefab
        base.Content <- prefab.Widget
        prefab.Password.Value.Text.OnValueChanged.AddWatcher env "Test" (fun evt ->
            logWarn env "Password" "Changed" (evt.Old, evt.New)
        )
        prefab.Password.Value.Text.SetValue "KK"
        //prefab.Label.Text.SetValue "Hello World"
    )

[<EntryPoint>]
[<STAThread>]
let main argv =
    printfn "Hello World from F#!"
    (new Application ()) .Run (new TestForm ())
    0 // return an integer exit code

module Dap.Console.Client.Program

open System
open Eto.Forms
open Eto.Drawing
open System.Runtime.CompilerServices

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Eto.Prefab
open Dap.Console.Client.Helper

let logging = setupConsole LogLevelWarning
let env = Env.live MailboxPlatform logging "Console"

[<EntryPoint>]
[<STAThread>]
let main argv =
    (new Application ()) .Run (new TestForm (env))
    0 // return an integer exit code

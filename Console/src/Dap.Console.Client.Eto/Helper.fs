module Dap.Console.Client.Helper

open System
open Eto.Forms
open Eto.Drawing
open System.Runtime.CompilerServices

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Eto.Prefab
open Dap.Console.Client.Prefab

type TestForm (env : IEnv) =
    inherit Form ()
    do (
        base.Title <- "TEST"
        base.ClientSize <- Size (800, 600)
        let prefab = new LoginForm.Prefab (env.Logging)
        logWarn env "JSON" "NEW" <| E.encodeJson 4 prefab
        base.Content <- prefab.Widget
        prefab.Password.Value.Model.Text.OnChanged.AddWatcher env "Test" (fun evt ->
            logWarn env "Password" "Changed" (evt.Old, evt.New)
        )
        prefab.Login.OnClick.OnEvent.AddWatcher env "Test" (fun evt ->
            logWarn env "Login" "OnClick" evt
        )
        prefab.Password.Value.Model.Text.SetValue "KK"
        prefab.Title.Model.Text.SetValue "Hello World"
    )

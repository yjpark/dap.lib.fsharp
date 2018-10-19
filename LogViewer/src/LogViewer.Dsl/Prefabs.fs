module LogViewer.Dsl.Prefabs

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder
open Dap.Context.Generator
open Dap.Platform
open Dap.Gui
open Dap.Gui.Builder
open Dap.Gui.Dsl.Prefab

let LoginForm =
    v_stack {
        prefab "login"
        styles ["style1" ; "style2"]
        child "title" (
            label {
                text "Login with your credential"
            }
        )
        child "name" (inputField "User Name")
        child "password" (inputField "Password")
        child "login" (
            button {
                text "Login"
            }
        )
    }

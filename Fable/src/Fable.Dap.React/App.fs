[<RequireQualifiedAccess>]
module Dap.App.React.App

open Fable.Core

open Dap.Prelude
open Dap.Platform
open Dap.App.React.Types
module Logic = Dap.App.React.Logic

let [<PassGenericsAttribute>] create<'runner, 'route, 'model, 'msg when 'runner :> App<'runner, 'route, 'model, 'msg>
                and 'route :> IRoute
                and 'model : not struct and 'msg :> IMsg>
            (scope : Scope) (spawn : Spawner<'runner>)
            (args : Args<'runner, 'route, 'model, 'msg>)
            (minimumLevel : LogLevel option)
            : 'runner =
    let logging = setupConsole minimumLevel
    let env = Env.live logging scope
    let spec = Logic.spec spawn args
    let app = env |> Env.spawn spec "App" "React" :?> 'runner
    app
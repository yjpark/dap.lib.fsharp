[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.App.Simple

open Dap.Prelude
open Dap.Platform

let newEnv (scope : Scope) (boot : Boot.Model) : IEnv =
#if FABLE_COMPILER
    Env.live boot.Logging scope
#else
    Env.live MailboxPlatform boot.Logging scope
#endif

type Args = {
    Boot : Boot.Args
    NewEnv : Boot.Model -> IEnv
} with
    static member Create boot newEnv =
        {
            Boot = boot
            NewEnv = newEnv
        }
#if FABLE_COMPILER
    static member Default scope consoleLogLevel =
        newEnv scope
        |> Args.Create ^<| Boot.Args.Default ^<| Boot.newLogging consoleLogLevel
#else
    static member Default scope consoleLogLevel logFile =
        newEnv scope
        |> Args.Create ^<| Boot.Args.Default ^<| Boot.newLogging consoleLogLevel logFile
#endif

and Model = {
    Args : Args
    Boot : Boot.Model
    Env : IEnv
}

let mutable private instance : Model option = None
let getInstance () = instance |> Option.get

let init (args : Args) : Model =
    if instance.IsSome then
        failWith "App.Simple" "Already_Init"
    let boot = Boot.init args.Boot
    let env = args.NewEnv boot
    instance <- Some {
        Args = args
        Boot = boot
        Env = env
    }
    getInstance ()
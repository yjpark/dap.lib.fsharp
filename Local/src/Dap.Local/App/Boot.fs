[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.App.Boot

open Dap.Prelude
open Dap.Platform
open Dap.Archive

[<Literal>]
let DefaultLogFolder = "log"

let DefaultLogConsole = true

let calcIdent (time : Instant) =
    let timestamp = Profile.perMinute.CalcVolumeKey <| time
    timestamp.Replace(":", "_")

let mutable private logConsole = DefaultLogConsole
let getLogConsole () = logConsole

let setLogConsole v =
    logConsole <- v

let mutable private logFolder = DefaultLogFolder

let getLogFolder () = logFolder

let setLogFolder v =
    logFolder <- v

let newLogging (consoleLogLevel : LogLevel) (logFile : string) (ident : string) =
    let filePath = sprintf "%s/%s/%s" logFolder ident logFile
    setupSerilog (
        [
            addDailyFileSink filePath
            //addSeqSink "http://localhost:5341"
        ] |> List.append (
            if logConsole then
                [ addConsoleSink consoleLogLevel ]
            else
                []
        )
    )
    :> ILogging

type Args = {
    CalcIdent : Instant -> string
    NewLogging : string -> ILogging
} with
    static member Create calcIdent newLogging =
        {
            CalcIdent = calcIdent
            NewLogging = newLogging
        }
    static member Default newLogging =
        Args.Create calcIdent newLogging

type Model = {
    Time : Instant
    Ident : string
    Logging : ILogging
}

let init (args : Args) : Model =
    let time = getNow' ()
    let ident = args.CalcIdent time
    let logging = args.NewLogging ident
    {
        Time = time
        Ident = ident
        Logging = logging
    }
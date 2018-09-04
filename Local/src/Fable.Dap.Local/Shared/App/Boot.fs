[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.App.Boot

open Dap.Prelude
open Dap.Platform

#if !FABLE_COMPILER
[<Literal>]
let DefaultLogFolder = "log"

[<Literal>]
let DefaultDocFolder = "doc"

[<Literal>]
let DefaultLogToConsole = true

[<Literal>]
let DefaultLogToFile = true

let mutable private logToConsole = DefaultLogToConsole
let getLogToConsole () = logToConsole
let setLogToConsole v =
    logToConsole <- v

let mutable private logToFile = DefaultLogToFile
let getLogToFile () = logToFile
let setLogToFile v =
    logToFile <- v

let mutable private logFolder = DefaultLogFolder

let mutable private docFolder = DefaultDocFolder

let getLogFolder () = logFolder

let getDocFolder () = docFolder

let setLogFolder v =
    logFolder <- v

let setDocFolder v =
    docFolder <- v
#endif

let calcIdent (time : Instant) =
#if !FABLE_COMPILER
    let time = time |> toDateTimeUtc
#endif
    let timestamp = time |> dateTimeToText
    timestamp.Replace(":", "_")

#if FABLE_COMPILER
let newLogging (consoleLogLevel : LogLevel) (ident : string) =
    setupConsole consoleLogLevel
    :> ILogging
#else
let newLogging (consoleLogLevel : LogLevel) (logFile : string) (ident : string) =
    let consoleSink =
        if logToConsole then
            [ addConsoleSink consoleLogLevel ]
        else
            []
    let fileSink =
        if logToFile then
            let filePath = sprintf "%s/%s/%s" logFolder ident logFile
            [ addDailyFileSink filePath ]
        else
            []
    setupSerilog <| consoleSink @ fileSink
    :> ILogging
#endif

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
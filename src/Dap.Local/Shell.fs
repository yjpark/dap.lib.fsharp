[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Shell

open System.Diagnostics

// https://stackoverflow.com/questions/44205260/net-core-copy-to-clipboard
// https://loune.net/2017/06/running-shell-bash-commands-in-net-core/

let exec (filename : string) (arguments : string) =
    let startInfo =
        new ProcessStartInfo (
            FileName = filename,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true
        )
    let proc = new Process (StartInfo = startInfo)
    if proc.Start () then
        let result = proc.StandardOutput.ReadToEnd ()
        proc.WaitForExit ()
        result
    else
        ""

let escape (param : string) =
    param.Replace("\"", "\\\"")

let private run (filename : string) (prefix : string) (cmd : string) =
    let args = cmd |> escape
    exec filename (prefix + args)

let bash (cmd : string) =
    run "/bin/bash" "-c " cmd

let bat (cmd : string) =
    run "cmd.exe" "/c " cmd
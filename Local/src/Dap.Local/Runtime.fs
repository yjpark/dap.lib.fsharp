[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Runtime

open System.Runtime.InteropServices

type Platform =
    | Mac
    | Linux
    | Windows

type Runtime = Runtime
with
    static member Platform : Platform =
        if RuntimeInformation.IsOSPlatform (OSPlatform.OSX) then
            Mac
        elif RuntimeInformation.IsOSPlatform (OSPlatform.Linux) then
            Linux
        elif RuntimeInformation.IsOSPlatform (OSPlatform.Windows) then
            Windows
        else
            failwith "Unknown_Platform"
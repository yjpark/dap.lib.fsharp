#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Dap.Console.Client/Dsl/Prefabs.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let projects =
    !! "src/Dap.Console.Server/*.fsproj"
    ++ "src/Dap.Console.Client/*.fsproj"
    ++ "src/Dap.Console.Client.Gtk/*.fsproj"

DotNet.create DotNet.release projects


DotNet.createPrepares [
    ["Dap.Console.Client"], fun _ ->
        Dap.Console.Client.Dsl.Prefabs.compile ["src" ; "Dap.Console.Client"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

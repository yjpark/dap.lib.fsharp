#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Dap.Console.Client/Dsl/Prefabs.fs"
#load "src/Dap.Console.Client.Forms/Dsl.fs"
#load "src/Dap.Console.Client.Eto/Dsl.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let projects =
    !! "src/Dap.Console.Server/*.fsproj"
    ++ "src/Dap.Console.Client/*.fsproj"
    ++ "src/Dap.Console.Client.Forms/*.fsproj"
    ++ "src/Dap.Console.Client.Eto/*.fsproj"
    ++ "src/Dap.Console.Client.Gtk/*.fsproj"

DotNet.create DotNet.release projects


DotNet.createPrepares [
    ["Dap.Console.Client.Forms"], fun _ ->
        Dap.Console.Client.Forms.Dsl.compile ["src" ; "Dap.Console.Client.Forms"]
        |> List.iter traceSuccess
    ["Dap.Console.Client.Eto"], fun _ ->
        Dap.Console.Client.Eto.Dsl.compile ["src" ; "Dap.Console.Client.Eto"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/LogViewer.Dsl/Prefabs.fs"
#load "src/LogViewer.Eto/Dsl/Prefabs.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let projects =
    !! "src/LogViewer.Dsl/*.fsproj"
    ++ "src/LogViewer.Eto/*.fsproj"
    ++ "src/LogViewer.Gtk/*.fsproj"

DotNet.create DotNet.release projects


DotNet.createPrepares [
    ["LogViewer.Eto"], fun _ ->
        LogViewer.Eto.Dsl.Prefabs.compile ["src" ; "LogViewer.Eto"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

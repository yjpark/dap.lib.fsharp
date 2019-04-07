(* FAKE: 5.12.1 *)
#r "paket: groupref Main //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Dap.Remote.Dashboard/Dsl/Args.fs"
#load "src/Dap.Remote.Dashboard/_Gen/Args.fs"
#load "src/Dap.Remote.Dashboard/Meta.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Types.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Operator.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Pack.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let feed =
    NuGet.Feed.Create (
        apiKey = NuGet.Environment "API_KEY_nuget_org"
    )

let projects =
    !! "src/Dap.Remote.FSharpData/*.fsproj"
    ++ "src/Dap.Remote.Dashboard/*.fsproj"
    ++ "src/Dap.Remote.Server/*.fsproj"
    ++ "src/Dap.Remote.Web/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Dap.Remote.Dashboard"], fun _ ->
        Dap.Remote.Dashboard.Dsl.Args.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Types.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Operator.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Pack.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build


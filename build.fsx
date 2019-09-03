(* FAKE: 5.13.3 *)
#r "paket: groupref Main //"
#load ".fake/build.fsx/intellisense.fsx"

#load "src/Dap.Local/Meta.fs"
#load "src/Dap.Local/Dsl.fs"
#load "src/Dap.Local.Farango/Dsl.fs"

#load "src/Dap.Remote.Dashboard/Dsl/Args.fs"
#load "src/Dap.Remote.Dashboard/_Gen/Args.fs"
#load "src/Dap.Remote.Dashboard/Meta.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Types.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Operator.fs"
#load "src/Dap.Remote.Dashboard/Dsl/Pack.fs"
#load "src/Dap.Remote.Web/Dsl.fs"
#load "src/Dap.Remote.Aws/Dsl.fs"
#load "src/Dap.Remote.Squidex/Dsl.fs"
#load "src/Dap.Remote.Squidex.Sync/Dsl.fs"

#load "src/Dap.Fable/Dsl.fs"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators

open Dap.Build

[<Literal>]
let Prepare = "Prepare"

let feed =
    NuGet.Feed.Create (
        apiKey = NuGet.Environment "API_KEY_nuget_org"
    )

let projects =
    !! "src/Dap.Local/*.fsproj"
    ++ "src/Dap.Local.Farango/*.fsproj"
    ++ "src/Dap.Remote.Dashboard/*.fsproj"
    ++ "src/Dap.Remote.FSharpData/*.fsproj"
    ++ "src/Dap.Remote.Server/*.fsproj"
    ++ "src/Dap.Remote.Web/*.fsproj"
    ++ "src/Dap.Remote.Aws/*.fsproj"
    ++ "src/Dap.Remote.Squidex/*.fsproj"
    ++ "src/Dap.Remote.Squidex.Sync/*.fsproj"
    ++ "src/Dap.Fable/*.fsproj"
    ++ "src/Fable.Dap.Local/*.fsproj"
    ++ "src/Fable.Dap.React/*.fsproj"
    ++ "src/Fable.Dap.Fulma/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Dap.Local"], fun _ ->
        Dap.Local.Dsl.compile ["src" ; "Dap.Local"]
        |> List.iter traceSuccess
    ["Dap.Local.Farango"], fun _ ->
        Dap.Local.Farango.Dsl.compile ["src" ; "Dap.Local.Farango"]
        |> List.iter traceSuccess
    ["Dap.Remote.Dashboard"], fun _ ->
        Dap.Remote.Dashboard.Dsl.Args.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Types.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Operator.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
        Dap.Remote.Dashboard.Dsl.Pack.compile ["src" ; "Dap.Remote.Dashboard"]
        |> List.iter traceSuccess
    ["Dap.Remote.Web"], fun _ ->
        Dap.Remote.Web.Dsl.compile ["src" ; "Dap.Remote.Web"]
        |> List.iter traceSuccess
    ["Dap.Remote.Aws"], fun _ ->
        Dap.Remote.Aws.Dsl.compile ["src" ; "Dap.Remote.Aws"]
        |> List.iter traceSuccess
    ["Dap.Remote.Squidex"], fun _ ->
        Dap.Remote.Squidex.Dsl.compile ["src" ; "Dap.Remote.Squidex"]
        |> List.iter traceSuccess
    ["Dap.Remote.Squidex.Sync"], fun _ ->
        Dap.Remote.Squidex.Sync.Dsl.compile ["src" ; "Dap.Remote.Squidex.Sync"]
        |> List.iter traceSuccess
    ["Dap.Fable"], fun _ ->
        Dap.Fable.Dsl.compile ["src" ; "Fable.Dap.Local"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

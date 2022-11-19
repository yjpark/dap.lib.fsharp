open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators

open Dap.Build

let feed =
    NuGet.Feed.Create (
        apiKey = NuGet.Environment "API_KEY_nuget_org"
    )

let projects =
    !! "../src/Dap.Local/*.fsproj"
    ++ "../src/Dap.Remote.Dashboard/*.fsproj"
    ++ "../src/Dap.Remote.FSharpData/*.fsproj"
    ++ "../src/Dap.Remote.Server/*.fsproj"
    ++ "../src/Dap.Remote.Web/*.fsproj"
    ++ "../src/Dap.Remote.Aws/*.fsproj"
    ++ "../src/Dap.Remote.Squidex/*.fsproj"
    ++ "../src/Dap.Remote.Squidex.Sync/*.fsproj"
    ++ "../src/Dap.Fable/*.fsproj"
    ++ "../src/Fable.Dap.Local/*.fsproj"
    //++ "../src/Fable.Dap.React/*.fsproj"
    //++ "../src/Fable.Dap.Fulma/*.fsproj"

let createTargets () =
    NuGet.create NuGet.release feed projects

    DotNet.createPrepares [
        ["Dap.Local"], fun _ ->
            Dap.Local.Dsl.compile ["../src" ; "Dap.Local"]
            |> List.iter traceSuccess
        ["Dap.Remote.Dashboard"], fun _ ->
            Dap.Remote.Dashboard.Dsl.Args.compile ["../src" ; "Dap.Remote.Dashboard"]
            |> List.iter traceSuccess
            Dap.Remote.Dashboard.Dsl.Types.compile ["../src" ; "Dap.Remote.Dashboard"]
            |> List.iter traceSuccess
            Dap.Remote.Dashboard.Dsl.Operator.compile ["../src" ; "Dap.Remote.Dashboard"]
            |> List.iter traceSuccess
            Dap.Remote.Dashboard.Dsl.Pack.compile ["../src" ; "Dap.Remote.Dashboard"]
            |> List.iter traceSuccess
        ["Dap.Remote.Web"], fun _ ->
            Dap.Remote.Web.Dsl.compile ["../src" ; "Dap.Remote.Web"]
            |> List.iter traceSuccess
        ["Dap.Remote.Aws"], fun _ ->
            Dap.Remote.Aws.Dsl.compile ["../src" ; "Dap.Remote.Aws"]
            |> List.iter traceSuccess
        ["Dap.Remote.Squidex"], fun _ ->
            Dap.Remote.Squidex.Dsl.compile ["../src" ; "Dap.Remote.Squidex"]
            |> List.iter traceSuccess
        ["Dap.Remote.Squidex.Sync"], fun _ ->
            Dap.Remote.Squidex.Sync.Dsl.compile ["../src" ; "Dap.Remote.Squidex.Sync"]
            |> List.iter traceSuccess
        ["Dap.Fable"], fun _ ->
            Dap.Fable.Dsl.compile ["../src" ; "Fable.Dap.Local"]
            |> List.iter traceSuccess
    ]

[<EntryPoint>]
let main argv =
    argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext
    createTargets ()
    Target.runOrDefaultWithArguments DotNet.Build
    0

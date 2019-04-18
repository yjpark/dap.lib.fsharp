(* FAKE: 5.12.1 *)
#r "paket: groupref Main //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Dap.Local/Meta.fs"
#load "src/Dap.Local/Dsl.fs"
#load "src/Dap.Local.Farango/Dsl.fs"

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

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Dap.Local"], fun _ ->
        Dap.Local.Dsl.compile ["src" ; "Dap.Local"]
        |> List.iter traceSuccess
    ["Dap.Local.Farango"], fun _ ->
        Dap.Local.Farango.Dsl.compile ["src" ; "Dap.Local.Farango"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

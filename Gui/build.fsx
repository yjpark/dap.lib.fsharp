#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Fable.Dap.Gui/Shared/Dsl/Widgets.fs"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators

open Dap.Build

[<Literal>]
let Prepare = "Prepare"

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Environment "API_KEY_nuget_yjpark_org"
}

let projects =
    !! "src/Fable.Dap.Gui/*.fsproj"
    ++ "src/Dap.Gui/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Fable.Dap.Gui"], fun _ ->
        Dap.Gui.Dsl.Widgets.compile ["src" ; "Fable.Dap.Gui"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

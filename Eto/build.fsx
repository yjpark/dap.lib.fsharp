#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Dap.Eto/Builder/Helper.fs"
#load "src/Dap.Eto/Generator/Gui.fs"
#load "src/Dap.Eto/Generator/Helper.fs"
#load "src/Dap.Eto/Dsl/Prefabs.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Environment "API_KEY_nuget_yjpark_org"
}

let projects =
    !! "src/Dap.Eto/*.fsproj"
    ++ "src/Dap.Eto.Gtk/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Dap.Eto"], fun _ ->
        Dap.Eto.Dsl.Prefabs.compile ["src" ; "Dap.Eto"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

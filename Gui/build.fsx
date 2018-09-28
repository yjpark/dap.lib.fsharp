#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Fable.Dap.Gui/Shared/Dsl.fs"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators

open Dap.Build

[<Literal>]
let Prepare = "Prepare"

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Plain "wnHZEG9N_OrmO3XKoAGT"
}

let projects =
    !! "src/Fable.Dap.Gui/*.fsproj"
    ++ "src/Dap.Gui/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepares [
    ["Fable.Dap.Gui"], fun _ ->
        Dap.Gui.Dsl.compile ["src" ; "Fable.Dap.Gui"]
        |> List.iter traceSuccess
]

Target.runOrDefault DotNet.Build

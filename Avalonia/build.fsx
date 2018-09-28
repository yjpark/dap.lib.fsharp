#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
//#load "src/Dap.Avalonia/Dsl.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Plain "wnHZEG9N_OrmO3XKoAGT"
}

let projects =
    !! "src/Dap.Avalonia/*.fsproj"

NuGet.create NuGet.release feed projects

(*
DotNet.createPrepares [
    ["Dap.Avalonia"], fun _ ->
        Dap.Avalonia.Dsl.compile ["src" ; "Dap.Avalonia"]
        |> List.iter traceSuccess
]
*)

Target.runOrDefault DotNet.Build

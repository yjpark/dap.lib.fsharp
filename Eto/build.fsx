#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
//#load "src/Dap.Eto/Dsl.fs"

open Fake.Core
open Fake.IO.Globbing.Operators

open Dap.Build

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Plain "wnHZEG9N_OrmO3XKoAGT"
}

let projects =
    !! "src/Dap.Eto/*.fsproj"
    ++ "src/Dap.Eto.Gtk/*.fsproj"

NuGet.create NuGet.release feed projects

(*
DotNet.createPrepares [
    ["Dap.Eto"], fun _ ->
        Dap.Eto.Dsl.compile ["src" ; "Dap.Eto"]
        |> List.iter traceSuccess
]
*)

Target.runOrDefault DotNet.Build

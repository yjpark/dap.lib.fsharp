#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"
#load "src/Fable.Dap.Local/Shared/Gui/Dsl.fs"

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
    !! "src/Fable.Dap.Local/*.fsproj"
    ++ "src/Dap.Local/*.fsproj"
    ++ "src/Dap.Local.Farango/*.fsproj"

NuGet.create NuGet.release feed projects

DotNet.createPrepare "Fable.Dap.Local" (fun project ->
    trace <| Dap.Local.Gui.Dsl.compile ["src" ; project]
) ["Dap.Local"]

Target.runOrDefault DotNet.Build

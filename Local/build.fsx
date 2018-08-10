#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators

module NuGet = Dap.Build.NuGet

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Plain "wnHZEG9N_OrmO3XKoAGT"
}

let projects =
    !! "src/Fable.Dap.Local/*.fsproj"
    ++ "src/Dap.Local/*.fsproj"
    ++ "src/Dap.Local.Farango/*.fsproj"

NuGet.createAndRun NuGet.release feed projects

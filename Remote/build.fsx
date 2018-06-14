#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators

module NuGet = Dap.Build.NuGet

let cleanDirs =
    !! "src/**/bin"
    ++ "src/**/obj"

let projects =
    !! "src/Dap.Remote.FSharpData/*.fsproj"
    ++ "src/Dap.Remote.AspNetCore/*.fsproj"

let feed : NuGet.Feed = {
    NuGet.Source = "https://nuget.yjpark.org/nuget/dap"
    NuGet.ApiKey = NuGet.Environment "API_KEY_nuget_yjpark_org"
}

NuGet.run cleanDirs projects feed
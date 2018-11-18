#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators

module NuGet = Dap.Build.NuGet

let feed =
    NuGet.Feed.Create (
        server = NuGet.ProGet "https://nuget.yjpark.org/nuget/dap",
        apiKey = NuGet.Environment "API_KEY_nuget_yjpark_org"
    )

let projects =
    !! "src/Dap.Remote.FSharpData/*.fsproj"
    ++ "src/Dap.Remote.Server/*.fsproj"
    ++ "src/Dap.Remote.Console/*.fsproj"
    ++ "src/Dap.Remote.Web/*.fsproj"

NuGet.createAndRun NuGet.release feed projects

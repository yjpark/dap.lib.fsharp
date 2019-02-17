#r "paket: groupref Build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO.Globbing.Operators

module NuGet = Dap.Build.NuGet

let feed =
    NuGet.Feed.Create (
        apiKey = NuGet.Environment "API_KEY_nuget_org"
    )

let projects =
    !! "src/Fable.Dap.React/*.fsproj"
    ++ "src/Dap.Fable/*.fsproj"

NuGet.createAndRun NuGet.release feed projects

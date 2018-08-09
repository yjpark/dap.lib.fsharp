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
    !! "src/Dap.Remote.FSharpData/*.fsproj"
    ++ "src/Dap.Remote.AspNetCore/*.fsproj"
    ++ "src/Dap.Remote.JoseJwt/*.fsproj"
    ++ "src/Dap.Remote.Server/*.fsproj"
    ++ "src/Dap.Remote.Web/*.fsproj"

NuGet.createAndRun NuGet.release feed projects

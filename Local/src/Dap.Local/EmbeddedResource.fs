[<AutoOpen>]
module Dap.Local.EmbeddedResource

open System.Reflection;

open Dap.Prelude

type EmbeddedResource = EmbeddedResourceHelper with
    static member GetName (relPath : string, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let resPath = relPath.Replace("/", ".");
        sprintf "%s.%s" (assembly.GetName().Name) resPath
    static member GetNames (?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        assembly.GetManifestResourceNames ()
    static member LogNames (logger : ILogger, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        EmbeddedResource.GetNames (assembly = assembly)
        |> Array.iter (fun name ->
            logWarn logger "EmbeddedResource" "ResourceName" name
        )
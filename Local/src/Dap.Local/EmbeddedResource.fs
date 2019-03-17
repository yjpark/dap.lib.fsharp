[<AutoOpen>]
module Dap.Local.EmbeddedResource

open System.IO;
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
        EmbeddedResource.GetNames (?assembly = assembly)
        |> Array.iter (fun name ->
            logWarn logger "EmbeddedResource" "ResourceName" name
        )
    static member TryCreateFromStream<'v> (relPath : string, create : (System.IO.Stream -> 'v), ?logger : ILogger, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let name = EmbeddedResource.GetName (relPath, assembly = assembly)
        try
            use stream = assembly.GetManifestResourceStream (name)
            Some <| create stream
        with e ->
            let logger =
                logger
                |> Option.defaultWith (fun () -> getLogging () :> ILogger)
            logException logger "EmbeddedResource.FromStream" (typeof<'v>.FullName) (name) e
            None



[<AutoOpen>]
module Dap.Local.EmbeddedResource

open System.IO;
open System.Reflection;

open Dap.Prelude
open Dap.Local

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
    static member TryOpenStream (relPath : string, ?logger : ILogger, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let name = EmbeddedResource.GetName (relPath, assembly = assembly)
        try
            let stream = assembly.GetManifestResourceStream (name)
            if stream = null then
                let logger =
                    logger
                    |> Option.defaultWith (fun () -> getLogging () :> ILogger)
                logError logger "EmbeddedResource.TryOpenStream" "Not_Found" (name, assembly)
                //EmbeddedResource.LogNames (logger = logger, assembly = assembly)
                None
            else
                Some stream
        with e ->
            let logger =
                logger
                |> Option.defaultWith (fun () -> getLogging () :> ILogger)
            logException logger "EmbeddedResource.TryOpenStream" name (assembly) e
            None

    static member TryCreateFromStream<'v> (relPath : string, create : (System.IO.Stream -> 'v), ?logger : ILogger, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        EmbeddedResource.TryOpenStream (relPath, ?logger = logger, assembly = assembly)
        |> Option.bind (fun stream ->
            try
                let result = Some <| create stream
                stream.Close ()
                result
            with e ->
                let logger =
                    logger
                    |> Option.defaultWith (fun () -> getLogging () :> ILogger)
                logException logger "EmbeddedResource.TryCreateFromStream" (typeof<'v>.FullName) (assembly, relPath) e
                None
        )



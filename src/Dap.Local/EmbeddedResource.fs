[<AutoOpen>]
module Dap.Local.EmbeddedResource

open System.IO
open System.Text
open System.Reflection

open Dap.Prelude
open Dap.Context
open Dap.Local

let private decodeFromStream<'v> (decoder : JsonDecoder<'v>) (stream : Stream) =
    use reader = new StreamReader (stream, Encoding.UTF8)
    reader.ReadToEnd ()
    |> decodeJson decoder

type EmbeddedResource = EmbeddedResourceHelper with
    static member GetName (relPath : string, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let resPath = relPath.Replace("/", ".")
        sprintf "%s.%s" (assembly.GetName().Name) resPath
    static member GetNames (?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let prefix = sprintf "%s." (assembly.GetName().Name)
        assembly.GetManifestResourceNames ()
        |> Array.map (fun path ->
            path.Replace (prefix, "")
        )
    static member LogNames (logger : ILogger, ?assembly : Assembly) =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let name = assembly.GetName().Name
        EmbeddedResource.GetNames (assembly = assembly)
        |> (fun names ->
            logWarn logger "EmbeddedResource" name (sprintf "[%d]" names.Length, assembly)
            names
        )|> Array.iter (fun name ->
            logWarn logger "EmbeddedResource" name name
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
    static member DecodeMultiple<'v> (prefix : string, decoder : JsonDecoder<'v>, ?logger : ILogger, ?assembly : Assembly) : (string * 'v) array =
        let assembly = assembly |> Option.defaultValue (Assembly.GetCallingAssembly ())
        let prefix = prefix.Replace ("/", ".")
        let logger =
            logger
            |> Option.defaultWith (fun () -> getLogging () :> ILogger)
        EmbeddedResource.GetNames (assembly)
        |> Array.choose (fun name ->
            if name.StartsWith prefix then
                EmbeddedResource.TryCreateFromStream (relPath = name, create = decodeFromStream decoder, logger = logger, assembly = assembly)
                |> Option.map (fun v -> name.Replace (prefix, ""), v)
            else
                //logWarn logger "EmbeddedResource.CreateMultiple" "Prefix_Not_Matched" (name, prefix)
                None
        )

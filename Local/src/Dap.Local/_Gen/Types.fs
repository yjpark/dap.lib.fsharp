[<AutoOpen>]
module Dap.Local.Types

open Dap.Prelude
open Dap.Context

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type FileSystemArgs = {
    AppData : string
    AppCache : string
} with
    static member Create appData appCache
            : FileSystemArgs =
        {
            AppData = appData
            AppCache = appCache
        }
    static member Default () =
        FileSystemArgs.Create
            ""
            ""
    static member JsonEncoder : JsonEncoder<FileSystemArgs> =
        fun (this : FileSystemArgs) ->
            E.object [
                "app_data", E.string this.AppData
                "app_cache", E.string this.AppCache
            ]
    static member JsonDecoder : JsonDecoder<FileSystemArgs> =
        D.decode FileSystemArgs.Create
        |> D.optional "app_data" D.string ""
        |> D.optional "app_cache" D.string ""
    static member JsonSpec =
        FieldSpec.Create<FileSystemArgs>
            FileSystemArgs.JsonEncoder FileSystemArgs.JsonDecoder
    interface IJson with
        member this.ToJson () = FileSystemArgs.JsonEncoder this
    interface IObj
    member this.WithAppData (appData : string) = {this with AppData = appData}
    member this.WithAppCache (appCache : string) = {this with AppCache = appCache}

type ILocalPackArgs =
    abstract FileSystem : FileSystemArgs with get

type ILocalPack =
    abstract Args : ILocalPackArgs with get
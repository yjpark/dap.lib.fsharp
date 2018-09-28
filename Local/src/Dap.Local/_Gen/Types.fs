[<AutoOpen>]
module Dap.Local.Types

open Dap.Context.Helper
open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type FileSystemArgs = {
    AppData : (* FileSystemArgs *) string
    AppCache : (* FileSystemArgs *) string
} with
    static member Create appData appCache
            : FileSystemArgs =
        {
            AppData = (* FileSystemArgs *) appData
            AppCache = (* FileSystemArgs *) appCache
        }
    static member Default () =
        FileSystemArgs.Create
            (* FileSystemArgs *) (* appData *) ""
            (* FileSystemArgs *) (* appCache *) ""
    static member SetAppData ((* FileSystemArgs *) appData : string) (this : FileSystemArgs) =
        {this with AppData = appData}
    static member SetAppCache ((* FileSystemArgs *) appCache : string) (this : FileSystemArgs) =
        {this with AppCache = appCache}
    static member UpdateAppData ((* FileSystemArgs *) update : string -> string) (this : FileSystemArgs) =
        this |> FileSystemArgs.SetAppData (update this.AppData)
    static member UpdateAppCache ((* FileSystemArgs *) update : string -> string) (this : FileSystemArgs) =
        this |> FileSystemArgs.SetAppCache (update this.AppCache)
    static member JsonEncoder : JsonEncoder<FileSystemArgs> =
        fun (this : FileSystemArgs) ->
            E.object [
                (* FileSystemArgs *) "app_data", E.string this.AppData
                (* FileSystemArgs *) "app_cache", E.string this.AppCache
            ]
    static member JsonDecoder : JsonDecoder<FileSystemArgs> =
        D.decode FileSystemArgs.Create
        |> D.optional (* FileSystemArgs *) "app_data" D.string ""
        |> D.optional (* FileSystemArgs *) "app_cache" D.string ""
    static member JsonSpec =
        FieldSpec.Create<FileSystemArgs>
            FileSystemArgs.JsonEncoder FileSystemArgs.JsonDecoder
    interface IJson with
        member this.ToJson () = FileSystemArgs.JsonEncoder this
    interface IObj
    member this.WithAppData ((* FileSystemArgs *) appData : string) =
        this |> FileSystemArgs.SetAppData appData
    member this.WithAppCache ((* FileSystemArgs *) appCache : string) =
        this |> FileSystemArgs.SetAppCache appCache

type ILocalPackArgs =
    abstract FileSystem : FileSystemArgs with get

type ILocalPack =
    inherit IPack
    abstract Args : ILocalPackArgs with get
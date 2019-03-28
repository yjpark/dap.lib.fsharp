[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Feature.Preferences

open System.IO
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

type Context (logging : ILogging) =
    inherit BasePreferences<Context> (logging)
    do (
        let environment = Environment.getInstance ()
        let root = base.Properties.Root
        let getPath = fun (luid : Luid) ->
            let folder = Path.Combine (environment.Properties.DataDirectory.Value, root.Value)
            Path.Combine (folder, luid)
        base.Has.SetupHandler (fun (luid : Luid) ->
            FileSystem.fileExists <| getPath luid
        )
        base.Get.SetupHandler (fun (luid : Luid) ->
            TextFile.load <| getPath luid
        )
        base.Set.SetupHandler (fun (req : SetTextReq) ->
            TextFile.save (getPath req.Path) req.Text
        )
        base.Remove.SetupHandler (fun (luid : Luid) ->
            FileSystem.deleteFile <| getPath luid
            |> ignore
        )
        base.Clear.SetupHandler (fun () ->
            FileSystem.deleteFolder root.Value
            |> ignore
        )
    )
    override this.Self = this
    override __.Spawn l = new Context (l)
    interface IFallback

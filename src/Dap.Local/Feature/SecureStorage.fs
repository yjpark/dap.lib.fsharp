[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Feature.SecureStorage

open System.IO

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local

type Context (logging : ILogging) =
    inherit BaseSecureStorage<Context> (logging)
    do (
        let owner = base.Owner
        let environment = Environment.getInstance ()
        let root = base.Properties.Root
        let secret = base.Properties.Secret
        let getPath = fun (luid : Luid) ->
            let folder = Path.Combine (environment.Properties.DataDirectory.Value, root.Value)
            Path.Combine (folder, luid)
        let encrypt (content : string) =
            Des.encrypt secret.Value content
        let decrypt (content : string) : string option =
            Des.decrypt owner secret.Value content
        base.HasAsync.SetupHandler (fun (luid : Luid) -> task {
            return FileSystem.fileExists <| getPath luid
        })
        base.GetAsync.SetupHandler (fun (luid : Luid) -> task {
            return
                TextFile.load <| getPath luid
                |> Option.bind decrypt
        })
        base.SetAsync.SetupHandler (fun (req : SetTextReq) -> task {
            return TextFile.save (getPath req.Path) (encrypt req.Text)
        })
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

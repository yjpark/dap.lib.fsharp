[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Db

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

type Args = {
    Uri : string
} with
    static member Create uri =
        {
            Uri = uri
        }

type Model = {
    Conn : Connection
}

let init (runner : IEnv) (args : Args) : Model =
    let conn =
        args.Uri
        |> Result.ofTry (
            connect >> Async.RunSynchronously
        )|> Result.mapError (fun e ->
            logException runner "DB" "Connect_Failed" args.Uri e
            failwith <| sprintf "[DB] Connect_Failed: %s -> %s" args.Uri e.Message
        )|> Result.get |> Result.get
    {
        Conn = conn
    }
[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Db

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

[<Literal>]
let JK_DocumentKey = "_key"

type Args = {
    Uri : string
} with
    static member Create uri =
        {
            Uri = uri
        }

type Model = {
    Conn : Connection
    Logger : ILogger
} with
    member this.CursorUrl =
        sprintf "_db/%s/_api/cursor" this.Conn.Database
    member this.LogResult (op : string) (res : Result<'a, 'b>) =
        match res with
        | Ok res ->
            logInfo this.Logger "DB_Succeed" op res
        | Error err ->
            logError this.Logger "DB_Failed" op res
    interface ILogger with
        member this.Log m = this.Logger.Log m

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
        Logger = runner.Logging.GetLogger "DB"
    }
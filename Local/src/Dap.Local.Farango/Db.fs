[<AutoOpen>]
module Dap.Local.Farango.Db

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

[<Literal>]
let Kind = Dsl.FarangoDbKind

[<Literal>]
let JK_DocumentKey = "_key"

type IndexIsUnique = bool
type IndexIsSparse = bool
type IndexIsDeduplicate = bool

type IndexDef = Farango.Collections.IndexSetting

type CollectionDef = {
    Name : string
    Indexes : IndexDef list
} with
    static member Create name indexes =
        {
            Name = name
            Indexes = indexes
        }

type Args = DbArgs
and Req = NoReq
and Evt = NoEvt
and Msg = NoMsg

and Model = {
    Runner : IAgent
    Conn : Connection
} with
    member this.CursorUrl =
        sprintf "_db/%s/_api/cursor" this.Conn.Database
    member this.LogResult (op : string) (res : Result<'a, 'b>) =
        match res with
        | Ok res ->
            logInfo this.Runner "DB_Succeed" op res
        | Error err ->
            logError this.Runner "DB_Failed" op err
    interface ILogger with
        member this.Log m = this.Runner.Log m

type Agent (param) =
    inherit BaseAgent<Agent, Args, Model, Msg, Req, Evt> (param)
    override this.Runner = this
    static member Spawn (param) = new Agent (param)
    member this.Conn = this.Actor.State.Conn

let private init : ActorInit<Args, Model, Msg> =
    fun runner args ->
        let uri = args.Uri
        let conn =
            uri
            |> Result.ofTry (
                connect >> Async.RunSynchronously
            )|> Result.mapError (fun e ->
                logException runner "DB" "Connect_Failed" uri e
                failwith <| sprintf "[DB] Connect_Failed: %s -> %s" uri e.Message
            )|> Result.get |> Result.get
        ({
            Runner = runner
            Conn = conn
        }, noCmd)

let private update : Update<Agent, Model, Msg> =
    fun runner msg model ->
        (model, noCmd)

let spec args =
    new ActorSpec<Agent, Args, Model, Msg, Req, Evt>
        (Agent.Spawn, args, noWrapReq, noCastEvt, init, update)

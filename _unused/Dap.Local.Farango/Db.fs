[<AutoOpen>]
module Dap.Local.Farango.Db

open System.Threading.Tasks

open Farango.Types
open Farango.Connection

open Dap.Prelude
open Dap.Platform

[<Literal>]
let Kind = Dsl.FarangoDbKind

[<Literal>]
let JK_DocumentKey = "_key"

type Connection = Farango.Types.Connection

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
    NewConn : unit -> Connection
    mutable _Conn : Connection
} with
    member this.CursorUrl =
        sprintf "_db/%s/_api/cursor" this._Conn.Database
    member this.LogResult (op : string) (res : Result<'a, 'b>) =
        match res with
        | Ok res ->
            logInfo this.Runner "DB_Succeed" op res
        | Error err ->
            logError this.Runner "DB_Failed" op err
    member this.ExecAsync'<'res> (execAsync : Connection -> Async<Result<'res, string>>) = async {
        let! result = execAsync this._Conn
        match result with
        | Ok _ ->
            return result
        | Error err ->
            //In Case of the JWT Expired
            if err.StartsWith ("Connection failed with 401:") then
                this._Conn <- this.NewConn ()
                return! execAsync this._Conn
            else
                return result
    }
    member this.ExecAsync<'res> (execAsync : Connection -> Task<Result<'res, string>>) = task {
        let! result = execAsync this._Conn
        match result with
        | Ok _ ->
            return result
        | Error err ->
            //In Case of the JWT Expired
            if err.StartsWith ("Connection failed with 401:") then
                this._Conn <- this.NewConn ()
                return! execAsync this._Conn
            else
                return result
    }

    interface ILogger with
        member this.Log m = this.Runner.Log m

type Agent (param) =
    inherit BaseAgent<Agent, Args, Model, Msg, Req, Evt> (param)
    override this.Runner = this
    static member Spawn (param) = new Agent (param)
    //member this.Conn = this.Actor.State._Conn

let private newConn (runner : IRunner) (uri : string) () =
    uri
    |> Result.ofTry (
        connect >> Async.RunSynchronously
    )|> Result.mapError (fun e ->
        logException runner "DB" "Connect_Failed" uri e
        failwith <| sprintf "[DB] Connect_Failed: %s -> %s" uri e.Message
    )|> Result.get |> Result.get

let private init : ActorInit<Args, Model, Msg> =
    fun runner args ->
        let uri = args.Uri
        let newConn' = newConn runner uri
        ({
            Runner = runner
            NewConn = newConn'
            _Conn = newConn' ()
        }, noCmd)

let private update : Update<Agent, Model, Msg> =
    fun runner msg model ->
        (model, noCmd)

let spec args =
    new ActorSpec<Agent, Args, Model, Msg, Req, Evt>
        (Agent.Spawn, args, noWrapReq, noCastEvt, init, update)

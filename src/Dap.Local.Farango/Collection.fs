[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Collection

open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local.Farango
open Dap.Local.Farango.Util

type Collection = {
    Db : Db
    Name : string
} with
    static member Create db name =
        {
            Db = db
            Name = name
        }
    member this.CreateDocumentAsync (json : Json) =
        let key = castJson (D.field JK_DocumentKey D.string) json
        let body = E.encode 0 json
        this.Db |> Document.createAsync this.Name key body
    member inline this.ReplyCreateDocumentAsync'<'runner, 'res when 'runner :> IRunner> (json : Json) (newRes : string -> 'res) : GetReplyTask<'runner, 'res> =
        fun req callback runner -> task {
            match! this.CreateDocumentAsync json with
            | Ok res ->
                reply runner callback <| ack req ^<| newRes res
            | Error err ->
                reply runner callback <| nak req "DB_Error" (err, E.encode 4 json)
        }
    member inline this.ReplyCreateDocumentAsync<'runner when 'runner :> IRunner> (json : Json) : GetReplyTask<'runner, unit> =
        this.ReplyCreateDocumentAsync' json ignore

type Db.Model with
    member this.GetCollection name = Collection.Create this name

type IJson with
    member this.CreateDocumentAsync (collection : Collection) =
        collection.CreateDocumentAsync <| this.ToJson ()

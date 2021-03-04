[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.Util.Documents

open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Farango.Documents

open Dap.Prelude
open Dap.Context
open Dap.Local.Farango


let getAsync<'res> (decoder : JsonDecoder<'res>)
        (statement : string) (batchSize : int option) (bindVars : Json option)
        (db : Db) : Task<Result<'res list, string>> = task {
    let query = Query.Create statement batchSize bindVars
    let! results = db.ExecuteQueryAsync query
    return
        results
        |> Result.map (fun results ->
            results
            |> List.choose (fun x ->
                match tryDecodeJson decoder x with
                | Ok r -> Some r
                | Error err ->
                    logError db "Documents.getAsync" "Decode_Failed" (query, x, err)
                    None
            )
        )
}
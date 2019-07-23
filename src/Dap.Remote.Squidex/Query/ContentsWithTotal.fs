[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Squidex.Query.ContentsWithTotal

open System.Text
open System.Globalization

open Dap.Prelude
open Dap.Context
open Dap.Platform

open Dap.Remote.FSharpData

open Dap.Remote.Squidex

let getQueryName = Contents.getQueryName' "ContentsWithTotal"

type Res = ContentsWithTotalResult

let private decodeRes (query : ContentsQuery) : JsonDecoder<Res> =
    Res.JsonDecoderWithFlatten query
    |> Contents.decodeRes'<Res> (getQueryName query.Schema)

type Args = Dap.Remote.Squidex.Query.Contents.Args

type Dap.Remote.Squidex.Query.Contents.Args with
    member this.ToContentsWithTotalRequest : Http.Request<Res> =
        let query = SquidexItem.WrapContentsQuery true this.Query
        let decoder = decodeRes this.Query
        let body = query.ToBody getQueryName
        this.ToRequest' query decoder body

let queryAsync : AsyncApi<IRunner, Args, Http.Response<Res>> =
    fun runner args ->
        Http.handleAsync runner args.ToContentsWithTotalRequest
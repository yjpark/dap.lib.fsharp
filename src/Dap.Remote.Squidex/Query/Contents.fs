[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Squidex.Query.Contents

open System.Text
open System.Globalization

open Dap.Prelude
open Dap.Context
open Dap.Platform

open Dap.Remote.FSharpData

open Dap.Remote.Squidex

let private textInfo = (new CultureInfo("en-US", false)) .TextInfo

let getQueryName' (pattern : string) (schema : string) =
    sprintf "query%s%s" (textInfo.ToTitleCase schema) pattern

let getQueryName = getQueryName' "Contents"

type Res = SquidexItem list

let decodeRes'<'res> (queryName : string) (decoder : JsonDecoder<'res>) : JsonDecoder<'res> =
    decoder
    |> D.field queryName
    |> D.field "data"

let private decodeRes (query : ContentsQuery) : JsonDecoder<Res> =
    D.list (SquidexItem.JsonDecoderWithFlatten query)
    |> decodeRes'<Res> (getQueryName query.Schema)


type Args = {
    Config : SquidexConfig
    Query : ContentsQuery
    Timeout : int<ms> option
    ExtraHeaders : seq<string * string> option
} with
    static member Create
        (
            config : SquidexConfig,
            query : ContentsQuery,
            ?timeout : int<ms>,
            ?extraHeaders : seq<string * string>
        ) =
        {
            Config = config
            Query = query
            Timeout = timeout
            ExtraHeaders = extraHeaders
        }
    member this.ToRequest'<'res> (query : ContentsQuery) (decoder : JsonDecoder<'res>) body
                : Http.Request<'res> =
        {
            Method = Http.Method.Post
            Url = this.Config.GraphqlUrl
            Decoder = decoder
            Timeout = this.Timeout
            Headers = Some (this.Config.Headers this.ExtraHeaders)
            Body = Some body
        }
    member this.ToContentsRequest : Http.Request<Res> =
        let query = SquidexItem.WrapContentsQuery false this.Query
        let decoder = decodeRes this.Query
        let body = query.ToBody this.Config getQueryName
        this.ToRequest' query decoder body

let queryAsync : AsyncApi<IRunner, Args, Http.Response<Res>> =
    fun runner args ->
        Http.handleAsync runner args.ToContentsRequest
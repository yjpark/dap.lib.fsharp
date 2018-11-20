[<AutoOpen>]
module Dap.Remote.Web.Http

open System
open System.Text
open Microsoft.AspNetCore.Http
open Giraffe

open Dap.Prelude
open Dap.Context

type Http = HttpHelper with
    static member TextContentType = "text/plain; charset=utf-8"
    static member JsonContentType = "application/json; charset=utf-8"
    static member Return (statusCode : int, bytes : byte [], ?contentType : string) =
        fun (ctx : HttpContext) ->
            ctx.SetStatusCode statusCode
            contentType |> Option.iter ctx.SetContentType
            ctx.WriteBytesAsync bytes
    static member Return (statusCode : int, text : string) =
        let bytes = Encoding.UTF8.GetBytes text
        Http.Return (statusCode, bytes, contentType = Http.TextContentType)
    static member Return (statusCode : int, json : IJson, ?tabs : int) =
        let text = encodeJson (defaultArg tabs 4) json
        let bytes = Encoding.UTF8.GetBytes text
        Http.Return (statusCode, bytes, contentType = Http.JsonContentType)

type Http with
    static member Ok (text : string) = Http.Return (200, text)
    static member Ok (json : IJson, ?tabs : int) = Http.Return (200, json, ?tabs = tabs)

type Http with
    static member NotFound (text : string) = Http.Return (404, text)
    static member NotFound (json : IJson, ?tabs : int) = Http.Return (404, json, ?tabs = tabs)

type Http with
    static member InternalError (text : string) = Http.Return (500, text)
    static member InternalError (json : IJson, ?tabs : int) = Http.Return (500, json, ?tabs = tabs)


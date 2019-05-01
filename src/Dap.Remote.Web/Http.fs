[<AutoOpen>]
module Dap.Remote.Web.Http

open System
open System.Text
open FSharp.Control.Tasks.V2
open Microsoft.AspNetCore.Http
open Giraffe

open Dap.Prelude
open Dap.Context

let fileContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider ()

type Http = HttpHelper with
    static member TextContentType = "text/plain; charset=utf-8"
    static member JsonContentType = "application/json; charset=utf-8"
    static member Return (statusCode : int, bytes : byte [],
                            ?contentType : string, ?filename : string) =
        fun (ctx : HttpContext) ->
            ctx.SetStatusCode statusCode
            contentType
            |> Option.defaultWith (fun () ->
                filename
                |> Option.bind (fun x ->
                    let result, value = fileContentTypeProvider.TryGetContentType (x)
                    if result then
                        Some value
                    else
                        None
                )|> Option.defaultValue "application/octet-stream"
            )|> ctx.SetContentType
            ctx.WriteBytesAsync bytes
    static member Return (statusCode : int, text : string) =
        let bytes = Encoding.UTF8.GetBytes text
        Http.Return (statusCode, bytes, contentType = Http.TextContentType)
    static member Return (statusCode : int, json : IJson, ?tabs : int) =
        let text = encodeJson (defaultArg tabs 4) json
        let bytes = Encoding.UTF8.GetBytes text
        Http.Return (statusCode, bytes, contentType = Http.JsonContentType)

type Http with
    static member Ok (bytes : byte [], ?contentType : string, ?filename : string) =
        Http.Return (200, bytes, ?contentType = contentType, ?filename = filename)
    static member Ok (text : string) = Http.Return (200, text)
    static member Ok (json : IJson, ?tabs : int) = Http.Return (200, json, ?tabs = tabs)

type Http with
    static member Unauthorized (text : string) = Http.Return (401, text)
    static member Unauthorized (json : IJson, ?tabs : int) = Http.Return (401, json, ?tabs = tabs)
    static member Forbidden (text : string) = Http.Return (403, text)
    static member Forbidden (json : IJson, ?tabs : int) = Http.Return (403, json, ?tabs = tabs)
    static member BadRequest (text : string) = Http.Return (400, text)
    static member BadRequest (json : IJson, ?tabs : int) = Http.Return (400, json, ?tabs = tabs)
    static member NotFound (text : string) = Http.Return (404, text)
    static member NotFound (json : IJson, ?tabs : int) = Http.Return (404, json, ?tabs = tabs)

type Http with
    static member InternalError (text : string) = Http.Return (500, text)
    static member InternalError (json : IJson, ?tabs : int) = Http.Return (500, json, ?tabs = tabs)

type Http with
    static member HandleJsonPost<'param when 'param :> IJson> (decoder : JsonDecoder<'param>) (samples : Json list) (getNext : 'param -> HttpFunc) =
        fun (ctx : HttpContext) -> task {
            let! body = ctx.ReadBodyFromRequestAsync ()
            match tryDecodeJson decoder body with
            | Ok param ->
                let next = getNext param
                return! next ctx
            | Error err ->
                let res =
                    HttpTypes.InvalidBody.Create (
                        url = ctx.GetRequestUrl (),
                        body = body,
                        error = err,
                        samples = samples
                    )
                return! ctx |> Http.BadRequest (res)
        }

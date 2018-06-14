[<RequireQualifiedAccess>]
module Dap.Remote.Http

open System.Net
open FSharp.Data
open Newtonsoft.Json
open Newtonsoft.Json.Linq

open Dap.Prelude
open Dap.Platform
open Dap.Remote
open System.Text

module D = Thoth.Json.Net.Decode

type Method =
    | Get
    | Post
with
    member this.ToHttpMethod () =
        match this with
        | Get -> "GET"
        | Post -> "POST"

type Error =
    | BadUrl of string
    | NetworkError of WebException
    | InternalError of exn
    | BadStatus of HttpResponse
    | BadPayload of HttpResponse * D.DecoderError

type Request<'res> = {
    Method : Method
    Url : string
    Decoder : D.Decoder<'res>
    Timeout : int<ms> option
    Headers : seq<string * string> option
    Body : HttpRequestBody option
} with
    static member Create method url decoder timeout headers body =
        {
            Method = method
            Url = url
            Decoder = decoder
            Timeout = timeout
            Headers = headers
            Body = body
        }

type Response<'res> = Result<'res * string, Error>

let private tplFailed<'res> : Request<'res> -> Error -> LogEvent =
    LogEvent.Template3<string, Request<'res>, Error>(LogLevelError, "[{Section}] {Req} ~> Failed: {Error}") "Http"

let handleAsync (runner : IRunner) (callback : Response<'res> -> unit) (req : Request<'res>) = async {
    let callback' = fun res ->
        res
        |> Result.mapError (fun err ->
            runner.Log <| tplFailed req err
            err
        )|> callback
    try
        let httpMethod = Some <| req.Method.ToHttpMethod ()
        let timeout = req.Timeout |> Option.map (fun t -> t / 1<ms>)
        let! response = FSharp.Data.Http.AsyncRequest (req.Url, ?headers = req.Headers, ?httpMethod = httpMethod, ?body = req.Body, ?timeout = timeout)
        match response.Body with
        | Text text ->
            decodeJson text
            |> req.Decoder
            |> Result.mapError (fun e -> BadPayload (response, e))
            |> Result.map (fun res -> (res, text))
            |> callback'
        | Binary bytes ->
            callback' <| Error ^<| BadPayload (response, D.FailMessage ^<| sprintf "Expecting text, but got a binary response (%d bytes)" bytes.Length)
    with
    | :? WebException as e ->
        callback' <| Error ^<| NetworkError e
    | e ->
        callback' <| Error ^<| InternalError e
}

let handle : Api<IRunner, Request<'res>, Response<'res>> =
    fun runner callback req ->
        handleAsync runner callback req
        |> Async.Start

let get runner callback url (decoder : D.Decoder<'res>) = 
    let req = Request<'res>.Create Get url decoder None None None
    handle runner callback req

let post runner callback url (decoder : D.Decoder<'res>) body =
    let req = Request<'res>.Create Post url decoder None None (Some body)
    handle runner callback req

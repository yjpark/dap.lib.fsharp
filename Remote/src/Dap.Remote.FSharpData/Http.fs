[<RequireQualifiedAccess>]
module Dap.Remote.Http

open System.Net
open FSharp.Data
open FSharp.Control.Tasks.V2

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
    | Put
    | Delete
with
    member this.ToHttpMethod () =
        match this with
        | Get -> "GET"
        | Post -> "POST"
        | Put -> "PUT"
        | Delete -> "DELETE"

type Error =
    | BadUrl of string
    | NetworkError of WebException
    | DecodeError of pkt:string * err:D.DecoderError
    | InternalError of exn
    | BadStatus of HttpResponse
    | BadPayload of HttpResponse * err:string

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

let handleAsync' (runner : IRunner) (req : Request<'res>) (callback : Response<'res> -> unit) = async {
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
            try
                decodeJson text
                |> req.Decoder
                |> Result.mapError (fun e ->
                    DecodeError (text, e)
                )|> Result.map (fun res -> (res, text))
                |> callback'
            with e ->
                callback' <| Error ^<| DecodeError (text, D.FailMessage <| sprintf "Exception_Raised: %s" e.Message)
        | Binary bytes ->
            callback' <| Error ^<| BadPayload (response, sprintf "Expecting text, but got a binary response (%d bytes)" bytes.Length)
    with
    | :? WebException as e ->
        callback' <| Error ^<| NetworkError e
    | e ->
        callback' <| Error ^<| InternalError e
}

let handleAsync : AsyncApi<IRunner, Request<'res>, Response<'res>> =
    fun runner req -> task {
        let mutable res = None
        fun r -> res <- Some r
        |> handleAsync' runner req
        |> Async.StartAsTask
        |> Async.AwaitTask
        |> ignore
        return res |> Option.get
    }

let handle : Api<IRunner, Request<'res>, Response<'res>> =
    fun runner req callback ->
        handleAsync' runner req callback
        |> Async.Start

let get runner url (decoder : D.Decoder<'res>) callback =
    let req = Request<'res>.Create Get url decoder None None None
    handle runner req callback

let getAsync runner url (decoder : D.Decoder<'res>) =
    let req = Request<'res>.Create Get url decoder None None None
    handleAsync runner req

let post runner url (decoder : D.Decoder<'res>) body callback =
    let req = Request<'res>.Create Post url decoder None None (Some body)
    handle runner req callback

let postAsync runner url (decoder : D.Decoder<'res>) body =
    let req = Request<'res>.Create Post url decoder None None (Some body)
    handleAsync runner req

let put runner url (decoder : D.Decoder<'res>) body callback =
    let req = Request<'res>.Create Put url decoder None None (Some body)
    handle runner req callback

let putAsync runner url (decoder : D.Decoder<'res>) body =
    let req = Request<'res>.Create Put url decoder None None (Some body)
    handleAsync runner req

let delete runner url (decoder : D.Decoder<'res>) callback =
    let req = Request<'res>.Create Delete url decoder None None None
    handle runner req callback

let deleteAsync runner url (decoder : D.Decoder<'res>) body =
    let req = Request<'res>.Create Delete url decoder None None None
    handleAsync runner req

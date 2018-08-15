[<RequireQualifiedAccess>]
module Dap.Remote.FSharpData.Http

open System.Net
open FSharp.Data
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Platform
open Dap.Remote
open System.Text
open System.Text

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
    | DecodeError of err:string
    | InternalError of exn
    | BadStatus of HttpResponse
    | BadPayload of HttpResponse * err:string

type Request<'res> = {
    Method : Method
    Url : string
    Decoder : JsonDecoder<'res>
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

type Response<'res> = {
    ReqTime : Instant
    Request : Request<'res>
    ResTime : Instant
    ResBody : string
    Result : Result<'res, Error>
} with
    member this.IsOk = this.Result |> Result.isOk
    member this.IsError = this.Result |> Result.isError

let getReqKindPayload (kind : string) (req : Request<'res>) =
    new System.Text.StringBuilder()
    |> fun b -> b.AppendLine <| req.Method.ToHttpMethod ()
    |> fun b -> b.AppendLine req.Url
    |> (fun b ->
        req.Headers
        |> Option.iter (fun headers ->
            b.AppendLine "Headers:"
            |> ignore
            for (k, v) in headers do
                b.AppendLine <| sprintf "\t%s = %s" k v
                |> ignore
        )
        b
    )|> (fun b ->
        req.Body
        |> Option.iter (fun body ->
            match body with
            | TextRequest text ->
                b.AppendLine "TextRequest:"
                |> ignore
                b.AppendLine text
                |> ignore
            | FormValues form ->
                b.AppendLine "FormValues:"
                |> ignore
                for (k, v) in form do
                    b.AppendLine <| sprintf "\t%s = %s" k v
                    |> ignore
            | _ ->
                b.AppendLine <| sprintf "Body: %A" body
                |> ignore
        )
        b
    )|> fun b -> (sprintf "%s:Req" kind, b.ToString ())

let getResKindPayload (kind : string) (res : Response<'res>) =
    match res.Result with
    | Ok _ ->
        (sprintf "%s:Ack" kind, res.ResBody)
    | Error err ->
        (sprintf "%s:Nak" kind, sprintf "%A" err)

let private tplSucceed<'res> =
    LogEvent.Template4<string, Request<'res>, 'res, string>(AckLogLevel, "[{Section}] {Req} ~> Succeed: {Response} {Body}") "Http"

let private tplFailed<'res> =
    LogEvent.Template4<string, Request<'res>, Error, string>(LogLevelError, "[{Section}] {Req} ~> Failed: {Error} {Body}") "Http"

let handleAsync' (runner : IRunner) (req : Request<'res>) (callback : (Response<'res>) -> unit) = async {
    let reqTime = runner.Clock.Now
    let callback' = fun ((resTime, resBody, result) : Instant * string * Result<'res, Error>) ->
        match result with
        | Ok res ->
            runner.Log <| tplSucceed<'res> req res resBody
        | Error err ->
            runner.Log <| tplFailed<'res> req err resBody
        {
            ReqTime = reqTime
            Request = req
            ResTime = resTime
            ResBody = resBody
            Result = result
        }|> callback
    try
        let httpMethod = Some <| req.Method.ToHttpMethod ()
        let timeout = req.Timeout |> Option.map (fun t -> t / 1<ms>)
        let! response = FSharp.Data.Http.AsyncRequest (req.Url, ?headers = req.Headers, ?httpMethod = httpMethod, ?body = req.Body, ?timeout = timeout)
        let resTime = runner.Clock.Now
        match response.Body with
        | Text text ->
            try
                tryDecodeJson req.Decoder text
                |> Result.mapError (fun e -> DecodeError e)
                |> fun res -> callback' (resTime, text, res)
            with e ->
                callback' (resTime, text, Error ^<| DecodeError ^<| sprintf "Exception_Raised: %s" e.Message)
        | Binary bytes ->
            callback' (resTime, "", Error ^<| BadPayload (response, sprintf "Expecting text, but got a binary response (%d bytes)" bytes.Length))
    with
    | :? WebException as e ->
        let resTime = runner.Clock.Now
        callback' (resTime, "", Error ^<| NetworkError e)
    | e ->
        let resTime = runner.Clock.Now
        callback' (resTime, "", Error ^<| InternalError e)
}

let handleAsync : AsyncApi<IRunner, Request<'res>, Response<'res>> =
    fun runner req -> task {
        let mutable res = None
        do! fun r -> (res <- Some r)
            |> handleAsync' runner req
            |> Async.StartAsTask
        return res |> Option.get
    }

let handle : Api<IRunner, Request<'res>, Response<'res>> =
    fun runner req callback ->
        handleAsync' runner req callback
        |> Async.Start

let get runner url (decoder : JsonDecoder<'res>) callback =
    let req = Request<'res>.Create Get url decoder None None None
    handle runner req callback

let getAsync runner url (decoder : JsonDecoder<'res>) =
    let req = Request<'res>.Create Get url decoder None None None
    handleAsync runner req

let post runner url (decoder : JsonDecoder<'res>) body callback =
    let req = Request<'res>.Create Post url decoder None None (Some body)
    handle runner req callback

let postAsync runner url (decoder : JsonDecoder<'res>) body =
    let req = Request<'res>.Create Post url decoder None None (Some body)
    handleAsync runner req

let put runner url (decoder : JsonDecoder<'res>) body callback =
    let req = Request<'res>.Create Put url decoder None None (Some body)
    handle runner req callback

let putAsync runner url (decoder : JsonDecoder<'res>) body =
    let req = Request<'res>.Create Put url decoder None None (Some body)
    handleAsync runner req

let delete runner url (decoder : JsonDecoder<'res>) callback =
    let req = Request<'res>.Create Delete url decoder None None None
    handle runner req callback

let deleteAsync runner url (decoder : JsonDecoder<'res>) body =
    let req = Request<'res>.Create Delete url decoder None None None
    handleAsync runner req

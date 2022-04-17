[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Aws.S3

open System.Threading.Tasks

open Dap.Prelude
open Dap.Context
open Dap.Platform
open System.Threading
open Amazon.S3.Model
open Amazon.S3.Model

// https://docs.aws.amazon.com/sdkfornet/v3/apidocs/Index.html

type S3Client = Amazon.S3.AmazonS3Client
type S3Config = Amazon.S3.AmazonS3Config
type S3Bucket = Amazon.S3.Model.S3Bucket
type S3Metadata = Amazon.S3.Model.MetadataCollection

type ListBucketsRequest = Amazon.S3.Model.ListBucketsRequest
type ListBucketsResponse = Amazon.S3.Model.ListBucketsResponse

type PutBucketRequest = Amazon.S3.Model.PutBucketRequest
type PutBucketResponse = Amazon.S3.Model.PutBucketResponse

type ListObjectsRequest = Amazon.S3.Model.ListObjectsRequest
type ListObjectsResponse = Amazon.S3.Model.ListObjectsResponse

type PutObjectRequest = Amazon.S3.Model.PutObjectRequest
type PutObjectResponse = Amazon.S3.Model.PutObjectResponse

type GetObjectRequest = Amazon.S3.Model.GetObjectRequest
type GetObjectResponse = Amazon.S3.Model.GetObjectResponse

type DeleteObjectRequest = Amazon.S3.Model.DeleteObjectRequest
type DeleteObjectResponse = Amazon.S3.Model.DeleteObjectResponse

type DeleteObjectsRequest = Amazon.S3.Model.DeleteObjectsRequest
type DeleteObjectsResponse = Amazon.S3.Model.DeleteObjectsResponse

type Amazon.S3.Model.S3Bucket with
    static member JsonEncoder : JsonEncoder<S3Bucket> =
        fun (this : S3Bucket) ->
            E.object [
                "bucket_name", E.string this.BucketName
                "creation_date", E.dateTime this.CreationDate
            ]
    static member JsonDecoder : JsonDecoder<S3Bucket> =
        D.object (fun get ->
            let result = new S3Bucket ()
            result.BucketName <- get.Required.Field "bucket_name" D.string
            result.CreationDate <- get.Required.Field "creation_date" D.dateTime
            result
        )
    static member JsonSpec = FieldSpec.Create<S3Bucket> (S3Bucket.JsonEncoder, S3Bucket.JsonDecoder)
    member this.ToJson () = S3Bucket.JsonEncoder this

let connect (token : AwsToken) (config : S3Config option) : S3Client =
    match config with
    | Some config ->
        new S3Client (token.Key, token.Secret, config)
    | None ->
        new S3Client (token.Key, token.Secret)

type S3 (token : AwsToken, ?config : S3Config) =
    let client = connect token config
    member __.Client = client
    member __.ListBucketsAsync
            (?req : ListBucketsRequest, ?cancellationToken : CancellationToken)
            : Task<ListBucketsResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        match req with
        | Some req ->
            client.ListBucketsAsync (req, cancellationToken)
        | None ->
            client.ListBucketsAsync (cancellationToken)
    member __.PutBucketAsync
            (?bucket : string, ?req : PutBucketRequest, ?cancellationToken : CancellationToken)
            : Task<PutBucketResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        match req with
        | Some req ->
            client.PutBucketAsync (req, cancellationToken)
        | None ->
            client.PutBucketAsync (bucket.Value, cancellationToken)
    member __.PutObjectAsync
            (?bucket : string, ?key : string,
                ?contentBody : string, ?contentType : string, ?filePath : string,
                ?updateReq : PutObjectRequest -> PutObjectRequest,
                ?req : PutObjectRequest, ?cancellationToken : CancellationToken)
            : Task<PutObjectResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        req
        |> Option.defaultWith (fun () ->
            let req = new PutObjectRequest ()
            req.BucketName <- bucket.Value
            req.Key <- key.Value
            contentBody |> Option.iter (fun x -> req.ContentBody <- x)
            contentType |> Option.iter (fun x -> req.ContentType <- x)
            filePath |> Option.iter (fun x -> req.FilePath <- x)
            (defaultArg updateReq id) req
        )|> fun req -> client.PutObjectAsync (req, cancellationToken)
    member __.GetObjectAsync
            (?bucket : string, ?key : string,
                ?updateReq : GetObjectRequest -> GetObjectRequest,
                ?req : GetObjectRequest, ?cancellationToken : CancellationToken)
            : Task<GetObjectResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        req
        |> Option.defaultWith (fun () ->
            let req = new GetObjectRequest ()
            req.BucketName <- bucket.Value
            req.Key <- key.Value
            (defaultArg updateReq id) req
        )|> fun req -> client.GetObjectAsync (req, cancellationToken)
    member __.ListObjectsAsync
            (?bucket : string, ?prefix : string, ?marker : string, ?maxKeys : int,
                ?updateReq : ListObjectsRequest -> ListObjectsRequest,
                ?req : ListObjectsRequest, ?cancellationToken : CancellationToken)
            : Task<ListObjectsResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        req
        |> Option.defaultWith (fun () ->
            let req = new ListObjectsRequest ()
            req.BucketName <- bucket.Value
            prefix |> Option.iter (fun x -> req.Prefix <- x)
            marker |> Option.iter (fun x -> req.Marker <- x)
            maxKeys |> Option.iter (fun x -> req.MaxKeys <- x)
            (defaultArg updateReq id) req
        )|> fun req -> client.ListObjectsAsync (req, cancellationToken)
    member __.DeleteObjectAsync
            (?bucket : string, ?key : string, ?versionId : string,
                ?updateReq : DeleteObjectRequest -> DeleteObjectRequest,
                ?req : DeleteObjectRequest, ?cancellationToken : CancellationToken)
            : Task<DeleteObjectResponse> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None
        req
        |> Option.defaultWith (fun () ->
            let req = new DeleteObjectRequest ()
            req.BucketName <- bucket.Value
            req.Key <- key.Value
            versionId |> Option.iter (fun x -> req.VersionId <- x)
            (defaultArg updateReq id) req
        )|> fun req -> client.DeleteObjectAsync (req, cancellationToken)


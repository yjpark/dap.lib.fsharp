[<AutoOpen>]
module Dap.Remote.Squidex.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type SquidexConfig = {
    Url : (* SquidexConfig *) string
    App : (* SquidexConfig *) string
    Token : (* SquidexConfig *) string
    Languages : (* SquidexConfig *) string list
} with
    static member Create
        (
            ?url : (* SquidexConfig *) string,
            ?app : (* SquidexConfig *) string,
            ?token : (* SquidexConfig *) string,
            ?languages : (* SquidexConfig *) string list
        ) : SquidexConfig =
        {
            Url = (* SquidexConfig *) url
                |> Option.defaultWith (fun () -> "")
            App = (* SquidexConfig *) app
                |> Option.defaultWith (fun () -> "")
            Token = (* SquidexConfig *) token
                |> Option.defaultWith (fun () -> "")
            Languages = (* SquidexConfig *) languages
                |> Option.defaultWith (fun () -> [])
        }
    static member SetUrl ((* SquidexConfig *) url : string) (this : SquidexConfig) =
        {this with Url = url}
    static member SetApp ((* SquidexConfig *) app : string) (this : SquidexConfig) =
        {this with App = app}
    static member SetToken ((* SquidexConfig *) token : string) (this : SquidexConfig) =
        {this with Token = token}
    static member SetLanguages ((* SquidexConfig *) languages : string list) (this : SquidexConfig) =
        {this with Languages = languages}
    static member JsonEncoder : JsonEncoder<SquidexConfig> =
        fun (this : SquidexConfig) ->
            E.object [
                "url", E.string (* SquidexConfig *) this.Url
                "app", E.string (* SquidexConfig *) this.App
                "token", E.string (* SquidexConfig *) this.Token
                "languages", (E.list E.string) (* SquidexConfig *) this.Languages
            ]
    static member JsonDecoder : JsonDecoder<SquidexConfig> =
        D.object (fun get ->
            {
                Url = get.Required.Field (* SquidexConfig *) "url" D.string
                App = get.Required.Field (* SquidexConfig *) "app" D.string
                Token = get.Required.Field (* SquidexConfig *) "token" D.string
                Languages = get.Required.Field (* SquidexConfig *) "languages" (D.list D.string)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<SquidexConfig> (SquidexConfig.JsonEncoder, SquidexConfig.JsonDecoder)
    interface IJson with
        member this.ToJson () = SquidexConfig.JsonEncoder this
    interface IObj
    member this.WithUrl ((* SquidexConfig *) url : string) =
        this |> SquidexConfig.SetUrl url
    member this.WithApp ((* SquidexConfig *) app : string) =
        this |> SquidexConfig.SetApp app
    member this.WithToken ((* SquidexConfig *) token : string) =
        this |> SquidexConfig.SetToken token
    member this.WithLanguages ((* SquidexConfig *) languages : string list) =
        this |> SquidexConfig.SetLanguages languages

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type SquidexItem = {
    Id : (* SquidexItem *) string
    Version : (* SquidexItem *) int
    Created : (* SquidexItem *) System.DateTime
    CreatedBy : (* SquidexItem *) string
    LastModified : (* SquidexItem *) System.DateTime
    LastModifiedBy : (* SquidexItem *) string
    Url : (* SquidexItem *) string
    Data : (* SquidexItem *) Json
    DataFlatten : (* SquidexItem *) Json
} with
    static member Create
        (
            ?id : (* SquidexItem *) string,
            ?version : (* SquidexItem *) int,
            ?created : (* SquidexItem *) System.DateTime,
            ?createdBy : (* SquidexItem *) string,
            ?lastModified : (* SquidexItem *) System.DateTime,
            ?lastModifiedBy : (* SquidexItem *) string,
            ?url : (* SquidexItem *) string,
            ?data : (* SquidexItem *) Json,
            ?dataFlatten : (* SquidexItem *) Json
        ) : SquidexItem =
        {
            Id = (* SquidexItem *) id
                |> Option.defaultWith (fun () -> "")
            Version = (* SquidexItem *) version
                |> Option.defaultWith (fun () -> 0)
            Created = (* SquidexItem *) created
                |> Option.defaultWith (fun () -> System.DateTime.UtcNow)
            CreatedBy = (* SquidexItem *) createdBy
                |> Option.defaultWith (fun () -> "")
            LastModified = (* SquidexItem *) lastModified
                |> Option.defaultWith (fun () -> System.DateTime.UtcNow)
            LastModifiedBy = (* SquidexItem *) lastModifiedBy
                |> Option.defaultWith (fun () -> "")
            Url = (* SquidexItem *) url
                |> Option.defaultWith (fun () -> "")
            Data = (* SquidexItem *) data
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
            DataFlatten = (* SquidexItem *) dataFlatten
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
        }
    static member SetId ((* SquidexItem *) id : string) (this : SquidexItem) =
        {this with Id = id}
    static member SetVersion ((* SquidexItem *) version : int) (this : SquidexItem) =
        {this with Version = version}
    static member SetCreated ((* SquidexItem *) created : System.DateTime) (this : SquidexItem) =
        {this with Created = created}
    static member SetCreatedBy ((* SquidexItem *) createdBy : string) (this : SquidexItem) =
        {this with CreatedBy = createdBy}
    static member SetLastModified ((* SquidexItem *) lastModified : System.DateTime) (this : SquidexItem) =
        {this with LastModified = lastModified}
    static member SetLastModifiedBy ((* SquidexItem *) lastModifiedBy : string) (this : SquidexItem) =
        {this with LastModifiedBy = lastModifiedBy}
    static member SetUrl ((* SquidexItem *) url : string) (this : SquidexItem) =
        {this with Url = url}
    static member SetData ((* SquidexItem *) data : Json) (this : SquidexItem) =
        {this with Data = data}
    static member SetDataFlatten ((* SquidexItem *) dataFlatten : Json) (this : SquidexItem) =
        {this with DataFlatten = dataFlatten}
    static member JsonEncoder : JsonEncoder<SquidexItem> =
        fun (this : SquidexItem) ->
            E.object [
                "id", E.string (* SquidexItem *) this.Id
                "version", E.int (* SquidexItem *) this.Version
                "created", E.dateTime (* SquidexItem *) this.Created
                "createdBy", E.string (* SquidexItem *) this.CreatedBy
                "lastModified", E.dateTime (* SquidexItem *) this.LastModified
                "lastModifiedBy", E.string (* SquidexItem *) this.LastModifiedBy
                "url", E.string (* SquidexItem *) this.Url
                "data", E.json (* SquidexItem *) this.Data
                "dataFlatten", E.json (* SquidexItem *) this.DataFlatten
            ]
    static member JsonDecoder : JsonDecoder<SquidexItem> =
        D.object (fun get ->
            {
                Id = get.Optional.Field (* SquidexItem *) "id" D.string
                    |> Option.defaultValue ""
                Version = get.Optional.Field (* SquidexItem *) "version" D.int
                    |> Option.defaultValue 0
                Created = get.Optional.Field (* SquidexItem *) "created" D.dateTime
                    |> Option.defaultValue System.DateTime.UtcNow
                CreatedBy = get.Optional.Field (* SquidexItem *) "createdBy" D.string
                    |> Option.defaultValue ""
                LastModified = get.Optional.Field (* SquidexItem *) "lastModified" D.dateTime
                    |> Option.defaultValue System.DateTime.UtcNow
                LastModifiedBy = get.Optional.Field (* SquidexItem *) "lastModifiedBy" D.string
                    |> Option.defaultValue ""
                Url = get.Optional.Field (* SquidexItem *) "url" D.string
                    |> Option.defaultValue ""
                Data = get.Optional.Field (* SquidexItem *) "data" D.json
                    |> Option.defaultValue (decodeJsonValue D.json """null""")
                DataFlatten = get.Optional.Field (* SquidexItem *) "dataFlatten" D.json
                    |> Option.defaultValue (decodeJsonValue D.json """null""")
            }
        )
    static member JsonSpec =
        FieldSpec.Create<SquidexItem> (SquidexItem.JsonEncoder, SquidexItem.JsonDecoder)
    interface IJson with
        member this.ToJson () = SquidexItem.JsonEncoder this
    interface IObj
    member this.WithId ((* SquidexItem *) id : string) =
        this |> SquidexItem.SetId id
    member this.WithVersion ((* SquidexItem *) version : int) =
        this |> SquidexItem.SetVersion version
    member this.WithCreated ((* SquidexItem *) created : System.DateTime) =
        this |> SquidexItem.SetCreated created
    member this.WithCreatedBy ((* SquidexItem *) createdBy : string) =
        this |> SquidexItem.SetCreatedBy createdBy
    member this.WithLastModified ((* SquidexItem *) lastModified : System.DateTime) =
        this |> SquidexItem.SetLastModified lastModified
    member this.WithLastModifiedBy ((* SquidexItem *) lastModifiedBy : string) =
        this |> SquidexItem.SetLastModifiedBy lastModifiedBy
    member this.WithUrl ((* SquidexItem *) url : string) =
        this |> SquidexItem.SetUrl url
    member this.WithData ((* SquidexItem *) data : Json) =
        this |> SquidexItem.SetData data
    member this.WithDataFlatten ((* SquidexItem *) dataFlatten : Json) =
        this |> SquidexItem.SetDataFlatten dataFlatten
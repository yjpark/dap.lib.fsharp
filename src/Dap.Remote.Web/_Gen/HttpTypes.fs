[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Web.HttpTypes

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type InvalidBody = {
    Url : (* InvalidBody *) string
    Body : (* InvalidBody *) string
    Error : (* InvalidBody *) string
    Samples : (* InvalidBody *) Json list
} with
    static member Create
        (
            ?url : (* InvalidBody *) string,
            ?body : (* InvalidBody *) string,
            ?error : (* InvalidBody *) string,
            ?samples : (* InvalidBody *) Json list
        ) : InvalidBody =
        {
            Url = (* InvalidBody *) url
                |> Option.defaultWith (fun () -> "")
            Body = (* InvalidBody *) body
                |> Option.defaultWith (fun () -> "")
            Error = (* InvalidBody *) error
                |> Option.defaultWith (fun () -> "")
            Samples = (* InvalidBody *) samples
                |> Option.defaultWith (fun () -> [])
        }
    static member SetUrl ((* InvalidBody *) url : string) (this : InvalidBody) =
        {this with Url = url}
    static member SetBody ((* InvalidBody *) body : string) (this : InvalidBody) =
        {this with Body = body}
    static member SetError ((* InvalidBody *) error : string) (this : InvalidBody) =
        {this with Error = error}
    static member SetSamples ((* InvalidBody *) samples : Json list) (this : InvalidBody) =
        {this with Samples = samples}
    static member JsonEncoder : JsonEncoder<InvalidBody> =
        fun (this : InvalidBody) ->
            E.object [
                "url", E.string (* InvalidBody *) this.Url
                "body", E.string (* InvalidBody *) this.Body
                "error", E.string (* InvalidBody *) this.Error
                "samples", (E.list E.json) (* InvalidBody *) this.Samples
            ]
    static member JsonDecoder : JsonDecoder<InvalidBody> =
        D.object (fun get ->
            {
                Url = get.Required.Field (* InvalidBody *) "url" D.string
                Body = get.Required.Field (* InvalidBody *) "body" D.string
                Error = get.Required.Field (* InvalidBody *) "error" D.string
                Samples = get.Required.Field (* InvalidBody *) "samples" (D.list D.json)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<InvalidBody> (InvalidBody.JsonEncoder, InvalidBody.JsonDecoder)
    interface IJson with
        member this.ToJson () = InvalidBody.JsonEncoder this
    interface IObj
    member this.WithUrl ((* InvalidBody *) url : string) =
        this |> InvalidBody.SetUrl url
    member this.WithBody ((* InvalidBody *) body : string) =
        this |> InvalidBody.SetBody body
    member this.WithError ((* InvalidBody *) error : string) =
        this |> InvalidBody.SetError error
    member this.WithSamples ((* InvalidBody *) samples : Json list) =
        this |> InvalidBody.SetSamples samples

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type OpenIdConnectConfig = {
    ClientId : (* OpenIdConnectConfig *) string
    ClientSecret : (* OpenIdConnectConfig *) string
    Authority : (* OpenIdConnectConfig *) string
    CallbackPath : (* OpenIdConnectConfig *) string
    ResponseType : (* OpenIdConnectConfig *) string
    SaveTokens : (* OpenIdConnectConfig *) bool
    GetClaimsFromUserInfoEndpoint : (* OpenIdConnectConfig *) bool
    RequireHttpsMetadata : (* OpenIdConnectConfig *) bool
    NameClaimType : (* OpenIdConnectConfig *) string
    ClaimsIssuer : (* OpenIdConnectConfig *) string option
} with
    static member Create
        (
            ?clientId : (* OpenIdConnectConfig *) string,
            ?clientSecret : (* OpenIdConnectConfig *) string,
            ?authority : (* OpenIdConnectConfig *) string,
            ?callbackPath : (* OpenIdConnectConfig *) string,
            ?responseType : (* OpenIdConnectConfig *) string,
            ?saveTokens : (* OpenIdConnectConfig *) bool,
            ?getClaimsFromUserInfoEndpoint : (* OpenIdConnectConfig *) bool,
            ?requireHttpsMetadata : (* OpenIdConnectConfig *) bool,
            ?nameClaimType : (* OpenIdConnectConfig *) string,
            ?claimsIssuer : (* OpenIdConnectConfig *) string
        ) : OpenIdConnectConfig =
        {
            ClientId = (* OpenIdConnectConfig *) clientId
                |> Option.defaultWith (fun () -> "")
            ClientSecret = (* OpenIdConnectConfig *) clientSecret
                |> Option.defaultWith (fun () -> "")
            Authority = (* OpenIdConnectConfig *) authority
                |> Option.defaultWith (fun () -> "")
            CallbackPath = (* OpenIdConnectConfig *) callbackPath
                |> Option.defaultWith (fun () -> "/signin-oidc")
            ResponseType = (* OpenIdConnectConfig *) responseType
                |> Option.defaultWith (fun () -> "code")
            SaveTokens = (* OpenIdConnectConfig *) saveTokens
                |> Option.defaultWith (fun () -> true)
            GetClaimsFromUserInfoEndpoint = (* OpenIdConnectConfig *) getClaimsFromUserInfoEndpoint
                |> Option.defaultWith (fun () -> true)
            RequireHttpsMetadata = (* OpenIdConnectConfig *) requireHttpsMetadata
                |> Option.defaultWith (fun () -> false)
            NameClaimType = (* OpenIdConnectConfig *) nameClaimType
                |> Option.defaultWith (fun () -> "name")
            ClaimsIssuer = (* OpenIdConnectConfig *) claimsIssuer
        }
    static member SetClientId ((* OpenIdConnectConfig *) clientId : string) (this : OpenIdConnectConfig) =
        {this with ClientId = clientId}
    static member SetClientSecret ((* OpenIdConnectConfig *) clientSecret : string) (this : OpenIdConnectConfig) =
        {this with ClientSecret = clientSecret}
    static member SetAuthority ((* OpenIdConnectConfig *) authority : string) (this : OpenIdConnectConfig) =
        {this with Authority = authority}
    static member SetCallbackPath ((* OpenIdConnectConfig *) callbackPath : string) (this : OpenIdConnectConfig) =
        {this with CallbackPath = callbackPath}
    static member SetResponseType ((* OpenIdConnectConfig *) responseType : string) (this : OpenIdConnectConfig) =
        {this with ResponseType = responseType}
    static member SetSaveTokens ((* OpenIdConnectConfig *) saveTokens : bool) (this : OpenIdConnectConfig) =
        {this with SaveTokens = saveTokens}
    static member SetGetClaimsFromUserInfoEndpoint ((* OpenIdConnectConfig *) getClaimsFromUserInfoEndpoint : bool) (this : OpenIdConnectConfig) =
        {this with GetClaimsFromUserInfoEndpoint = getClaimsFromUserInfoEndpoint}
    static member SetRequireHttpsMetadata ((* OpenIdConnectConfig *) requireHttpsMetadata : bool) (this : OpenIdConnectConfig) =
        {this with RequireHttpsMetadata = requireHttpsMetadata}
    static member SetNameClaimType ((* OpenIdConnectConfig *) nameClaimType : string) (this : OpenIdConnectConfig) =
        {this with NameClaimType = nameClaimType}
    static member SetClaimsIssuer ((* OpenIdConnectConfig *) claimsIssuer : string option) (this : OpenIdConnectConfig) =
        {this with ClaimsIssuer = claimsIssuer}
    static member JsonEncoder : JsonEncoder<OpenIdConnectConfig> =
        fun (this : OpenIdConnectConfig) ->
            E.object [
                "ClientId", E.string (* OpenIdConnectConfig *) this.ClientId
                "ClientSecret", E.string (* OpenIdConnectConfig *) this.ClientSecret
                "Authority", E.string (* OpenIdConnectConfig *) this.Authority
                "CallbackPath", E.string (* OpenIdConnectConfig *) this.CallbackPath
                "ResponseType", E.string (* OpenIdConnectConfig *) this.ResponseType
                "SaveTokens", E.bool (* OpenIdConnectConfig *) this.SaveTokens
                "GetClaimsFromUserInfoEndpoint", E.bool (* OpenIdConnectConfig *) this.GetClaimsFromUserInfoEndpoint
                "RequireHttpsMetadata", E.bool (* OpenIdConnectConfig *) this.RequireHttpsMetadata
                "NameClaimType", E.string (* OpenIdConnectConfig *) this.NameClaimType
                "ClaimsIssuer", (E.option E.string) (* OpenIdConnectConfig *) this.ClaimsIssuer
            ]
    static member JsonDecoder : JsonDecoder<OpenIdConnectConfig> =
        D.object (fun get ->
            {
                ClientId = get.Optional.Field (* OpenIdConnectConfig *) "ClientId" D.string
                    |> Option.defaultValue ""
                ClientSecret = get.Optional.Field (* OpenIdConnectConfig *) "ClientSecret" D.string
                    |> Option.defaultValue ""
                Authority = get.Optional.Field (* OpenIdConnectConfig *) "Authority" D.string
                    |> Option.defaultValue ""
                CallbackPath = get.Optional.Field (* OpenIdConnectConfig *) "CallbackPath" D.string
                    |> Option.defaultValue "/signin-oidc"
                ResponseType = get.Optional.Field (* OpenIdConnectConfig *) "ResponseType" D.string
                    |> Option.defaultValue "code"
                SaveTokens = get.Optional.Field (* OpenIdConnectConfig *) "SaveTokens" D.bool
                    |> Option.defaultValue true
                GetClaimsFromUserInfoEndpoint = get.Optional.Field (* OpenIdConnectConfig *) "GetClaimsFromUserInfoEndpoint" D.bool
                    |> Option.defaultValue true
                RequireHttpsMetadata = get.Optional.Field (* OpenIdConnectConfig *) "RequireHttpsMetadata" D.bool
                    |> Option.defaultValue false
                NameClaimType = get.Optional.Field (* OpenIdConnectConfig *) "NameClaimType" D.string
                    |> Option.defaultValue "name"
                ClaimsIssuer = get.Optional.Field (* OpenIdConnectConfig *) "ClaimsIssuer" D.string
            }
        )
    static member JsonSpec =
        FieldSpec.Create<OpenIdConnectConfig> (OpenIdConnectConfig.JsonEncoder, OpenIdConnectConfig.JsonDecoder)
    interface IJson with
        member this.ToJson () = OpenIdConnectConfig.JsonEncoder this
    interface IObj
    member this.WithClientId ((* OpenIdConnectConfig *) clientId : string) =
        this |> OpenIdConnectConfig.SetClientId clientId
    member this.WithClientSecret ((* OpenIdConnectConfig *) clientSecret : string) =
        this |> OpenIdConnectConfig.SetClientSecret clientSecret
    member this.WithAuthority ((* OpenIdConnectConfig *) authority : string) =
        this |> OpenIdConnectConfig.SetAuthority authority
    member this.WithCallbackPath ((* OpenIdConnectConfig *) callbackPath : string) =
        this |> OpenIdConnectConfig.SetCallbackPath callbackPath
    member this.WithResponseType ((* OpenIdConnectConfig *) responseType : string) =
        this |> OpenIdConnectConfig.SetResponseType responseType
    member this.WithSaveTokens ((* OpenIdConnectConfig *) saveTokens : bool) =
        this |> OpenIdConnectConfig.SetSaveTokens saveTokens
    member this.WithGetClaimsFromUserInfoEndpoint ((* OpenIdConnectConfig *) getClaimsFromUserInfoEndpoint : bool) =
        this |> OpenIdConnectConfig.SetGetClaimsFromUserInfoEndpoint getClaimsFromUserInfoEndpoint
    member this.WithRequireHttpsMetadata ((* OpenIdConnectConfig *) requireHttpsMetadata : bool) =
        this |> OpenIdConnectConfig.SetRequireHttpsMetadata requireHttpsMetadata
    member this.WithNameClaimType ((* OpenIdConnectConfig *) nameClaimType : string) =
        this |> OpenIdConnectConfig.SetNameClaimType nameClaimType
    member this.WithClaimsIssuer ((* OpenIdConnectConfig *) claimsIssuer : string option) =
        this |> OpenIdConnectConfig.SetClaimsIssuer claimsIssuer
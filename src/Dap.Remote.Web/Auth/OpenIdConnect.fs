[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.Web.Auth.OpenIdConnect

open System
open System.Threading
open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication.OpenIdConnect

open Giraffe

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote.Web

type AuthenticationScheme with
    static member JsonEncoder (this : AuthenticationScheme) =
        if this =? null then
            E.nil
        else
            E.object [
                "Name", E.string this.Name
                "DisplayName", E.string this.DisplayName
            ]
    member this.ToJson () =
        AuthenticationScheme.JsonEncoder this

let encodeStringDict (this : System.Collections.Generic.IDictionary<string, string>) =
    this
    |> List.ofSeq
    |> List.map (fun kv -> (kv.Key, E.string kv.Value))
    |> E.object

type AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties
type Microsoft.AspNetCore.Authentication.AuthenticationProperties with
    static member JsonEncoder (this : AuthenticationProperties) =
        if this =? null then
            E.nil
        else
            E.object [
                "RedirectUri", E.string this.RedirectUri
                "IssuedUtc", E.string (sprintf "%A" this.IssuedUtc)
                "ExpiresUtc", E.string (sprintf "%A" this.ExpiresUtc)
                "IsPersistent", E.bool this.IsPersistent
                //"AllowRefresh", E.bool this.AllowRefresh
                "Items", encodeStringDict this.Items
            ]
    member this.ToJson () =
        AuthenticationProperties.JsonEncoder this

type Claim = System.Security.Claims.Claim
type System.Security.Claims.Claim with
    static member JsonEncoder (this : Claim) =
        if this =? null then
            E.nil
        else
            E.object [
                "Issuer", E.string this.Issuer
                "OriginalIssuer", E.string this.Issuer
                "Subject", E.string this.Subject.Name
                "Type", this.Type.ToJson ()
                "Value", this.Value.ToJson ()
                "ValueType", this.ValueType.ToJson ()
                "Properties", encodeStringDict this.Properties
            ]
    member this.ToJson () =
        Claim.JsonEncoder this

type ClaimsIdentity = System.Security.Claims.ClaimsIdentity
type System.Security.Claims.ClaimsIdentity with
    static member JsonEncoder (this : ClaimsIdentity) =
        if this =? null then
            E.nil
        else
            E.object [
                "Name", E.string this.Name
                "IsAuthenticated", E.bool this.IsAuthenticated
                "Label", E.string this.Label
                "Claims", (E.list Claim.JsonEncoder) (List.ofSeq this.Claims)
                "NameClaimType", E.string this.NameClaimType
                "RoleClaimType", E.string this.RoleClaimType
            ]
    member this.ToJson () =
        ClaimsIdentity.JsonEncoder this

type ClaimsPrincipal = System.Security.Claims.ClaimsPrincipal
type System.Security.Claims.ClaimsPrincipal with
    static member JsonEncoder (this : ClaimsPrincipal) =
        if this =? null then
            E.nil
        else
            E.object [
                //"Identity", ClaimsIdentity.JsonEncoder this.Identity
                "Identities", (E.list ClaimsIdentity.JsonEncoder) (List.ofSeq this.Identities)
                "Claims", (E.list Claim.JsonEncoder) (List.ofSeq this.Claims)
            ]
    member this.ToJson () =
        ClaimsPrincipal.JsonEncoder this

let encodeRemoteAuthenticationContext<'options when 'options :> AuthenticationSchemeOptions> (this : RemoteAuthenticationContext<'options>) =
    [
        "Scheme", this.Scheme.ToJson ()
        "Principal", this.Principal.ToJson ()
        "Properties", this.Properties.ToJson ()
    ]

type AuthenticationFailedContext with
    static member JsonEncoder (this : AuthenticationFailedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
                "Exception", E.string (sprintf "%A" this.Exception)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        AuthenticationFailedContext.JsonEncoder this

type AuthorizationCodeReceivedContext with
    static member JsonEncoder (this : AuthorizationCodeReceivedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
                "JwtSecurityToken", E.string (sprintf "%A" this.JwtSecurityToken)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        AuthorizationCodeReceivedContext.JsonEncoder this

type MessageReceivedContext with
    static member JsonEncoder (this : MessageReceivedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "Token", E.string this.Token
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        MessageReceivedContext.JsonEncoder this

type RedirectContext with
    static member JsonEncoder (this : RedirectContext) =
        if this =? null then
            E.nil
        else
            E.object [
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
            ]
    member this.ToJson () =
        RedirectContext.JsonEncoder this

type RemoteFailureContext with
    static member JsonEncoder (this : RemoteFailureContext) =
        if this =? null then
            E.nil
        else
            E.object [
                "Failure", E.string (sprintf "%A" this.Failure)
            ]
    member this.ToJson () =
        RemoteFailureContext.JsonEncoder this

type RemoteSignOutContext with
    static member JsonEncoder (this : RemoteSignOutContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        RemoteSignOutContext.JsonEncoder this

type TicketReceivedContext with
    static member JsonEncoder (this : TicketReceivedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ReturnUri", E.string (sprintf "%A" this.ReturnUri)
                "Options", E.string (sprintf "%A" this.Options)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        TicketReceivedContext.JsonEncoder this

type TokenResponseReceivedContext with
    static member JsonEncoder (this : TokenResponseReceivedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        TokenResponseReceivedContext.JsonEncoder this

type TokenValidatedContext with
    static member JsonEncoder (this : TokenValidatedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
                "ProtocolMessage", E.string (sprintf "%A" this.ProtocolMessage)
                "SecurityToken", E.string (sprintf "%A" this.SecurityToken)
                "Nonce", E.string (sprintf "%A" this.Nonce)
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        TokenValidatedContext.JsonEncoder this

type UserInformationReceivedContext with
    static member JsonEncoder (this : UserInformationReceivedContext) =
        if this =? null then
            E.nil
        else
            E.object ([
            ] @ encodeRemoteAuthenticationContext (this))
    member this.ToJson () =
        UserInformationReceivedContext.JsonEncoder this

type OpenIdConnectOptions with
    member this.SetupEvents
        (
            logger : ILogger,
            ?logLevel : LogLevel,
            ?onAuthenticationFailed : AuthenticationFailedContext -> Task<unit>,
            ?onAuthorizationCodeReceived : AuthorizationCodeReceivedContext -> Task<unit>,
            ?onMessageReceived : MessageReceivedContext -> Task<unit>,
            ?onRedirectToIdentityProvider : RedirectContext -> Task<unit>,
            ?onRedirectToIdentityProviderForSignOut : RedirectContext -> Task<unit>,
            ?onRemoteFailure : RemoteFailureContext -> Task<unit>,
            ?onRemoteSignOut : RemoteSignOutContext -> Task<unit>,
            ?onSignedOutCallbackRedirect : RemoteSignOutContext -> Task<unit>,
            ?onTicketReceived : TicketReceivedContext -> Task<unit>,
            ?onTokenResponseReceived : TokenResponseReceivedContext -> Task<unit>,
            ?onTokenValidated : TokenValidatedContext -> Task<unit>,
            ?onUserInformationReceived : UserInformationReceivedContext -> Task<unit>
        ) =
        let logLevel = defaultArg logLevel LogLevelDebug
        let events = new OpenIdConnectEvents ()
        events.OnAuthenticationFailed <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnAuthenticationFailed" (E.encode 4 <| ctx.ToJson ())
            match onAuthenticationFailed with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnAuthorizationCodeReceived <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnAuthorizationCodeReceived" (E.encode 4 <| ctx.ToJson ())
            match onAuthorizationCodeReceived with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnMessageReceived <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnMessageReceived" (E.encode 4 <| ctx.ToJson ())
            match onMessageReceived with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnRedirectToIdentityProvider <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnRedirectToIdentityProvider" (E.encode 4 <| ctx.ToJson ())
            match onRedirectToIdentityProvider with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnRedirectToIdentityProviderForSignOut <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnRedirectToIdentityProviderForSignOut" (E.encode 4 <| ctx.ToJson ())
            match onRedirectToIdentityProviderForSignOut with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnRemoteFailure <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnRemoteFailure" (E.encode 4 <| ctx.ToJson ())
            match onRemoteFailure with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnRemoteSignOut <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnRemoteSignOut" (E.encode 4 <| ctx.ToJson ())
            match onRemoteSignOut with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnSignedOutCallbackRedirect <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnSignedOutCallbackRedirect" (E.encode 4 <| ctx.ToJson ())
            match onSignedOutCallbackRedirect with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnTicketReceived <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnTicketReceived" (E.encode 4 <| ctx.ToJson ())
            match onTicketReceived with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnTokenResponseReceived <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnTokenResponseReceived" (E.encode 4 <| ctx.ToJson ())
            match onTokenResponseReceived with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnTokenValidated <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnTokenValidated" (E.encode 4 <| ctx.ToJson ())
            match onTokenValidated with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        events.OnUserInformationReceived <- fun ctx ->
            log logger logLevel "OpenIdConnect" "OnUserInformationReceived" (E.encode 4 <| ctx.ToJson ())
            match onUserInformationReceived with
            | Some onEvent -> onEvent ctx :> Task
            | None -> Task.CompletedTask
        this.Events <- events
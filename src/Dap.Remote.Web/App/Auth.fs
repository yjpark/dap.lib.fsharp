[<AutoOpen>]
module Dap.Remote.Web.App.Auth

open System
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication.OpenIdConnect

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Local
open Dap.Remote.Web

type OpenIdConnectConfig with
    member this.Apply (options : OpenIdConnectOptions) =
        options.ClientId <- this.ClientId
        options.ClientSecret <- this.ClientSecret
        options.Authority <- this.Authority
        options.CallbackPath <- PathString (this.CallbackPath)
        options.ResponseType <- this.ResponseType
        options.SaveTokens <- this.SaveTokens
        options.GetClaimsFromUserInfoEndpoint <- this.GetClaimsFromUserInfoEndpoint
        options.RequireHttpsMetadata <- this.RequireHttpsMetadata
        options.TokenValidationParameters.NameClaimType <- this.NameClaimType
        this.ClaimsIssuer
        |> Option.iter (fun x ->
            options.ClaimsIssuer <- x
        )

type WebHost with
    static member setupAuth
            (
                options : AuthenticationOptions -> unit,
                actions : (AuthenticationBuilder -> AuthenticationBuilder) list
            ) =
        WebHost.setupService (fun service ->
            let auth : AuthenticationBuilder =
                service.AddAuthentication (
                    Action<AuthenticationOptions> (options)
                )
            actions
            |> List.iter (fun action ->
                action auth |> ignore
            )
        )>> WebHost.setupApp (fun x -> x.UseAuthentication ())
    static member setupAuthWithOpenIdConnectAndCookie
            (
                openIdConnectOptions : OpenIdConnectOptions -> unit,
                ?cookieOptions : CookieAuthenticationOptions -> unit
            ) =
        WebHost.setupAuth (
            (fun (options : AuthenticationOptions) ->
                options.DefaultAuthenticateScheme <- CookieAuthenticationDefaults.AuthenticationScheme
                options.DefaultChallengeScheme <- OpenIdConnectDefaults.AuthenticationScheme
                options.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme
            ), [
                yield (fun (auth : AuthenticationBuilder) ->
                    auth.AddOpenIdConnect (
                        Action<OpenIdConnectOptions> (openIdConnectOptions)
                    )
                )
                yield (fun (auth : AuthenticationBuilder) ->
                    auth.AddCookie (
                        Action<CookieAuthenticationOptions> (
                            cookieOptions
                            |> Option.defaultValue ignore
                    ))
                )
            ]
        )
    static member setupAuthWithOpenIdConnectAndCookieFromConfig
            (
                getConfig : unit -> IConfiguration,
                ?section : string,
                ?nameClaimType : string,
                ?extraOptions : OpenIdConnectOptions -> unit,
                ?cookieOptions : CookieAuthenticationOptions -> unit
            ) =
        let section = defaultArg section "OpenIdConnect"
        let nameClaimType = defaultArg nameClaimType "name"
        WebHost.setupAuthWithOpenIdConnectAndCookie (
            openIdConnectOptions = (fun options ->
                let config = getConfig ()
                config.GetSection(section).Bind (options)
                options.TokenValidationParameters.NameClaimType <- nameClaimType
                extraOptions
                |> Option.iter (fun extra -> extra options)
            ),
            ?cookieOptions = cookieOptions
        )

    static member setupAuthWithOpenIdConnectAndCookieFromPreferences
            (
                luid : Luid,
                ?extraOptions : OpenIdConnectOptions -> unit,
                ?cookieOptions : CookieAuthenticationOptions -> unit
            ) =
        let config = IEnvironment.Instance.Preferences.Get.Handle luid
        if config.IsNone then
            failWith "OpenIdConnectConfig_Not_Found" luid
        let config = decodeJson OpenIdConnectConfig.JsonDecoder config.Value
        WebHost.setupAuthWithOpenIdConnectAndCookie (
            openIdConnectOptions = (fun options ->
                config.Apply (options)
                extraOptions
                |> Option.iter (fun extra -> extra options)
            ),
            ?cookieOptions = cookieOptions
        )

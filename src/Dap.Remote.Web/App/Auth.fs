[<AutoOpen>]
module Dap.Remote.Web.App.Auth

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication.OpenIdConnect

open Dap.Prelude
open Dap.Platform

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
        )
    static member setupOpenIdConnectAndCookie
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
    static member setupOpenIdConnectAndCookieFromConfig
            (
                getConfig : unit -> IConfiguration,
                ?section : string,
                ?nameClaimType : string,
                ?extraOptions : OpenIdConnectOptions -> unit,
                ?cookieOptions : CookieAuthenticationOptions -> unit
            ) =
        let section = defaultArg section "OpenIdConnect"
        let nameClaimType = defaultArg nameClaimType "name"
        WebHost.setupOpenIdConnectAndCookie (
            openIdConnectOptions = (fun options ->
                let config = getConfig ()
                config.GetSection(section).Bind (options)
                options.TokenValidationParameters.NameClaimType <- nameClaimType
                extraOptions
                |> Option.iter (fun extra -> extra options)
            ),
            ?cookieOptions = cookieOptions
        )

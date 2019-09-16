module Dap.Remote.Web.Dsl

open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs

let InvalidBody =
    combo {
        var (M.string "url")
        var (M.string "body")
        var (M.string "error")
        list (M.json "samples")
    }

let OpenIdConnectConfig =
    combo {
        var (M.string "ClientId")
        var (M.string "ClientSecret")
        var (M.string "Authority")
        var (M.string ("CallbackPath", "/signin-oidc"))
        var (M.string ("ResponseType", "code"))
        var (M.bool ("SaveTokens", true))
        var (M.bool ("GetClaimsFromUserInfoEndpoint", true))
        var (M.bool ("RequireHttpsMetadata", false))
        var (M.string ("NameClaimType", "name"))
        option (M.string "ClaimsIssuer")
    }

let compile segments =
    [
        G.File (segments, ["_Gen" ; "HttpTypes.fs"],
            G.AutoOpenQualifiedModule ("Dap.Remote.Web.HttpTypes",
                [
                    G.PlatformOpens
                    G.JsonRecord <@ InvalidBody @>
                    G.LooseJsonRecord <@ OpenIdConnectConfig @>
                ]
            )
        )
    ]

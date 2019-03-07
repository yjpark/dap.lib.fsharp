[<AutoOpen>]
module Dap.Remote.Web.App.Helper

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Dap.Prelude
open Dap.Platform

let runWebApp<'app when 'app :> IBaseApp> (newWebHost : 'app -> WebHost) (app : 'app) =
    let webHost = newWebHost app
    let host =
        new WebHostBuilder ()
        |> WebHostBuilderKestrelExtensions.UseKestrel
        |> webHost.Build
    WebApp.WebApp<'app>.Run host app

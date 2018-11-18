[<AutoOpen>]
module Dap.Remote.Web.WebPack

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Dap.Prelude
open Dap.Platform

type WebPack<'app when 'app :> IPack and 'app :> INeedSetupAsync>
        (
            app : 'app,
            host : WebHost
        ) =
    let webHost =
        new WebHostBuilder ()
        |> WebHostBuilderKestrelExtensions.UseKestrel
        |> host.Build
    member __.Run () =
        Feature.tryStartApp app
        logWarn app "WebPack" "WebHost_Running" webHost
        webHost.Run ()
        logWarn app "WebPack" "WebHost_Quit" webHost
    member __.App = app
    member __.WebHost = webHost
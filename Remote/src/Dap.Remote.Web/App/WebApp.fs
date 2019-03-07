[<RequireQualifiedAccess>]
module Dap.Remote.Web.App.WebApp

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

open Dap.Prelude
open Dap.Platform

type WebApp<'app when 'app :> IBaseApp> (host : IWebHost, app : 'app) =
    member private __.Run' () =
        logWarn app "WebApp.Run" "Starting" (host)
        Feature.tryStartApp app
        host.Run ()
        logWarn app "WebApp.Run" "Quitted" (host)
    member __.App = app
    member __.Host = host
    static member Run<'app when 'app :> IBaseApp> h a =
        let webApp = new WebApp<'app> (h, a)
        webApp.Run' ()
        0
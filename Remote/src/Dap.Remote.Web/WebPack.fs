[<AutoOpen>]
module Dap.Remote.Web.WebPack

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Dap.Prelude
open Dap.Platform
open Dap.Remote.AspNetCore.WebSocketHub

type WebPack<'app when 'app :> IPack and 'app :> INeedSetupAsync>
        (
            app : 'app,
            actions : (IWebHostBuilder -> unit) list,
            ?configureServices : IServiceCollection -> unit
        ) =
    let webHost =
        new WebHostBuilder ()
        |> WebHostBuilderKestrelExtensions.UseKestrel
        |> fun b ->
            actions
            |> List.iter (fun action -> action b)
            b
        |> fun b ->
            let configureServices = defaultArg configureServices (fun services ->
                services.AddGiraffe () |> ignore
            )
            b.ConfigureServices configureServices
        |> fun b -> b.Build ()
    member __.Run () =
        Feature.tryStartApp app
        logWarn app "WebPack" "WebHost_Running" webHost
        webHost.Run ()
        logWarn app "WebPack" "WebHost_Quit" webHost
    member __.App = app
    member __.WebHost = webHost
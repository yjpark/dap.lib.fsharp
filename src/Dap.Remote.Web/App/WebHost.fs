[<AutoOpen>]
module Dap.Remote.Web.App.WebHost

open System
open System.IO

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Diagnostics
open Microsoft.AspNetCore.Diagnostics.Elm
open Microsoft.Extensions.DependencyInjection

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote.Web

type WebHostConfig =
    | HostConfig of (ILogger -> IWebHostBuilder -> unit)
    | AppConfig of (ILogger -> IApplicationBuilder -> unit)
    | ServiceConfig of (ILogger -> IServiceCollection -> unit)

type WebHost = {
    Root : string option
    Urls : string []
    DevMode : bool
    Configs : WebHostConfig list
} with
    member this.Build (logger : ILogger) (builder : IWebHostBuilder) =
        let logger = getLogger "WebHost"
        this.Root
        |> Option.iter (fun root ->
            builder.UseContentRoot (root) |> ignore
            builder.UseWebRoot (root) |> ignore
        )
        builder.UseUrls this.Urls |> ignore
        let mutable appActions = []
        let mutable serviceActions = []
        this.Configs
        |> List.iterBack (fun config ->
            match config with
            | HostConfig action ->
                action logger builder
            | AppConfig action ->
                appActions <- action :: appActions
            | ServiceConfig action ->
                serviceActions <- action :: serviceActions
        )
        // Notice: Should only call builder.Configure () once
        builder.Configure (Action<IApplicationBuilder> (fun app ->
            appActions
            |> List.iterBack (fun action ->
                action logger app
            )
        )) |> ignore
        // Notice: Should only call builder.ConfigureServices () once
        builder.ConfigureServices (Action<IServiceCollection> (fun services ->
            serviceActions
            |> List.iterBack (fun action ->
                action logger services
            )
        )) |> ignore
        builder.Build ()
    static member empty =
        {
            Root = None
            Urls = [| |]
            DevMode = false
            Configs = []
        }

type WebHost with
    static member setUrls (urls : string []) =
        fun (config : WebHost) -> {config with Urls = urls}
    static member setUrl (url : string) =
        WebHost.setUrls [| url |]
    static member setPort (port : int) =
        sprintf "http://0.0.0.0:%d" port
        |> WebHost.setUrl
    static member setup (config : WebHostConfig) =
        fun (this : WebHost) ->
            {this with Configs = config :: this.Configs}

let private getMsg (action : obj) (msg : string option) =
    msg
    |> Option.defaultWith (fun () ->
        sprintf "%A" action
    )

type WebHost with
    static member setupHost (action : IWebHostBuilder -> unit, ?msg : string) =
        WebHost.setup <| HostConfig (fun (logger : ILogger) (host : IWebHostBuilder) ->
            logInfo logger "WebHost" "setupHost" (getMsg action msg)
            action host
        )
    static member setupApp (action : IApplicationBuilder -> unit, ?msg : string) =
        WebHost.setup <| AppConfig (fun (logger : ILogger) (app : IApplicationBuilder) ->
            logInfo logger "WebHost" "setupApp" (getMsg action msg)
            action app
        )
    static member setupService (action : IServiceCollection -> unit, ?msg : string) =
        WebHost.setup <| ServiceConfig (fun (logger : ILogger) (service : IServiceCollection) ->
            logInfo logger "WebHost" "setupService" (getMsg action msg)
            action service
        )

type WebHost with
    static member setupHost (action : IWebHostBuilder -> IWebHostBuilder, ?msg : string) =
        WebHost.setupHost (action >> ignore, ?msg = msg)
    static member setupApp (action : IApplicationBuilder -> IApplicationBuilder, ?msg : string) =
        WebHost.setupApp (action >> ignore, ?msg = msg)
    static member setupService (action : IServiceCollection -> IServiceCollection, ?msg : string) =
        WebHost.setupService (action >> ignore, ?msg = msg)
    static member setupAppBatch (actions : (IApplicationBuilder -> IApplicationBuilder) list) =
        fun (this : WebHost) ->
            actions
            |> List.fold (fun host action ->
                WebHost.setupApp (action) host
            ) this

type WebHost with
    static member setStaticRoot
            (
                root : string,
                ?options : FileServerOptions,
                ?enableDirectoryBrowsing : bool,
                ?serveUnknownFileTypes : bool
            ) =
        let root = Path.Combine (Directory.GetCurrentDirectory(), root);
        fun (this : WebHost) ->
            let options =
                options
                |> Option.defaultValue (
                    let mutable o = new FileServerOptions ()
                    o.EnableDirectoryBrowsing <- defaultArg enableDirectoryBrowsing this.DevMode
                    serveUnknownFileTypes
                    |> Option.iter (fun x ->
                        o.StaticFileOptions.ServeUnknownFileTypes <- x
                    )
                    o
                )
            {this with Root = Some root}
            |> WebHost.setupAppBatch [
                fun h -> h.UseFileServer (options)
            ]

type WebHost with
    static member setDevMode =
        fun (this : WebHost) ->
            {this with DevMode = true}
            |> WebHost.setupAppBatch [
                fun h -> h.UseDeveloperExceptionPage ()
                fun h -> h.UseStatusCodePages ()
                //fun h -> h.UseElmPage ()
                //fun h -> h.UseElmCapture ()
            ]//|> WebHost.setupServices (fun s -> s.AddElm ())

type WebHost with
    static member setWebSocketHub (env : IEnv, path : string, kind : Kind) =
        WebHost.setupApp (fun h ->
            h.UseWebSockets ()
        )>> WebHost.setupApp (WebSocketHub.useWebSocketHub env path kind)
    static member setWebSocketHubs (env : IEnv, hubs : (string * Kind) list) =
        WebHost.setupApp (fun h -> h.UseWebSockets ())
        >> WebHost.setupAppBatch [
            for (path, kind) in hubs do
                yield WebSocketHub.useWebSocketHub env path kind
        ]
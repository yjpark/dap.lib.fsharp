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

type WebHost = {
    Root : string option
    Urls : string []
    DevMode : bool
    Actions : (IApplicationBuilder -> unit) list
    ServicesActions : (IServiceCollection -> unit) list
} with
    member this.Build (builder : IWebHostBuilder) =
        this.Root
        |> Option.iter (fun root ->
            builder.UseContentRoot (root) |> ignore
            builder.UseWebRoot (root) |> ignore
        )
        builder.UseUrls this.Urls |> ignore
        builder.Configure (Action<IApplicationBuilder> (fun host ->
            this.Actions
            |> List.iterBack (fun action -> action host)
        )) |> ignore
        builder.ConfigureServices (Action<IServiceCollection> (fun services ->
            this.ServicesActions
            |> List.iterBack (fun action -> action services)
        )) |> ignore
        builder.Build ()
    static member empty =
        {
            Root = None
            Urls = [| |]
            DevMode = false
            Actions = []
            ServicesActions = []
        }

type WebHost with
    static member setUrls (urls : string []) =
        fun (config : WebHost) -> {config with Urls = urls}
    static member setUrl (url : string) =
        WebHost.setUrls [| url |]
    static member setPort (port : int) =
        sprintf "http://0.0.0.0:%d" port
        |> WebHost.setUrl

type WebHost with
    static member setup (action : IApplicationBuilder -> unit) =
        fun (this : WebHost) ->
            {this with Actions = action :: this.Actions}
    static member setupServices (action : IServiceCollection -> unit) =
        fun (this : WebHost) ->
            {this with ServicesActions = action :: this.ServicesActions}

type WebHost with
    static member setup (action : IApplicationBuilder -> IApplicationBuilder) =
        WebHost.setup (action >> ignore)
    static member setupBatch (actions : (IApplicationBuilder -> IApplicationBuilder) list) =
        actions
        |> List.rev
        |> List.fold (>>) id
        |> WebHost.setup
    static member setupServices (action : IServiceCollection -> IServiceCollection) =
        WebHost.setupServices (action >> ignore)

type WebHost with
    static member setStaticRoot (root : string) =
        let root = Path.Combine (Directory.GetCurrentDirectory(), root);
        fun (this : WebHost) ->
            {this with Root = Some root}
            |> WebHost.setupBatch [
                fun h -> h.UseFileServer (this.DevMode)
            ]

type WebHost with
    static member setDevMode =
        fun (this : WebHost) ->
            {this with DevMode = true}
            |> WebHost.setupBatch [
                fun h -> h.UseDeveloperExceptionPage ()
                fun h -> h.UseStatusCodePages ()
                fun h -> h.UseElmPage ()
                fun h -> h.UseElmCapture ()
            ]|> WebHost.setupServices (fun s -> s.AddElm ())

type WebHost with
    static member setWebSocketHub (env : IEnv, path : string, kind : Kind) =
        WebHost.setup (fun h ->
            h.UseWebSockets ()
        )>> WebHost.setup (WebSocketHub.useWebSocketHub env path kind)
    static member setWebSocketHubs (env : IEnv, hubs : (string * Kind) list) =
        WebHost.setup (fun h -> h.UseWebSockets ())
        >> WebHost.setupBatch [
            for (path, kind) in hubs do
                yield WebSocketHub.useWebSocketHub env path kind
        ]
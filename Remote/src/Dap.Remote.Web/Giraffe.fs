[<AutoOpen>]
module Dap.Remote.Web.Giraffe

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Dap.Prelude
open Dap.Platform
open Dap.Remote.AspNetCore.WebSocketHub

type Giraffe = GiraffeHelper with
    static member Use (action : IApplicationBuilder -> unit) =
        fun (builder : IWebHostBuilder) ->
            builder.Configure (Action<IApplicationBuilder> action)
            |> ignore
    static member UseHandler (handler : HttpHandler) =
        Giraffe.Use (fun b -> b.UseGiraffe handler |> ignore)
    static member UseErrorHandler (handler : ErrorHandler) =
        Giraffe.Use (fun b -> b.UseGiraffeErrorHandler handler |> ignore)
    static member UseWebSocketHub (env : IEnv, path : string, kind : Kind) =
        Giraffe.Use (useWebSocketHub path env kind >> ignore)
    static member UseWebSocketHubs (env : IEnv, hubs : (string * Kind) list) =
        fun (builder : IWebHostBuilder) ->
            hubs
            |> List.iter (fun (path, kind) ->
                builder |> Giraffe.UseWebSocketHub (env, path, kind)
            )

type Giraffe with
    static member SetHandler (handler : HttpHandler) = Giraffe.UseHandler handler
    static member SetErrorHandler (handler : ErrorHandler) = Giraffe.UseErrorHandler handler
    static member SetUrl (url : string) =
        fun (builder : IWebHostBuilder) ->
            builder.UseUrls url |> ignore
    static member SetPort (port : int) =
        sprintf "http://0.0.0.0:%d" port
        |> Giraffe.SetUrl

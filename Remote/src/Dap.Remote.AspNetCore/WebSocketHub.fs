module Dap.Remote.AspNetCore.WebSocketHub

open FSharp.Control.Tasks
open System.Threading
open System.Threading.Tasks
open System.Net.WebSockets
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder

open Dap.Prelude
open Dap.Platform
module GatewayTypes = Dap.Remote.WebSocketGateway.Types

type Args = {
    Path : PathString
    Env : IEnv
    ServiceKind : Kind
}

let handleWebSocket' (args : Args) (context : HttpContext) = task {
    let! socket = context.WebSockets.AcceptWebSocketAsync()
    let address = context.Connection.RemoteIpAddress.ToString()
    let guid = System.Guid.NewGuid().ToString()
    let ident = sprintf "%s_%s" address guid
    let! (agent, _) = args.Env.HandleAsync <| DoGetAgent args.ServiceKind ident
    let agent = agent :?> IAsyncPoster<GatewayTypes.Req>
    let! doProcess = agent.PostAsync <| GatewayTypes.DoAttach CancellationToken.None socket
    do! doProcess
}

let handleWebSocket (args : Args) : HttpContext -> System.Func<Task> -> Task =
    fun context next ->
        if context.Request.Path <> args.Path then
            next.Invoke context
        else
            if context.WebSockets.IsWebSocketRequest then
                handleWebSocket' args context
                :> Task
            else
                context.Response.StatusCode <- 400
                Task.CompletedTask

type IApplicationBuilder with
    member this.UseWebSocketHub (args : Args) =
        this
            .UseWebSockets()
            .Use(new System.Func<HttpContext, System.Func<Task>, Task> (handleWebSocket args))

let useWebSocketHub (path : string) (env : IEnv) (serviceKind : Kind) =
    fun (builder : IApplicationBuilder) ->
        let args = {
            Path = PathString path
            Env = env
            ServiceKind = serviceKind
        }
        builder.UseWebSocketHub args
[<RequireQualifiedAccess>]
module Dap.Remote.Web.WebSocketHub

open FSharp.Control.Tasks
open System.Threading
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Giraffe

open Dap.Prelude
open Dap.Context
open Dap.Platform
module GatewayTypes = Dap.Remote.WebSocketGateway.Types

type Args = {
    Env : IEnv
    Path : PathString
    ServiceKind : Kind
    Checker : HttpHandler option
}

let handleWebSocket' (logger : ILogger) (args : Args) (ctx : HttpContext) = task {
    let! socket = ctx.WebSockets.AcceptWebSocketAsync()
    let address = ctx.Connection.RemoteIpAddress.ToString()
    let guid = newGuid ()
    let ident = sprintf "%s_%s" address guid
    logInfo logger "Connected" ctx.Request.Host.Value (args, ident, ctx.Request)
    let! (agent, _) = args.Env.HandleAsync <| DoGetAgent args.ServiceKind ident
    let agent = agent :?> IAsyncPoster<GatewayTypes.Req>
    let! doProcess = agent.PostAsync <| GatewayTypes.DoAttach CancellationToken.None socket
    do! doProcess
}

type Middleware (next : RequestDelegate, args : Args) =
    let logger = args.Env.Logging.GetLogger <| sprintf "WebSocketHub:%s:%s" args.Path.Value args.ServiceKind

    member __.Invoke (ctx : HttpContext) = task {
        if ctx.Request.Path <> args.Path then
            return! next.Invoke ctx
        else
            if ctx.WebSockets.IsWebSocketRequest then
                match args.Checker with
                | Some checker ->
                    let handle = fun (ctx' : HttpContext) -> task {
                        do! handleWebSocket' logger args ctx'
                        return Some ctx'
                    }
                    let! _res = checker handle ctx
                    return ()
                | None ->
                    do! handleWebSocket' logger args ctx
            else
                ctx.Response.StatusCode <- 400
        }

let useWebSocketHub (env : IEnv) (path : string) (serviceKind : Kind) (checker : HttpHandler option) =
    fun (host : IApplicationBuilder) ->
        let args = {
            Env = env
            Path = PathString path
            ServiceKind = serviceKind
            Checker = checker
        }
        host.UseMiddleware<Middleware> args

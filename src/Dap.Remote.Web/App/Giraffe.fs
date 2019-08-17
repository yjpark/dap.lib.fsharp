[<AutoOpen>]
module Dap.Remote.Web.App.Giraffe

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

open Dap.Prelude
open Dap.Platform

type Giraffe = GiraffeHelper with
    static member addService (services : IServiceCollection) =
        services.AddGiraffe () |> ignore
    static member useHandler (handler : HttpHandler) =
        WebHost.setupApp (fun host -> host.UseGiraffe handler)
    static member useErrorHandler (handler : ErrorHandler) =
        WebHost.setupApp (fun host -> host.UseGiraffeErrorHandler handler)

type Giraffe with
    static member setHandler (handler : HttpHandler) = Giraffe.useHandler handler
    static member setErrorHandler (handler : ErrorHandler) = Giraffe.useErrorHandler handler

type WebHost with
    static member setGiraffe (handler : HttpHandler, ?errorHandler : ErrorHandler) =
        WebHost.setupService (fun s -> s.AddGiraffe ())
        >> Giraffe.setHandler handler
        >> (
            errorHandler
            |> Option.map Giraffe.setErrorHandler
            |> Option.defaultValue id
        )

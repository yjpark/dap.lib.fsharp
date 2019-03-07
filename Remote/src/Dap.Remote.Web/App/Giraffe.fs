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
        WebHost.setup (fun host -> host.UseGiraffe handler |> ignore)
    static member useErrorHandler (handler : ErrorHandler) =
        WebHost.setup (fun host -> host.UseGiraffeErrorHandler handler |> ignore)

type Giraffe with
    static member setHandler (handler : HttpHandler) = Giraffe.useHandler handler
    static member setErrorHandler (handler : ErrorHandler) = Giraffe.useErrorHandler handler

type WebHost with
    static member setGiraffe (handler : HttpHandler, ?errorHandler : ErrorHandler) =
        WebHost.setupServices (fun s -> s.AddGiraffe ())
        >> Giraffe.setHandler handler
        >> (
            errorHandler
            |> Option.map Giraffe.setErrorHandler
            |> Option.defaultValue id
        )

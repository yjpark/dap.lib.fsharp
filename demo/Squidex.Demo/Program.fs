module Squidex.Demo.Program

open System
open Dap.Prelude
open Dap.Context
open Dap.Platform

open Dap.Remote.Squidex
open Dap.Remote.Squidex.Query

type Translation = {
    Key : string
    Text : string
}

let execContentsQuery (env : IEnv) config query =
    let response =
        Contents.Args.Create (config, query)
        |> Contents.queryAsync env
        |> syncTask
    let logger = env.Logging.GetLogger "Contents"
    match response.Result with
    | Ok res ->
        logWip logger "Query" ((SquidexItem.WrapContentsQuery false query).ToQuery Contents.getQueryName)
        logWip logger "Succeed" (res.Length, res |> List.map (encodeJson 4))
    | Error err ->
        logWip logger "Query_Response" response
        logWip logger "Failed" err

let execContentsWithTotalQuery (env : IEnv) config query =
    let response =
        ContentsWithTotal.Args.Create (config, query)
        |> ContentsWithTotal.queryAsync env
        |> syncTask
    let logger = env.Logging.GetLogger "ContentsWithTotal"
    match response.Result with
    | Ok res ->
        logWip logger "Query" ((SquidexItem.WrapContentsQuery true query).ToQuery ContentsWithTotal.getQueryName)
        logWip logger "Succeed" (res.Total, res.Items |> List.map (encodeJson 4))
    | Error err ->
        logWip logger "Query_Response" response
        logWip logger "Failed" err

let execQuery env config query =
    execContentsQuery env config query
    execContentsWithTotalQuery env config query

[<EntryPoint>]
let main argv =
    let logging = setupConsole LogLevel.LogLevelInformation
    let platform = Feature.create<IPlatform> logging
    let clock = new RealClock ()
    let env = Env.create <| Env.param platform logging "Demo" clock
    let config =
        SquidexConfig.Create (
            url = "http://localhost:5000",
            app = "test",
            token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRkRENEVCRDYwNjMxNURFREI4MENEMDkzMERFRkZBMjFEREE2NkIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJrZjNVNjlZR01WM3R1QXpRa3czdi1pSGRwbXMifQ.eyJuYmYiOjE1NjM4NDk5MDAsImV4cCI6MTU2NjQ0MTkwMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2lkZW50aXR5LXNlcnZlciIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjUwMDAvaWRlbnRpdHktc2VydmVyL3Jlc291cmNlcyIsInNxdWlkZXgtYXBpIl0sImNsaWVudF9pZCI6InRlc3Q6ZGVmYXVsdCIsInNjb3BlIjpbInNxdWlkZXgtYXBpIl19.vIvZHxvUOJSBZjFpGvptfH5--4BaAUg6sSxDrfoi-Pd3lbrCHt035kiAFWMdwRBmYvKe6-swBoMRr7z7gILnryGV39CYV0psArRW-XAFKBSajYfCBVrjGyNHCWexouACvuhHOgpXZolUK5dJwp3O8HbqosoLMmpPHCHvzfDtxBiFDE96pvdhfvESOWmg2ZbWr7HAgZskQ59W1nDj3EftCmjenMsCpon-CpRkVpyv8qx8fxPU7UnxtBTSYDckal0OvSvUu89tt_AzHSd0Degtk8nTbA9xYxxJhK_n465OR_gE5HhmVs6wWBTO4Yofmdh71msqVvg4xYVvZ4XuJHGm-Q"
        )
    ContentsQuery.Create (
        schema = "translation",
        lang = "en",
        fields = [
            Squidex.InvariantValue "key" S.string
            Squidex.LocalizedValue "text" S.string
        ]
    ) |> execQuery env config

    let config = config.WithApp "feeds-config"
    ContentsQuery.Create (
        schema = "pipelines",
        lang = "en",
        fields = [
            Squidex.InvariantValue "key" S.string
            Squidex.InvariantValue "args" S.json
            Squidex.InvariantArray "pipes" [
                Squidex.InvariantValue "kind" S.string
                Squidex.InvariantValue "args" S.json
            ]
        ]
    ) |> execQuery env config

    0 // return an integer exit code

module Squidex.Demo.Program

open System
open Dap.Prelude
open Dap.Context
open Dap.Platform

open Dap.Local
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
        IEnvironment.Instance.Preferences.Get.Handle "SquidexConfig.json"
        |> Option.get
        |> decodeJson SquidexConfig.JsonDecoder
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

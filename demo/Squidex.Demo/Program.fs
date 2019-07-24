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
            url = "http://localhost:8000",
            app = "test",
            token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjkxRkRENEVCRDYwNjMxNURFREI4MENEMDkzMERFRkZBMjFEREE2NkIiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJrZjNVNjlZR01WM3R1QXpRa3czdi1pSGRwbXMifQ.eyJuYmYiOjE1NjM5NTUyMjMsImV4cCI6MTU2NjU0NzIyMywiaXNzIjoiaHR0cDovLzE5Mi4xNjguMC4yOjgwMDAvaWRlbnRpdHktc2VydmVyIiwiYXVkIjpbImh0dHA6Ly8xOTIuMTY4LjAuMjo4MDAwL2lkZW50aXR5LXNlcnZlci9yZXNvdXJjZXMiLCJzcXVpZGV4LWFwaSJdLCJjbGllbnRfaWQiOiJmZWVkcy1jb25maWc6ZGVmYXVsdCIsInNjb3BlIjpbInNxdWlkZXgtYXBpIl19.DxOX2G8L9EGqoJFzFlqyEXUcoDWr5fPQO7Sq2oVWzjILJvM8ydvEBX1ZhRP8YHY4Nvu_aWu7dYKnxlmuo7Jec28fEhs74Tl60-2WxRmGQemXtDwmTCNUVhs99H1KTNntQGh0R7BNrTK2wvb69fBT-SmdKbP8uLsu7UjEyGDi-p97Lxbhdr9kY9SYWGiZUtLDZvZ3goNM6XzSTeEpBklWQZ6vxU0ZD98RtycBW7WP1MXdrDzrqi9iUhXuccptxyT21c8ZTjtFvGEzzZ4VKmPPjdE95njmbTP5N-I_3uq6EDGkBXTKolaa5k7RXuoSPSCNvTqSriIgBx4AIon5JF_NCw"
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

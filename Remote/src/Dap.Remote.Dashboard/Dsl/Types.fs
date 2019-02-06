module Dap.Remote.Dashboard.Dsl.Types

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Remote.Dashboard.Meta

let AgentSnapshot =
    combo {
        var (M.instant (InstantFormat.DateHourMinuteSecondSub, "time"))
        var (M.json "version")
        var (M.json "state")
        var (M.json "stats")
    }

let EnvSnapshot =
    combo {
        list (M.custom (<@ AgentSnapshot @>, "services"))
        list (M.custom (<@ AgentSnapshot @>, "agents"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Remote.Dashboard.Types",
                [
                    G.PlatformOpens
                    G.LooseJsonRecord <@ AgentSnapshot @>
                    G.LooseJsonRecord <@ EnvSnapshot @>
                ]
            )
        )
    ]
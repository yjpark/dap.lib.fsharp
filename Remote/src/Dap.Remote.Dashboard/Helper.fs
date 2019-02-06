[<AutoOpen>]
module Dap.Remote.Dashboard.Helper

open Dap.Prelude
open Dap.Context
open Dap.Platform

type IAgent with
    member this.TakeSnapshot () : AgentSnapshot =
        this.Dash0.Inspect.Handle ()
        |> (fun res ->
            logWarn this "Snapshot" (E.encode 4 res) ()
            res
        )
        |> castJson AgentSnapshot.JsonDecoder

let private getAgents (agents : Map<Kind, Map<Key, IAgent>>) : IAgent list =
    agents
    |> Map.toList
    |> List.map snd
    |> List.map (fun kindAgents ->
        kindAgents
        |> Map.toList
        |> List.map snd
    )|> List.concat

let private takeAgentsSnapshot (agents : Map<Kind, Map<Key, IAgent>>) : AgentSnapshot list =
    getAgents agents
    |> List.map (fun agent -> agent.TakeSnapshot ())

type IEnv with
    member this.TakeSnapshot () : EnvSnapshot =
        let services = takeAgentsSnapshot this.State.Services
        let agents = takeAgentsSnapshot this.State.Agents
        {
            Services = services
            Agents = agents
        }

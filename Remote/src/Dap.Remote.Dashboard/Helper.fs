[<AutoOpen>]
module Dap.Remote.Dashboard.Helper

open Dap.Prelude
open Dap.Context
open Dap.Platform

type IAgent with
    member this.TakeSnapshot () : AgentSnapshot =
        this.Dash0.Inspect.Handle () |> ignore
        let props = this.Dash0.DashProps
        AgentSnapshot.Create (
            time = props.Time.Value,
            ident = this.Ident,
            version = props.Version.Value,
            state = props.State.Value,
            stats = toJson props.Stats
        )

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
            Scope = this.Scope
            Services = services
            Agents = agents
        }

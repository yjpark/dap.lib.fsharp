[<AutoOpen>]
module Dap.Remote.Dashboard.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type AgentSnapshot = {
    Time : (* AgentSnapshot *) Instant
    Version : (* AgentSnapshot *) Json
    State : (* AgentSnapshot *) Json
    Stats : (* AgentSnapshot *) Json
} with
    static member Create
        (
            ?time : (* AgentSnapshot *) Instant,
            ?version : (* AgentSnapshot *) Json,
            ?state : (* AgentSnapshot *) Json,
            ?stats : (* AgentSnapshot *) Json
        ) : AgentSnapshot =
        {
            Time = (* AgentSnapshot *) time
                |> Option.defaultWith (fun () -> (getNow' ()))
            Version = (* AgentSnapshot *) version
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
            State = (* AgentSnapshot *) state
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
            Stats = (* AgentSnapshot *) stats
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
        }
    static member SetTime ((* AgentSnapshot *) time : Instant) (this : AgentSnapshot) =
        {this with Time = time}
    static member SetVersion ((* AgentSnapshot *) version : Json) (this : AgentSnapshot) =
        {this with Version = version}
    static member SetState ((* AgentSnapshot *) state : Json) (this : AgentSnapshot) =
        {this with State = state}
    static member SetStats ((* AgentSnapshot *) stats : Json) (this : AgentSnapshot) =
        {this with Stats = stats}
    static member JsonEncoder : JsonEncoder<AgentSnapshot> =
        fun (this : AgentSnapshot) ->
            E.object [
                "time", InstantFormat.DateHourMinuteSecondSub.JsonEncoder (* AgentSnapshot *) this.Time
                "version", E.json (* AgentSnapshot *) this.Version
                "state", E.json (* AgentSnapshot *) this.State
                "stats", E.json (* AgentSnapshot *) this.Stats
            ]
    static member JsonDecoder : JsonDecoder<AgentSnapshot> =
        D.object (fun get ->
            {
                Time = get.Optional.Field (* AgentSnapshot *) "time" InstantFormat.DateHourMinuteSecondSub.JsonDecoder
                    |> Option.defaultValue (getNow' ())
                Version = get.Optional.Field (* AgentSnapshot *) "version" D.json
                    |> Option.defaultValue (decodeJsonValue D.json """null""")
                State = get.Optional.Field (* AgentSnapshot *) "state" D.json
                    |> Option.defaultValue (decodeJsonValue D.json """null""")
                Stats = get.Optional.Field (* AgentSnapshot *) "stats" D.json
                    |> Option.defaultValue (decodeJsonValue D.json """null""")
            }
        )
    static member JsonSpec =
        FieldSpec.Create<AgentSnapshot> (AgentSnapshot.JsonEncoder, AgentSnapshot.JsonDecoder)
    interface IJson with
        member this.ToJson () = AgentSnapshot.JsonEncoder this
    interface IObj
    member this.WithTime ((* AgentSnapshot *) time : Instant) =
        this |> AgentSnapshot.SetTime time
    member this.WithVersion ((* AgentSnapshot *) version : Json) =
        this |> AgentSnapshot.SetVersion version
    member this.WithState ((* AgentSnapshot *) state : Json) =
        this |> AgentSnapshot.SetState state
    member this.WithStats ((* AgentSnapshot *) stats : Json) =
        this |> AgentSnapshot.SetStats stats

(*
 * Generated: <Record>
 *     IsJson, IsLoose
 *)
type EnvSnapshot = {
    Services : (* EnvSnapshot *) AgentSnapshot list
    Agents : (* EnvSnapshot *) AgentSnapshot list
} with
    static member Create
        (
            ?services : (* EnvSnapshot *) AgentSnapshot list,
            ?agents : (* EnvSnapshot *) AgentSnapshot list
        ) : EnvSnapshot =
        {
            Services = (* EnvSnapshot *) services
                |> Option.defaultWith (fun () -> [])
            Agents = (* EnvSnapshot *) agents
                |> Option.defaultWith (fun () -> [])
        }
    static member SetServices ((* EnvSnapshot *) services : AgentSnapshot list) (this : EnvSnapshot) =
        {this with Services = services}
    static member SetAgents ((* EnvSnapshot *) agents : AgentSnapshot list) (this : EnvSnapshot) =
        {this with Agents = agents}
    static member JsonEncoder : JsonEncoder<EnvSnapshot> =
        fun (this : EnvSnapshot) ->
            E.object [
                "services", (E.list AgentSnapshot.JsonEncoder) (* EnvSnapshot *) this.Services
                "agents", (E.list AgentSnapshot.JsonEncoder) (* EnvSnapshot *) this.Agents
            ]
    static member JsonDecoder : JsonDecoder<EnvSnapshot> =
        D.object (fun get ->
            {
                Services = get.Optional.Field (* EnvSnapshot *) "services" (D.list AgentSnapshot.JsonDecoder)
                    |> Option.defaultValue []
                Agents = get.Optional.Field (* EnvSnapshot *) "agents" (D.list AgentSnapshot.JsonDecoder)
                    |> Option.defaultValue []
            }
        )
    static member JsonSpec =
        FieldSpec.Create<EnvSnapshot> (EnvSnapshot.JsonEncoder, EnvSnapshot.JsonDecoder)
    interface IJson with
        member this.ToJson () = EnvSnapshot.JsonEncoder this
    interface IObj
    member this.WithServices ((* EnvSnapshot *) services : AgentSnapshot list) =
        this |> EnvSnapshot.SetServices services
    member this.WithAgents ((* EnvSnapshot *) agents : AgentSnapshot list) =
        this |> EnvSnapshot.SetAgents agents
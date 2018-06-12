[<RequireQualifiedAccess>]
module Dap.Archive.Recorder.EventRecorder

open Dap.Prelude
open Dap.Platform
open Dap.Remote
open Dap.Archive

module E = Thoth.Json.Net.Encode
module D = Thoth.Json.Net.Decode

[<Literal>]
let Kind = "EventRecorder"
[<Literal>]
let Version = 1

type Extra = {
    Events : Map<string, int>
} with
    static member Create events = {
        Events = events
    }
    static member Decoder (extraDecoder : D.Decoder<'extra>) =
        D.decode Extra.Create
        |> D.required "events" (D.dict D.int)
    static member Encoder (this : Extra) =
        let events = this.Events |> Map.map (fun k v -> E.int v)
        E.object [
            "events", E.dict events
        ]
    interface JsonRecord with
        member this.ToJsonObject () =
            Extra.Encoder this

type Meta = Meta<Extra>
type Frame = PacketFrame
type Args = Args<Extra, Frame>
type Model = Model<Extra, Frame>
type Req = Req<Extra, Frame>
type Evt = Evt<Extra, Frame>
type Msg = Msg<Extra, Frame>

type IStorage' = IStorage'<Extra>

type Volume' = Volume'<Extra, Frame>

type BundleSpec' = Bundle.Spec'<Extra, Frame>
type BundleParam' = Bundle.Param'<Extra, Frame>
type Bundle' = Bundle'<Extra, Frame>

type Agent = IAgent<Model, Req, Evt>
type OnEvent = Evt -> unit

let newExtra () =
    {
        Events = Map.empty
    }

let updateExtra (extra : Extra) (frame : Frame) : Extra * Frame =
    let count =
        extra.Events
        |> Map.tryFind frame.Packet.Kind
        |> Option.defaultValue 0
    let count = count + 1
    let packet = {frame.Packet with Id = count.ToString()}
    let events =
        extra.Events
        |> Map.add frame.Packet.Kind count
    ({extra with Events = events}, {frame with Packet = packet})
    
let getSpec () =
    fun () ->
        {
            Event' = new Event<Evt>()
        }
    |> Logic.getSpec

let getSpawner env =
    getSpec ()
    |> Agent.getSpawner env

let appendEvent' (agent : Agent) (kind : string) (payload : string) : unit =
    let frame = {
        Time = agent.Env.Clock.Now
        Packet =
            {
                Id = ""
                Kind = kind
                Payload = payload
            }
    }
    agent.Post <| DoAppendFrame' frame None

let appendEvent (agent : Agent) (evt : IEvent) : unit =
    appendEvent' agent evt.Kind evt.Payload

let watchEvents (agent : Agent) 
                (onEvent : IEvent<'evt> when 'evt :> IEvent)
                (kinds : string list) : unit =
    let kinds = Set.ofList kinds
    onEvent.Add (fun evt ->
        let evt = evt :> IEvent
        if kinds |> Set.contains evt.Kind then
            appendEvent agent evt
    )

let getBundleSpec' (profile : Profile) : BundleSpec' =
    {
        Kind = Kind
        Version = 1
        CalcVolumeKey = profile.CalcVolumeKey
        VolumeDuration = profile.VolumeDuration
        NewExtra = newExtra
        UpdateExtra = updateExtra
    }

let createBundle' (profile : Profile) (param : BundleParam') : Bundle' =
    let spec = getBundleSpec' profile
    new Bundle' (spec, param)
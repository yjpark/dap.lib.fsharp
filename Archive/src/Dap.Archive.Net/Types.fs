[<AutoOpen>]
module Dap.Archive.Types

open System.IO
open Dap.Prelude
open Dap.Platform
open Dap.Remote

module E = Thoth.Json.Net.Encode
module D = Thoth.Json.Net.Decode

type Meta<'extra> when 'extra :> JsonRecord = {
    Kind : string
    Key : string
    Version : int
    Length : int
    Extra : 'extra
    BeginTime : Instant option
    EndTime : Instant option
    Memo : string option
} with
    static member Create kind key version length extra beginTime endTime memo = {
        Kind = kind
        Key = key
        Version = version
        Length = length
        Extra = extra
        BeginTime = beginTime
        EndTime = endTime
        Memo = memo
    }
    static member Decoder (extraDecoder : D.Decoder<'extra>) =
        D.decode Meta<'extra>.Create
        |> D.required "kind" D.string
        |> D.required "key" D.string
        |> D.required "version" D.int
        |> D.required "length" D.int
        |> D.required "extra" extraDecoder
        |> D.optional "begin_time" (D.option decodeInstant) None
        |> D.optional "end_time" (D.option decodeInstant) None
        |> D.optional "memo" (D.option D.string) None
    static member Encoder (this : Meta<'extra>) =
        E.object [
            "kind", E.string this.Kind
            "key", E.string this.Key
            "version", E.int this.Version
            "length", E.int this.Length
            "extra", this.Extra.ToJsonObject ()
            "begin_time", (E.option encodeInstant) this.BeginTime
            "end_time", (E.option encodeInstant) this.EndTime
            "memo", (E.option E.string) this.Memo
        ]
    interface JsonRecord with
        member this.ToJsonObject () =
            Meta<'extra>.Encoder this

type IFrame =
    abstract Time : Instant with get
    abstract WriteTo : BinaryWriter -> unit

type ReadFrame<'frame> = BinaryReader -> Result<'frame, exn>

type PacketFrame = {
    Time : Instant
    Packet : Packet'
}
with
    static member ReadFrom : ReadFrame<PacketFrame> =
        fun reader ->
            try
                let time = reader.ReadString ()
                instantOfText time
                |> Result.map (fun time' ->
                    let packet = {
                        Id = reader.ReadString ()
                        Kind = reader.ReadString ()
                        Payload = reader.ReadString ()
                    }
                    {
                        Time = time'
                        Packet = packet
                    }
                )
            with e ->
                Error e
    interface IFrame with
        member this.Time = this.Time
        member this.WriteTo writer =
            writer.Write (instantToText this.Time)
            writer.Write (this.Packet.Id)
            writer.Write (this.Packet.Kind)
            writer.Write (this.Packet.Payload)

type IStorage<'extra> when 'extra :> JsonRecord =
    abstract OpenFramesStream : IRunner -> string -> Stream

type IStorage'<'extra> when 'extra :> JsonRecord =
    abstract WriteMetaAsync : Meta<'extra> -> GetTask<unit>
    abstract NewFramesStream : IRunner -> string -> Stream

let newMeta kind key version extra (beginTime : Instant) memo =
    {
        Kind = kind
        Key = key
        Version = version
        Length = 0
        Extra = extra
        BeginTime = Some beginTime
        EndTime = None
        Memo = memo
    }

let incLengthOfMeta (extra : 'extra) (meta : Meta<'extra>) =
    { meta with
        Length = meta.Length + 1
        Extra = extra
    }
[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Archive.Bundle

open System.IO
open Dap.Prelude
open Dap.Platform
open Dap.Remote

module D = Thoth.Json.Net.Decode

type Spec<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame = {
    Kind : string
    Version : int
    ExtraDecoder : D.Decoder<'extra>
    ReadFrame : ReadFrame<'frame>
}

type Param<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame = {
    Storage : IStorage<'extra>
    KeepVolumes : bool
    KeepVolumeFrames : bool
}

type Bundle<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame (spec', param') =
    let spec : Spec<'extra, 'frame> = spec'
    let param : Param<'extra, 'frame> = param'
    let mutable volume : Volume<'extra, 'frame> option = None
    let mutable volumes : Volume<'extra, 'frame> list = []
    let mutable volumeForWrite : Volume<'extra, 'frame> option = None
    member _this.Spec with get () = spec
    member _this.Param with get () = param
    member _this.Volume with get () = volume
    member _this.Volumes with get () = volumes

type Spec'<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame = {
    Kind : string
    Version : int
    CalcVolumeKey : Instant -> string
    VolumeDuration : Duration
    NewExtra : unit -> 'extra
    UpdateExtra : 'extra -> 'frame -> 'extra * 'frame
}

type Param'<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame = {
    Storage : IStorage'<'extra>
    KeepVolumes : bool
    KeepVolumeFrames : bool
}

type Bundle'<'extra, 'frame> when 'extra :> JsonRecord and 'frame :> IFrame (spec', param') =
    let spec : Spec'<'extra, 'frame> = spec'
    let param : Param'<'extra, 'frame> = param'
    let mutable volume : Volume'<'extra, 'frame> option = None
    let mutable volumes : Volume'<'extra, 'frame> list = []
    let closeVolume (runner : IRunner) : unit =
        match volume with
        | Some volume' ->
            volume'.Close runner
            runner.RunTask ignoreOnFailed <| param.Storage.WriteMetaAsync volume'.Meta
            volume <- None
        | None ->
            ()
    let checkVolume (runner : IRunner) (time : Instant) : unit =
        let key = spec.CalcVolumeKey time
        match volume with
        | Some volume' ->
            if volume'.Meta.Key <> key then
                closeVolume runner
        | None ->
            ()
        if volume.IsNone then
            let extra = spec.NewExtra ()
            let meta = newMeta spec.Kind key spec.Version extra runner.Clock.Now None
            let volumeParam : Volume.Param'<'frame> = {
                KeepFrames = param.KeepVolumeFrames
            }
            let volume' = new Volume'<'extra, 'frame> (volumeParam, meta)
            let stream = param.Storage.NewFramesStream runner key
            volume'.Open runner stream
            volume <- Some volume'
            if param.KeepVolumes then
                volumes <- volumes @ [ volume' ]
    member _this.Spec with get () = spec
    member _this.Param with get () = param
    member _this.Volume with get () = volume
    member _this.Volumes with get () = volumes
    member _this.Open (runner : IRunner) (time : Instant) : unit =
        checkVolume runner time
    member _this.Close (runner : IRunner) : unit =
        closeVolume runner
    member _this.WriteFrame (runner : IRunner) (frame : 'frame) : unit =
        checkVolume runner frame.Time
        match volume with
        | Some volume ->
            let (extra, frame) = spec.UpdateExtra volume.Meta.Extra frame
            volume.WriteFrame runner (extra, frame)
        | None ->
            logError runner "Bundle'.WriteFrame" "CheckVolume_Failed" frame
            failwith "Bundle'.WriteFrame Failed: CheckVolume_Failed"

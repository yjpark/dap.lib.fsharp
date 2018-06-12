[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Archive.Profile

open NodaTime
open NodaTime.Text
open Dap.Prelude
open Dap.Platform
open Dap.Remote

type Profile = {
    CalcVolumeKey : Instant -> string
    VolumeDuration : Duration
}

let perMinute = {
    CalcVolumeKey =
        let pattern = InstantPattern.CreateWithInvariantCulture ("uuuu-MM-ddTHH:mm")
        fun time -> pattern.Format (time)
    VolumeDuration = Duration.FromMinutes(1L)
}

let perHour = {
    CalcVolumeKey =
        let pattern = InstantPattern.CreateWithInvariantCulture ("uuuu-MM-ddTHH")
        fun time -> pattern.Format (time)
    VolumeDuration = Duration.FromHours(1)
}

let perDay = {
    CalcVolumeKey =
        let pattern = InstantPattern.CreateWithInvariantCulture ("uuuu-MM-dd")
        fun time -> pattern.Format (time)
    VolumeDuration = Duration.FromDays(1)
}

let prefixesDay = [10]
let prefixesMonthDay = [7 ; 10]
let prefixesYearMonthDay = [4 ; 7 ; 10]
let prefixesDayHour = [10 ; 13]

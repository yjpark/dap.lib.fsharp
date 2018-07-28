[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Forms.Const

let mutable LogFolder = "log"

let setLogFolder folder =
    LogFolder <- folder
[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Native

open Fable.Core
open Fable.Core.JsInterop

open Dap.Prelude
open Dap.Context
open Dap.Platform

[<Literal>]
let JsonFormatterTarget = "Json_Formatter_Target"

[<Import("getInputValue", "./Native/misc.js")>]
let getInputValue : obj -> string = jsNative

[<Import("renderJsonWithDefault", "./Native/json_formatter.js")>]
let renderJsonWithDefault' : string -> Json -> int -> obj = jsNative

[<Import("renderJsonWithConfig", "./Native/json_formatter.js")>]
let renderJsonWithConfig' : string -> Json -> int -> Json -> obj = jsNative

[<Import("renderJsonTemp", "./Native/json_formatter.js")>]
let renderJsonTemp' : Json -> obj = jsNative


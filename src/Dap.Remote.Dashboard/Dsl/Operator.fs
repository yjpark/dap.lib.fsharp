module Dap.Remote.Dashboard.Dsl.Operator

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Remote.Meta
open Dap.Remote.Generator

open Dap.Remote.Dashboard.Meta
open Dap.Remote.Dashboard.Dsl.Types

let AuthRes =
    combo {
        var (M.json "info")
    }

let AuthErr =
    union {
        kind "InvalidToken"
    }

let Evt =
    union {
        case "OnNewAgent" (fields {
            var (M.custom (<@ AgentSnapshot @>, "agent"))
        })
    }

let InspectErr =
    union {
        kind "PermissionDenied"
    }

let Operator =
    stub {
        req Do "Auth"
        req Do "Inspect"
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Operator.fs"],
            G.QualifiedModule ("Dap.Remote.Dashboard.Operator",
                [
                    ["type AuthReq = JsonString"]
                    G.JsonRecord <@ AuthRes @>
                    G.JsonUnion <@ AuthErr @>
                    ["type InspectReq = JsonNil"]
                    ["type InspectRes = EnvSnapshot"]
                    G.JsonUnion <@ InspectErr @>
                    G.JsonUnion <@ Evt @>
                    @ [
                        "    interface IEvent with"
                        "        member this.Kind = Union.getKind<Evt> this"
                    ]
                    G.Stub <@ Operator @>
                ]
            )
        )
    ]
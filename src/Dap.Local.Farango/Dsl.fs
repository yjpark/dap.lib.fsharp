module Dap.Local.Farango.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator

[<Literal>]
let FarangoDbKind = "FarangoDb"

let DbArgs =
    combo {
        var (M.string ("uri"))
    }

type M with
    static member farangoDb (?kind : Kind, ?key : Key) =
        let kind = defaultArg kind FarangoDbKind
        let alias = "FarangoDb", "Dap.Local.Farango.Db"
        let args = JsonArgs "FarangoDb.Args"
        let type' = "FarangoDb.Agent"
        let spec = "FarangoDb.spec"
        M.agent (args, type', spec, kind, ?key = key, aliases = [alias])

let IDbPack =
    pack [] {
        add (M.farangoDb ())
    }

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Farango.Types",
                [
                    G.LooseJsonRecord (<@ DbArgs @>)
                ]
            )
        )
        G.File (segments, ["_Gen"; "Builder.fs"],
            G.BuilderModule ("Dap.Local.Farango.Builder",
                [
                    G.ValueBuilder <@ DbArgs @>
                ]
            )
        )
        G.File (segments, ["_Gen"; "Packs.fs"],
            G.AutoOpenModule ("Dap.Local.Farango.Packs",
                [
                    G.PackOpens
                    [
                        "type Db = FarangoDb.Model"
                    ]
                    G.PackInterface <@ IDbPack @>
                ]
            )
        )
    ]
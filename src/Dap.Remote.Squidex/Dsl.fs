module Dap.Remote.Squidex.Dsl

open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Meta.Net
open Dap.Platform.Generator
open Dap.Platform.Dsl.Packs
open Dap.WebSocket.Meta
open Dap.Remote.Meta
open Dap.Remote.Meta.Net

let SquidexConfig =
    combo {
        var (M.string "url")
        var (M.string "app")
        var (M.string "token")
    }

let SquidexItem =
    combo {
        var (M.string "id")
        var (M.int "version")
        var (M.dateTime "created")
        var (M.string "createdBy")
        var (M.dateTime "lastModified")
        var (M.string "lastModifiedBy")
        var (M.string "url")
        var (M.json "data")
        var (M.json "dataFlatten")
    }

let ContentField =
    union {
        kind "NoField"
        case "SimpleValue" (fields {
            var (M.string "key")
            var (M.custom ("FieldSpec", "spec", "S.json"))
        })
        case "InvariantValue" (fields {
            var (M.string "key")
            var (M.custom ("FieldSpec", "spec", "S.json"))
        })
        case "LocalizedValue" (fields {
            var (M.string "key")
            var (M.custom ("FieldSpec", "spec", "S.json"))
        })
        case "SimpleChild" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "InvariantChild" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "LocalizedChild" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "SimpleArray" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "InvariantArray" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "LocalizedArray" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "SimpleLinks" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "InvariantLinks" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
        case "LocalizedLinks" (fields {
            var (M.string "key")
            list (M.custom ("ContentField", "fields", "NoField"))
        })
    }|> UnionMeta.SetInitValue (Some "NoField")

let ContentsQuery =
    combo {
        var (M.string "schema")
        var (M.string "lang")
        list (M.custom (<@ ContentField @>, "fields"))
        var (M.int ("top", 200))
        var (M.int ("skip", 0))
        option (M.string "filter")
        option (M.string "orderby")
    }

let ContentsWithTotalResult =
    combo {
        var (M.int "total")
        list (M.custom (<@ SquidexItem @>, "items"))
    }

let compile segments =
    [
        G.File (segments, ["_Gen" ; "SquidexTypes.fs"],
            G.AutoOpenModule ("Dap.Remote.Squidex.Types",
                [
                    G.PlatformOpens
                    G.JsonRecord <@ SquidexConfig @>
                    G.LooseJsonRecord <@ SquidexItem @>
                ]
            )
        )
        G.File (segments, ["_Gen" ; "QueryTypes.fs"],
            G.AutoOpenModule ("Dap.Remote.Squidex.QueryTypes",
                [
                    G.PlatformOpens
                    G.Union <@ ContentField @>
                    G.Record <@ ContentsQuery @>
                    G.JsonRecord <@ ContentsWithTotalResult @>
                ]
            )
        )
    ]



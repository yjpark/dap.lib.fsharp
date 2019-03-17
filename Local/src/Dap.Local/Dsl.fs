module Dap.Local.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Meta.Net
open Dap.Context.Generator
open Dap.Platform
open Dap.Platform.Meta
open Dap.Platform.Generator
open Dap.Platform.Dsl
open Dap.Local.Meta

let SetTextReq =
    combo {
        var (M.luid ("path"))
        var (M.string ("text"))
    }

let EnvironmentProps =
    combo {
        var (M.string ("data_directory", value="data"))
        var (M.string ("cache_directory", value="cache"))
    }

let Environment =
    context <@ EnvironmentProps @> {
        kind "Environment"
        handler (M.unit "inspect") (M.json response)
    }

let PreferencesProps =
    combo {
        var (M.string ("root", value="preferences"))
    }

let Preferences =
    context <@ PreferencesProps @> {
        kind "Preferences"
        handler (M.luid "has") (M.bool response)
        handler (M.luid "get") (M.string response)
        handler (M.custom (<@ SetTextReq @>, "set")) (M.unit response)
        handler (M.luid "remove") (M.unit response)
        handler (M.unit "clear") (M.unit response)
    }

let SecureStorageProps =
    combo {
        var (M.string ("root", value="secure_storage"))
        var (M.string ("secret", value="Iemohwai9iiY2phojael2och7quiex6Thohneothaek7eeghaebeewohghie9shu"))
    }

let SecureStorage =
    context <@ SecureStorageProps @> {
        kind "SecureStorage"
        async_handler (M.luid "has") (M.bool response)
        async_handler (M.luid "get") (M.string response)
        async_handler (M.custom (<@ SetTextReq @>, "set")) (M.unit response)
        handler (M.luid "remove") (M.unit response)
        handler (M.unit "clear") (M.unit response)
    }

let IAppPack =
    pack [] {
        add (M.environment ())
        add (M.preferences ())
        add (M.secureStorage ())
    }

type G with
    static member AppPack (feature : string option) =
        let feature = defaultArg feature "Dap.Local.Feature"
        [
            sprintf "type Preferences = %s.Preferences.Context" feature
            sprintf "type SecureStorage = %s.SecureStorage.Context" feature
        ]

let compile segments =
    [
        G.File (segments, ["_Gen"; "Types.fs"],
            G.AutoOpenModule ("Dap.Local.Types",
                [
                    G.PlatformOpens
                    G.JsonRecord (<@ SetTextReq @>)
                    G.Combo (<@ EnvironmentProps @>)
                    G.Feature (<@ Environment @>)
                    G.Combo (<@ PreferencesProps @>)
                    G.Feature (<@ Preferences @>)
                    G.Combo (<@ SecureStorageProps @>)
                    G.Feature (<@ SecureStorage @>)
                ]
            )
        )
        G.File (segments, ["_Gen"; "IAppPack.fs"],
            G.AutoOpenModule ("Dap.Local.IAppPack",
                [
                    G.PlatformOpens
                    G.PackOpens
                    G.PackInterface <@ IAppPack @>
                ]
            )
        )
    ]
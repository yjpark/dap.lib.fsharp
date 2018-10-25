module Dap.Local.Dsl

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
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
                    G.Combo (<@ PreferencesProps @>)
                    G.Context (<@ Preferences @>)
                    G.Combo (<@ SecureStorageProps @>)
                    G.Context (<@ SecureStorage @>)
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
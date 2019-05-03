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

let Version =
    combo {
        // https://semver.org/
        var (M.int "major")
        var (M.int "minor")
        var (M.int "patch")
        var (M.string "commit")
        var (M.string "comment")
    }

let IVersion = """
type IVersion =
#if !FABLE_COMPILER
    inherit Dap.Platform.Cli.ICliHook
#endif
    abstract Major : int with get
    abstract Minor : int with get
    abstract Patch : int with get
    abstract Commit : string with get
    abstract Comment : string with get

[<AutoOpen>]
module IVersionExtensions =
    type IVersion with
        member this.SemVer =
            sprintf "%s.%s.%s" this.Major this.Minor this.Patch
        member this.DevVer =
            this.SemVer
            |> (fun x ->
                if System.String.IsNullOrEmpty this.Commit then
                    x
                else
                    sprintf "%s-%s" x this.Commit
            )
        member this.ToVersion () =
            Version.Create (
                major = this.Major,
                minor = this.Minor,
                patch = this.Patch,
                commit = this.Commit,
                comment = this.Comment
            )
"""

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
        handler (M.luid "get") (M.option (M.string response))
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
        async_handler (M.luid "get") (M.option (M.string response))
        async_handler (M.custom (<@ SetTextReq @>, "set")) (M.unit response)
        handler (M.luid "remove") (M.unit response)
        handler (M.unit "clear") (M.unit response)
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
                    G.JsonRecord (<@ Version @>)
                    [ IVersion ]
                    G.JsonRecord (<@ SetTextReq @>)
                    G.Combo (<@ PreferencesProps @>)
                    G.Feature (<@ Preferences @>)
                    G.Combo (<@ SecureStorageProps @>)
                    G.Feature (<@ SecureStorage @>)
                    G.Combo (<@ EnvironmentProps @>)
                    G.FeatureInterface (<@ Environment @>) @ [
                        "    abstract Version : Version with get"
                        "    abstract Preferences : IPreferences with get"
                        "    abstract SecureStorage : ISecureStorage with get"
                    ]
                ]
            )
        )
    ]
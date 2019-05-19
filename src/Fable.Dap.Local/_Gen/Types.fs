[<AutoOpen>]
module Dap.Local.Types

open Dap.Prelude
open Dap.Context
open Dap.Platform

(*
 * Generated: <Record>
 *     IsJson
 *)
type Version = {
    Major : (* Version *) int
    Minor : (* Version *) int
    Patch : (* Version *) int
    Commit : (* Version *) string
    Comment : (* Version *) string
    PreRelease : (* Version *) string option
} with
    static member Create
        (
            ?major : (* Version *) int,
            ?minor : (* Version *) int,
            ?patch : (* Version *) int,
            ?commit : (* Version *) string,
            ?comment : (* Version *) string,
            ?preRelease : (* Version *) string
        ) : Version =
        {
            Major = (* Version *) major
                |> Option.defaultWith (fun () -> 0)
            Minor = (* Version *) minor
                |> Option.defaultWith (fun () -> 0)
            Patch = (* Version *) patch
                |> Option.defaultWith (fun () -> 0)
            Commit = (* Version *) commit
                |> Option.defaultWith (fun () -> "")
            Comment = (* Version *) comment
                |> Option.defaultWith (fun () -> "")
            PreRelease = (* Version *) preRelease
        }
    static member SetMajor ((* Version *) major : int) (this : Version) =
        {this with Major = major}
    static member SetMinor ((* Version *) minor : int) (this : Version) =
        {this with Minor = minor}
    static member SetPatch ((* Version *) patch : int) (this : Version) =
        {this with Patch = patch}
    static member SetCommit ((* Version *) commit : string) (this : Version) =
        {this with Commit = commit}
    static member SetComment ((* Version *) comment : string) (this : Version) =
        {this with Comment = comment}
    static member SetPreRelease ((* Version *) preRelease : string option) (this : Version) =
        {this with PreRelease = preRelease}
    static member JsonEncoder : JsonEncoder<Version> =
        fun (this : Version) ->
            E.object [
                "major", E.int (* Version *) this.Major
                "minor", E.int (* Version *) this.Minor
                "patch", E.int (* Version *) this.Patch
                "commit", E.string (* Version *) this.Commit
                "comment", E.string (* Version *) this.Comment
                "pre_release", (E.option E.string) (* Version *) this.PreRelease
            ]
    static member JsonDecoder : JsonDecoder<Version> =
        D.object (fun get ->
            {
                Major = get.Required.Field (* Version *) "major" D.int
                Minor = get.Required.Field (* Version *) "minor" D.int
                Patch = get.Required.Field (* Version *) "patch" D.int
                Commit = get.Required.Field (* Version *) "commit" D.string
                Comment = get.Required.Field (* Version *) "comment" D.string
                PreRelease = get.Required.Field (* Version *) "pre_release" (D.option D.string)
            }
        )
    static member JsonSpec =
        FieldSpec.Create<Version> (Version.JsonEncoder, Version.JsonDecoder)
    interface IJson with
        member this.ToJson () = Version.JsonEncoder this
    interface IObj
    member this.WithMajor ((* Version *) major : int) =
        this |> Version.SetMajor major
    member this.WithMinor ((* Version *) minor : int) =
        this |> Version.SetMinor minor
    member this.WithPatch ((* Version *) patch : int) =
        this |> Version.SetPatch patch
    member this.WithCommit ((* Version *) commit : string) =
        this |> Version.SetCommit commit
    member this.WithComment ((* Version *) comment : string) =
        this |> Version.SetComment comment
    member this.WithPreRelease ((* Version *) preRelease : string option) =
        this |> Version.SetPreRelease preRelease


type IVersion =
#if !FABLE_COMPILER
    inherit Dap.Platform.Cli.ICliHook
#endif
    abstract Major : int with get
    abstract Minor : int with get
    abstract Patch : int with get
    abstract Commit : string with get
    abstract Comment : string with get
    abstract PreRelease : string option with get

[<AutoOpen>]
module IVersionExtensions =
    type IVersion with
        member this.ToVersion () =
            Version.Create (
                major = this.Major,
                minor = this.Minor,
                patch = this.Patch,
                commit = this.Commit,
                comment = this.Comment,
                ?preRelease = this.PreRelease
            )

    type Version with
        member this.SemVer =
            sprintf "%i.%i.%i" this.Major this.Minor this.Patch
        member this.DevVer =
            this.SemVer
            |> (fun x ->
                match this.PreRelease with
                | None -> x
                | Some pre -> sprintf "%s-%s" x pre
            )|> (fun x ->
                if System.String.IsNullOrEmpty this.Commit then
                    x
                else
                    sprintf "%s+%s" x this.Commit
            )

[<AutoOpen>]
module Dap.Local.GitVersion

open Dap.Prelude
open Dap.Context
open Dap.Platform

[<Literal>]
let VersionFile = "Version.fs"

[<Literal>]
let GitVersionFile = "GitVersion.fs"

[<Literal>]
let QuoteTag = "$QUOTE$"

[<Literal>]
let Quote = "\"\"\""

[<Literal>]
let PrefixTag = "$PREFIX$"

[<Literal>]
let CommitTag = "$COMMIT$"

[<Literal>]
let CommentTag = "$COMMENT$"

[<Literal>]
let AppVersionTemplate = """
module $PREFIX$.AppVersion

open Dap.Local

type AppVersion () =
with
    interface IVersion with
        member __.Major = 0
        member __.Minor = 1
        member __.Patch = 0
        member __.Commit = $PREFIX$.GitVersion.Commit
        member __.Comment = $PREFIX$.GitVersion.Comment
"""

[<Literal>]
let GitVersionTemplate = """
module $PREFIX$.GitVersion

let Commit = "$COMMIT$"
let Comment = $QUOTE$
$COMMENT$
$QUOTE$
"""

let Separator =
    "--------------------------------------------------------------------------------\n"

let updateGitVersionFile (logger : ILogger) (folder : string) (prefix : string) =
    let commit = (Shell.exec "git" "rev-parse HEAD") .Replace ("\n", "")
    let comment =
        [
            Shell.exec "git" "log -1"
            Shell.exec "git" "status"
            Shell.exec "git" "diff --cached"
            Shell.exec "git" "diff"
        ]|> List.map (fun l -> l.Replace (Quote, "\\\"\\\"\\\""))
        |> String.concat Separator
    let path = System.IO.Path.Combine (folder, GitVersionFile)
    GitVersionTemplate
        .Replace(QuoteTag, Quote)
        .Replace(PrefixTag, prefix)
        .Replace(CommitTag, commit)
        .Replace(CommentTag, comment)
    |> TextFile.save path
    logWarn logger "GitVersion.updateGitVersionFile" "GitVersion_Updated" path

let checkAppVersionFile (logger : ILogger) (folder : string) (prefix : string) =
    let path = System.IO.Path.Combine (folder, VersionFile)
    if FileSystem.fileExists path then
        logWarn logger "GitVersion.checkAppVersionFile" "AppVersion_Exists" path
    else
        AppVersionTemplate.Replace(PrefixTag, prefix)
        |> TextFile.save path
        logWarn logger "GitVersion.checkAppVersionFile" "AppVersion_Created" path

let update (logger : ILogger) (folder : string) (prefix : string) =
    if FileSystem.folderExists folder then
        updateGitVersionFile logger folder prefix
        checkAppVersionFile logger folder prefix
    else
        logWarn logger "GitVersion.update" "Folder_Not_Exist" folder
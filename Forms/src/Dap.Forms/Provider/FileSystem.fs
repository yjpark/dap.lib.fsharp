module Dap.Forms.Provider.FileSystem

open System.IO
open Xamarin.Forms

open Dap.Prelude
module Boot = Dap.Local.App.Boot

open Dap.Forms

[<Literal>]
let UWP_LogFolderTip = """
For UWP, need to change the log folder before init:
    var cacheFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
    Dap.Local.App.Boot.setLogFolder(cacheFolder + "/log");
    var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
    Dap.Local.App.Boot.setDocFolder(localFolder + "/doc");
"""

let getCacheFolder () =
    if hasEssentials () then
        Some Xamarin.Essentials.FileSystem.CacheDirectory
    else
        None

let getAppDataFolder () =
    if hasEssentials () then
        Some Xamarin.Essentials.FileSystem.AppDataDirectory
    else
        None

let fixBootLogging () =
    if isRealForms () then
        let device = Device.RuntimePlatform
        if (device = Device.macOS || device = Device.iOS) then
            Boot.setLogToConsole false
        elif device = Device.UWP then
            if Boot.getLogFolder () = Boot.DefaultLogFolder then
                failWith "Invalid_LogFolder" UWP_LogFolderTip

let fixBootFolders () =
    getCacheFolder ()
    |> Option.iter (fun cache ->
        Boot.setLogFolder <| Path.Combine (cache, "log")
    )
    getAppDataFolder ()
    |> Option.iter (fun appData ->
        Boot.setDocFolder <| Path.Combine (appData, "doc")
    )
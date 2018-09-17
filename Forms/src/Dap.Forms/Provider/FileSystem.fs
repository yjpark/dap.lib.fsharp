module Dap.Forms.Provider.FileSystem

open System.IO
open Xamarin.Forms

open Dap.Prelude
open Dap.Forms

let getAppDataFolder () =
    if hasEssentials () then
        Xamarin.Essentials.FileSystem.AppDataDirectory
    else
        ""

let getCacheFolder () =
    if hasEssentials () then
        Xamarin.Essentials.FileSystem.CacheDirectory
    else
        ""

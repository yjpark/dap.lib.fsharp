module Dap.Local.Generator

open Dap.Prelude
open Dap.Context
open Dap.Context.Generator
open Dap.Context.Generator.Util

type G with
    static member AppPack (?feature : string) =
        let feature = defaultArg feature "Dap.Local.Feature"
        [
            sprintf "type Preferences = %s.Preferences.Context" feature
            sprintf "type SecureStorage = %s.SecureStorage.Context" feature
        ]
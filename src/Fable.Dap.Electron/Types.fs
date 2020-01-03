[<AutoOpen>]
module Dap.Electron.Types

type IRoute =
    abstract Url : string with get

type Widget = Fable.Electron.ElectronElement


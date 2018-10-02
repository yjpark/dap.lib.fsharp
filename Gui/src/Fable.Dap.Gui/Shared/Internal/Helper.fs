[<AutoOpen>]
module Dap.Gui.Internal.Helper

open Dap.Prelude
open Dap.Context
open Dap.Context.Helper
open Dap.Gui

type Button with
    static member AddChannels (channels : IChannels) =
        channels.AddUnit "on_click"
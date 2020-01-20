[<AutoOpen>]
module Dap.React.Types

open Fable.Core
module H = Fable.React.Helpers
module R = Fable.React.Standard
module P = Fable.React.Props
                                                                      //__SILP__
open Dap.Prelude
open Dap.Context
open Dap.Platform

type IRoute =
    abstract Url : string with get

type Widget = Fable.React.ReactElement

[<Literal>]
let DefaultMarginPx = 12

type W = DapReactWidgets
with
    static member Space
        (
            ?px : int
        ) =
        let px = defaultArg px DefaultMarginPx
        R.span [
            P.Style [
                P.CSSProp.MarginRight (sprintf "%ipx" px)
            ]
        ] []



[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Button

//SILP: FULMA_VIEW_IMPORTS()
open Fable.Core                                                       //__SILP__
open Fulma                                                            //__SILP__
open Fable.FontAwesome                                                //__SILP__
module H = Fable.React.Helpers                                        //__SILP__
module R = Fable.React.Standard                                       //__SILP__
module P = Fable.React.Props                                          //__SILP__
                                                                      //__SILP__
open Dap.Prelude                                                      //__SILP__
open Dap.Context                                                      //__SILP__
open Dap.Platform                                                     //__SILP__
open Dap.React                                                        //__SILP__

type OnClick = Browser.Types.MouseEvent -> unit

type W with
    static member Button
        (
            text : string,
            onClick : OnClick,
            ?color : IColor
        ) =
        Button.button [
            if color.IsSome then
                yield Button.Color color.Value
            yield Button.OnClick onClick
        ] [H.str text]

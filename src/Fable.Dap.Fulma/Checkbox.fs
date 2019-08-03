[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Checkbox

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

type Model = {
    Text : string
    State : bool
    OnChange : (bool -> unit) option
    MarginLeftPx : int
} with
    static member Create
        (
            text : string,
            ?state : bool,
            ?onChange : bool -> unit,
            ?marginLeftPx : int
        ) : Model =
        {
            Text = text
            State = defaultArg state false
            OnChange = onChange
            MarginLeftPx = defaultArg marginLeftPx DefaultMarginPx
        }

type W with
    static member Checkbox' (model : Model) : Widget =
        Checkbox.checkbox [] [
            R.input [
                yield P.Type "checkbox"
                yield P.Checked model.State
                if model.OnChange.IsSome then
                    yield P.OnChange (fun e ->
                        //TODO get value from e
                        model.OnChange.Value (not model.State)
                    )
            ]
            R.span [P.Style [P.CSSProp.MarginLeft (sprintf "%ipx" model.MarginLeftPx)]] [
                H.str model.Text
            ]
        ]
    static member Checkbox
        (
            text : string,
            ?state : bool,
            ?onChange : bool -> unit,
            ?marginLeftPx : int
        ) : Widget =
        Model.Create (
            text,
            ?state = state,
            ?onChange = onChange,
            ?marginLeftPx = marginLeftPx
        )|> W.Checkbox'


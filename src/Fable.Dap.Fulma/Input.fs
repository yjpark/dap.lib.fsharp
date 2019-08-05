[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Input

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

[<RequireQualifiedAccess>]
type Style =
    | Text
    | Email
    | Password
with
    member this.Render =
        match this with
        | Text -> Input.text
        | Email -> Input.email
        | Password -> Input.password

type Model = {
    Style : Style
    Text : string
    Icon : Fa.IconOption option
    Error : string option
    OnChange : (string -> unit) option
    OnSubmit : (string -> unit) option
} with
    static member Create
        (
            ?style : Style,
            ?text : string,
            ?icon : Fa.IconOption,
            ?error : string,
            ?onChange : string -> unit,
            ?onSubmit : string -> unit
        ) : Model =
        {
            Style = defaultArg style Style.Text
            Text = defaultArg text ""
            Icon = icon
            Error = error
            OnChange = onChange
            OnSubmit = onSubmit
        }

type W with
    static member Input' (model : Model) : Widget list =
        let mutable v = model.Text
        let inputOptions = [
            yield Input.DefaultValue model.Text
            if model.OnChange.IsSome || model.OnSubmit.IsSome then
                yield Input.OnChange (fun e ->
                    v <- getInputValue e.target
                    if model.OnChange.IsSome then
                        model.OnChange.Value v
                )
            if model.Error.IsSome then
                yield Input.Color IsDanger
            if model.OnSubmit.IsSome then
                yield Input.Props [
                    P.OnKeyPress (fun e ->
                        if e.key = "Enter" then
                            model.OnSubmit.Value v
                    )
                ]
        ]
        let input =
            Control.div [
                if model.Icon.IsSome then
                    yield Control.HasIconLeft
                if model.Error.IsSome then
                    yield Control.HasIconRight
            ] [
                yield model.Style.Render inputOptions
                if model.Icon.IsSome then
                    yield Icon.icon [Icon.Size IsSmall; Icon.IsLeft] [Fa.i [model.Icon.Value] [] ]
                if model.Error.IsSome then
                    yield Icon.icon [Icon.Size IsSmall; Icon.IsRight] [Fa.i [Fa.Solid.Exclamation] [] ]
            ]
        match model.Error with
        | None -> [input]
        | Some err ->
            input :: [
                Help.help [Help.Color IsDanger] [H.str err]
            ]
    static member Input
        (
            ?style : Style,
            ?text : string,
            ?icon : Fa.IconOption,
            ?error : string,
            ?onChange : string -> unit,
            ?onSubmit : string -> unit
        ) : Widget list =
        Model.Create (
            ?style = style,
            ?text = text,
            ?icon = icon,
            ?error = error,
            ?onChange = onChange,
            ?onSubmit = onSubmit
        )|> W.Input'


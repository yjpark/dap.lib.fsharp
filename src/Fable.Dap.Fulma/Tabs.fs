[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Tabs

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

type Item = {
    IsActive : bool
    Text : string
    OnClick : unit -> unit
} with
    static member Create isActive text onClick =
        {
            IsActive = isActive
            Text = text
            OnClick = onClick
        }
    static member CreateInactive = Item.Create false
    static member CreateActive = Item.Create true

type W with
    static member Tabs (tabs : Item list) =
        tabs
        |> List.map (fun tab ->
            Tabs.tab [
                if tab.IsActive then
                    yield Tabs.Tab.IsActive true
            ] [
                Navbar.Item.a [
                    Navbar.Item.Props [ P.OnClick (fun _ ->
                        tab.OnClick ()
                    )]
                ] [
                    H.str tab.Text
                ]
            ]
        ) |> Tabs.tabs [
            Tabs.IsFullWidth
            Tabs.IsBoxed
        ]

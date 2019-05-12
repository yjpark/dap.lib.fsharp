[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Breadcrumb

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

type Item<'route when 'route :> IRoute> = {
    IsActive : bool
    Text : string
    Route : 'route
} with
    static member Create isActive text route : Item<'route> =
        {
            IsActive = isActive
            Text = text
            Route = route
        }
    static member CreateInactive = Item<'route>.Create false
    static member CreateActive = Item<'route>.Create true

type W with
    static member Breadcrumb<'route when 'route :> IRoute>
            (
                items : Item<'route> list,
                ?home : Item<'route>
            ) : Widget =
        home
        |> Option.map (fun x ->
            {x with IsActive = (items.Length = 0)}
        )|> Option.toList
        |> List.extend items
        |> List.map (fun item ->
            Breadcrumb.item [
                yield Breadcrumb.Item.IsActive item.IsActive
            ] [
                R.a [P.Href item.Route.Url] [H.str item.Text]
            ]
        )|> Breadcrumb.breadcrumb []

[<AutoOpen>]
module Dap.Forms.Util

open System.Threading.Tasks
open Xamarin.Forms
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

open Dap.Prelude
open Dap.Platform

let isMockForms () =
    try
        Device.Info = null
    with _ ->
        true

let isRealForms () =
    not <| isMockForms ()

let newApplication () =
    if isRealForms () then
        let application = new Application ()
        let emptyPage = View.ContentPage (content = View.Label (text = "TEST"))
        let page = emptyPage.Create ()
        application.MainPage <- page :?> Page
        application
    else
        failWith "newApplication" "Is_Not_Real_Forms"
[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Fulma.Json

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

type W with
    static member JsonContent (json : IJson) =
        let json = json.ToJson ()
        //Not sure why, after update with Fable2, the jsNative is not working
        //properly with more than one param, seems a bug in curry parameters
        //renderJsonWithDefault' JsonFormatterTarget json 20
        Native.renderJsonTemp' json
        |> ignore
        Content.content [
            Content.Props [P.Id Native.JsonFormatterTarget]
        ] [
            H.str "..."
        ]


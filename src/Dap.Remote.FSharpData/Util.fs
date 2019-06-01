[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Remote.FSharpData.Util

open Dap.Prelude
open Dap.Context

type IJson with
    member this.ToHttpHeaders (logger : ILogger) =
        let json = this.ToJson ()
        json.ToObjectKeys ()
        |> Seq.choose (fun k ->
            match tryCastJson (D.field k D.json) json with
            | Ok v ->
                if v.IsString || v.IsInt || v.IsFloat then
                    Some (k, v.ToString ())
                elif v.IsBool then
                    if v.Value<bool>() then
                        Some (k, "true")
                    else
                        Some (k, "false")
                else
                    logError logger "IJson.ToHttpHeaders" (sprintf "Field_Not_Supported: %s" k) (E.encode 4 v)
                    None
            | Error err ->
                logError logger "IJson.ToHttpHeaders" (sprintf "Decode_Field_Failed: %s" k) err
                None
        )
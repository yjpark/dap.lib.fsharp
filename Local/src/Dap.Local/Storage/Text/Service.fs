module Dap.Local.Storage.Text.Service

open Dap.Platform

module BaseTypes = Dap.Local.Storage.Base.Types
module Logic = Dap.Local.Storage.Base.Logic

[<Literal>]
let Kind = "TextStorage"

type Req = BaseTypes.Req<string>
type Evt = BaseTypes.Evt<string>
type Service = IAgent<Req, Evt>

type SaveParam = BaseTypes.SaveParam<string>
type IProvider = BaseTypes.IProvider<string>
type Args = BaseTypes.Args<string, string>

let TryLoad = BaseTypes.TryLoad
let DoLoad = BaseTypes.DoLoad
let DoSave = BaseTypes.DoSave
let DoSaveNew = BaseTypes.DoSaveNew

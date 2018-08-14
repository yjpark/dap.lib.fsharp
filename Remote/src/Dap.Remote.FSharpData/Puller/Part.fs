[<RequireQualifiedAccess>]
module Dap.Remote.FSharpData.Puller.Part

open Dap.Prelude
open Dap.Platform
open Dap.Remote

open Dap.Remote.FSharpData.Puller.Types
module Logic = Dap.Remote.FSharpData.Puller.Logic

let create<'actorRunner, 'actorModel, 'actorMsg, 'res
            when 'actorRunner :> IAgent<'actorMsg> and 'actorMsg :> IMsg>
        (args : Args<'res>) partMsg wrapMsg agent =
    let spec = Logic.spec<'actorMsg, 'res> args
    agent |> Part.create<'actorRunner, 'actorModel, 'actorMsg, Part<'actorMsg, 'res>, Args<'res>, Model<'res>, Msg<'res>, Req, Evt<'res>> spec partMsg wrapMsg

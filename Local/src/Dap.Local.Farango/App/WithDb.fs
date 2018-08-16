[<AutoOpen>]
[<RequireQualifiedAccess>]
module Dap.Local.Farango.App.WithDb

open System.Threading.Tasks
open FSharp.Control.Tasks.V2

open Dap.Prelude
open Dap.Platform
open Dap.Local.App

open Dap.Local.Farango

type Args = {
    Base : Simple.Args
    Db : Db.Args
    SetupAsync : Model -> Task<unit>
} with
    static member Create (base' : Simple.Args) db setupAsync =
        {
            Base = base'
            Db = db
            SetupAsync = setupAsync
        }

and Model = {
    Args : Args
    Base : Simple.Model
    Db : Db.Model
} with
    static member Create args (base' : Simple.Model) db =
        {
            Args = args
            Base = base'
            Db = db
        }
    member this.Boot = this.Base.Boot
    member this.Env = this.Base.Env

let mutable private instance : Model option = None
let getInstance () = instance |> Option.get

let init (args : Args) : Model =
    let app = Simple.init args.Base
    let model =
        Db.init app.Env args.Db
        |> Model.Create args app
    Async.AwaitTask <| args.SetupAsync model
    |> Async.RunSynchronously
    instance <- Some model
    getInstance ()

let noSetupAsync (app : Model) : Task<unit> = task {
    return ()
}
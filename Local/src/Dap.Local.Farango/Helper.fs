[<AutoOpen>]
module Dap.Local.Farango.Helper

type IDbPack with
    member this.Db = this.FarangoDb.Actor.State
    member this.Conn = this.Db.Conn
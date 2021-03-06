module Dap.Local.Farango.Builder

open Dap.Prelude
open Dap.Context
open Dap.Context.Builder

(*
 * Generated: <ValueBuilder>
 *)
type DbArgsBuilder () =
    inherit ObjBuilder<DbArgs> ()
    override __.Zero () = DbArgs.Create ()
    [<CustomOperation("uri")>]
    member __.Uri (target : DbArgs, (* DbArgs *) uri : string) =
        target.WithUri uri

let db_args = new DbArgsBuilder ()
[<AutoOpen>]
module Dap.Eto.Layout.Const

[<Literal>]
let Horizontal_Stack = "horizontal_stack"

[<Literal>]
let Vertical_Stack = "vertical_stack"

type LayoutKind =
    | Stack
    | Panel

let getKind layout : LayoutKind =
    match layout with
    | Horizontal_Stack -> Stack
    | Vertical_Stack -> Stack
    | _ -> Panel
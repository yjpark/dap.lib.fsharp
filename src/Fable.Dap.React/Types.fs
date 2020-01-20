[<AutoOpen>]
module Dap.React.Types

type IRoute =
    abstract Url : string with get

type Widget = Fable.React.ReactElement

[<Literal>]
let DefaultMarginPx = 12

type W = DapReactWidgets
with
    static member Space
        (
            ?px : int
        ) =
        let px = defaultArg px DefaultMarginPx
        R.span [
            P.Style [
                P.CSSProp.MarginRight (sprintf "%ipx" px)
            ]
        ] []



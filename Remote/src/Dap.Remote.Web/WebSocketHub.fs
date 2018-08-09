module Dap.Remote.Web.WebSocketHub

open Saturn
open Giraffe

open Dap.Prelude
open Dap.Platform
open Dap.Remote.WebSocketHub

let run (port : int) (path : string) (serviceKind : Kind) (env : IEnv) =
    let webRouter = router {
        not_found_handler (text "Api 404")
    }

    let webApp = application {
        url (sprintf "http://0.0.0.0:%d/" port)
        use_router webRouter
        use_static "wwwroot"
        app_config (useWebSocketHub path env serviceKind)
    }

    run webApp
    0

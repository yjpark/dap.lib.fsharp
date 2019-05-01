[<RequireQualifiedAccess>]
module Dap.Remote.Dashboard.Operator

open Dap.Prelude
open Dap.Context
open Dap.Platform
open Dap.Remote

type AuthReq = JsonString

(*
 * Generated: <Record>
 *     IsJson
 *)
type AuthRes = {
    Info : (* AuthRes *) Json
} with
    static member Create
        (
            ?info : (* AuthRes *) Json
        ) : AuthRes =
        {
            Info = (* AuthRes *) info
                |> Option.defaultWith (fun () -> (decodeJsonValue D.json """null"""))
        }
    static member SetInfo ((* AuthRes *) info : Json) (this : AuthRes) =
        {this with Info = info}
    static member JsonEncoder : JsonEncoder<AuthRes> =
        fun (this : AuthRes) ->
            E.object [
                "info", E.json (* AuthRes *) this.Info
            ]
    static member JsonDecoder : JsonDecoder<AuthRes> =
        D.object (fun get ->
            {
                Info = get.Required.Field (* AuthRes *) "info" D.json
            }
        )
    static member JsonSpec =
        FieldSpec.Create<AuthRes> (AuthRes.JsonEncoder, AuthRes.JsonDecoder)
    interface IJson with
        member this.ToJson () = AuthRes.JsonEncoder this
    interface IObj
    member this.WithInfo ((* AuthRes *) info : Json) =
        this |> AuthRes.SetInfo info

(*
 * Generated: <Union>
 *     IsJson
 *)
type AuthErr =
    | InvalidToken
with
    static member Create () : AuthErr =
        InvalidToken
    static member JsonSpec' : CaseSpec<AuthErr> list =
        [
            CaseSpec<AuthErr>.Create ("InvalidToken", [])
        ]
    static member JsonEncoder = E.union AuthErr.JsonSpec'
    static member JsonDecoder = D.union AuthErr.JsonSpec'
    static member JsonSpec =
        FieldSpec.Create<AuthErr> (AuthErr.JsonEncoder, AuthErr.JsonDecoder)
    interface IJson with
        member this.ToJson () = AuthErr.JsonEncoder this

type InspectReq = JsonNil

type InspectRes = EnvSnapshot

(*
 * Generated: <Union>
 *     IsJson
 *)
type InspectErr =
    | PermissionDenied
with
    static member Create () : InspectErr =
        PermissionDenied
    static member JsonSpec' : CaseSpec<InspectErr> list =
        [
            CaseSpec<InspectErr>.Create ("PermissionDenied", [])
        ]
    static member JsonEncoder = E.union InspectErr.JsonSpec'
    static member JsonDecoder = D.union InspectErr.JsonSpec'
    static member JsonSpec =
        FieldSpec.Create<InspectErr> (InspectErr.JsonEncoder, InspectErr.JsonDecoder)
    interface IJson with
        member this.ToJson () = InspectErr.JsonEncoder this

(*
 * Generated: <Union>
 *     IsJson
 *)
type Evt =
    | OnNewAgent of agent : AgentSnapshot
with
    static member Create agent : Evt =
        OnNewAgent (agent)
    static member JsonSpec' : CaseSpec<Evt> list =
        [
            CaseSpec<Evt>.Create ("OnNewAgent", [
                AgentSnapshot.JsonSpec
            ])
        ]
    static member JsonEncoder = E.union Evt.JsonSpec'
    static member JsonDecoder = D.union Evt.JsonSpec'
    static member JsonSpec =
        FieldSpec.Create<Evt> (Evt.JsonEncoder, Evt.JsonDecoder)
    interface IJson with
        member this.ToJson () = Evt.JsonEncoder this
    interface IEvent with
        member this.Kind = Union.getKind<Evt> this

(*
 * Generated: <Union>
 *     IsJson
 *)
type Req =
    | DoAuth of req : AuthReq
    | DoInspect of req : InspectReq
with
    static member CreateDoAuth req : Req =
        DoAuth (req)
    static member CreateDoInspect req : Req =
        DoInspect (req)
    static member JsonSpec' : CaseSpec<Req> list =
        [
            CaseSpec<Req>.Create ("DoAuth", [
                AuthReq.JsonSpec
            ])
            CaseSpec<Req>.Create ("DoInspect", [
                InspectReq.JsonSpec
            ])
        ]
    static member JsonEncoder = E.union Req.JsonSpec'
    static member JsonDecoder = D.union Req.JsonSpec'
    static member JsonSpec =
        FieldSpec.Create<Req> (Req.JsonEncoder, Req.JsonDecoder)
    interface IJson with
        member this.ToJson () = Req.JsonEncoder this
    interface IRequest with
        member this.Kind = Union.getKind<Req> this

type ClientRes =
    | OnAuth of AuthReq * StubResult<AuthRes, AuthErr>
    | OnInspect of InspectReq * StubResult<InspectRes, InspectErr>
with
    static member StubSpec : Stub.ResponseSpec<ClientRes> list =
        [
            Stub.ResponseSpec<ClientRes>.Create ("DoAuth", [AuthReq.JsonSpec],
                "OnAuth", AuthRes.JsonDecoder, AuthErr.JsonDecoder)
            Stub.ResponseSpec<ClientRes>.Create ("DoInspect", [InspectReq.JsonSpec],
                "OnInspect", InspectRes.JsonDecoder, InspectErr.JsonDecoder)
        ]

type ServerReq =
    | DoAuth of AuthReq * Callback<Result<AuthRes, AuthErr>>
    | DoInspect of InspectReq * Callback<Result<InspectRes, InspectErr>>
with
    static member HubSpec : Hub.RequestSpec<ServerReq> list =
        [
            Hub.RequestSpec<ServerReq>.Create ("DoAuth", [AuthReq.JsonSpec],
                Hub.getCallback<AuthRes, AuthErr>)
            Hub.RequestSpec<ServerReq>.Create ("DoInspect", [InspectReq.JsonSpec],
                Hub.getCallback<InspectRes, InspectErr>)
        ]
    interface IReq

let StubSpec : StubSpec<Req, ClientRes, Evt> =
    {
        Response = ClientRes.StubSpec
        Event = Evt.JsonSpec'
    }
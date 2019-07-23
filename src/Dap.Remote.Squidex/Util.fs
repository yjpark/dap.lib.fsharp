[<AutoOpen>]
module Dap.Remote.Squidex.Util

open FSharp.Data

open Dap.Prelude
open Dap.Context
open Dap.Platform

[<Literal>]
let Invariant = "iv"

[<Literal>]
let TabStr = "  "

let getPrefix (tabs : int) =
    [0..tabs]
    |> List.map (fun _ -> "")
    |> String.concat TabStr

type SquidexItem with
    static member UnwrapDataFields (fields : ContentField list) : ContentField list =
        fields
        |> List.choose (fun field ->
            match field with
            | SimpleChild (key, fields) ->
                if (key = "data") then
                    Some fields
                else
                    None
            | _ ->
                None
        )|> List.tryHead
        |> Option.defaultValue []

type ContentField with
    static member FieldsToQuery
            (selfLang : string option) (tabs : int) (lang : string)
            (key : string) (fields : ContentField list)
                : string list =
        let prefix = getPrefix tabs
        [
            yield sprintf "%s%s {" prefix key
            if selfLang.IsSome then
                yield sprintf "%s  %s {" prefix selfLang.Value
            let extra_tabs = if selfLang.IsSome then 2 else 1
            for field in fields do
                for line in (field.ToQuery (tabs + extra_tabs) lang) do
                    yield line
            if selfLang.IsSome then
                yield sprintf "%s  }" prefix
            yield sprintf "%s}" prefix
        ]

    member this.ToQuery (tabs : int) (lang : string) : string list =
        let prefix = getPrefix tabs
        match this with
        | NoField -> []
        | SimpleValue (key, _spec) ->
            [sprintf "%s%s" prefix key]
        | InvariantValue (key, _spec) ->
            [sprintf "%s%s { %s }" prefix key Invariant]
        | LocalizedValue (key, _spec) ->
            [sprintf "%s%s { %s }" prefix key lang]
        | SimpleChild (key, fields)
        | SimpleArray (key, fields) ->
            ContentField.FieldsToQuery None tabs lang key fields
        | InvariantChild (key, fields)
        | InvariantArray (key, fields) ->
            ContentField.FieldsToQuery (Some Invariant) tabs lang key fields
        | LocalizedChild (key, fields)
        | LocalizedArray (key, fields) ->
            ContentField.FieldsToQuery (Some lang) tabs lang key fields
    member this.TryFlattenValue
            (lang : string option,
                key : string, spec : FieldSpec,
                data : Json, errors : byref<string list>) =
        let decoder =
            match lang with
            | None -> spec.Decoder
            | Some lang -> D.field lang spec.Decoder
        let result =
            data
            |> tryCastJson (D.field key decoder)
            |> Result.bind (fun v ->
                try
                    Ok (key, spec.Encoder v)
                with e ->
                    Error <| e.Message
            )
        match result with
            | Ok (k, v) -> Some (k, v)
            | Error err ->
                errors <- err :: errors
                None
    member this.TryFlattenChild
            (selfLang : string option, lang : string,
                key : string, fields : ContentField list,
                data : Json, errors : byref<string list>) =
        let decoder =
            match selfLang with
            | None -> D.json
            | Some lang -> D.field lang D.json
        let result =
            data
            |> tryCastJson (D.field key decoder)
        match result with
        | Ok fieldData ->
            let mutable fieldErrors = []
            let result =
                Some (key, E.object [
                    for field in fields do
                        match field.TryFlatten (lang, fieldData, &fieldErrors) with
                        | Some (k, v) -> yield (k, v)
                        | None -> ()
                ])
            errors <- fieldErrors @ errors
            result
        | Error err ->
            errors <- err :: errors
            None
    member this.TryFlattenArray
            (selfLang : string option, lang : string,
                key : string, fields : ContentField list,
                data : Json, errors : byref<string list>) =
        let dataFields = SquidexItem.UnwrapDataFields fields
        let decoder =
            match dataFields with
            | [] -> D.json
            | _ -> D.field "data" D.json
        let decoder =
            match selfLang with
            | None -> D.array decoder
            | Some lang -> D.field lang (D.array decoder)
        let result =
            data
            |> tryCastJson (D.field key decoder)
        match result with
        | Ok fieldArray ->
            let mutable fieldErrors = []
            let encoder = fun (fieldData : Json) ->
                let fields' =
                    match dataFields with
                    | [] -> fields
                    | _ -> dataFields
                E.object [
                    for field in fields' do
                        match field.TryFlatten (lang, fieldData, &fieldErrors) with
                        | Some (k, v) -> yield (k, v)
                        | None -> ()
                ]
            let result =
                Some (key, E.array encoder fieldArray)
            errors <- fieldErrors @ errors
            result
        | Error err ->
            errors <- err :: errors
            None
    member this.TryFlatten (lang : string, data : Json, errors : byref<string list>) : (string * Json) option =
        match this with
        | NoField -> None
        | SimpleValue (key, spec) ->
            this.TryFlattenValue (None, key, spec, data, &errors)
        | InvariantValue (key, spec) ->
            this.TryFlattenValue (Some Invariant, key, spec, data, &errors)
        | LocalizedValue (key, spec) ->
            this.TryFlattenValue (Some lang, key, spec, data, &errors)
        | SimpleChild (key, fields) ->
            this.TryFlattenChild (None, lang, key, fields, data, &errors)
        | InvariantChild (key, fields) ->
            this.TryFlattenChild (Some Invariant, lang, key, fields, data, &errors)
        | LocalizedChild (key, fields) ->
            this.TryFlattenChild (Some lang, lang, key, fields, data, &errors)
        | SimpleArray (key, fields) ->
            this.TryFlattenArray (None, lang, key, fields, data, &errors)
        | InvariantArray (key, fields) ->
            this.TryFlattenArray (Some Invariant, lang, key, fields, data, &errors)
        | LocalizedArray (key, fields) ->
            this.TryFlattenArray (Some lang, lang, key, fields, data, &errors)

type SquidexConfig with
    member this.GraphqlUrl =
        sprintf "%s/api/content/%s/graphql" this.Url this.App
    member this.AuthorizationHeader =
        ("Authorization", sprintf "Bearer %s" this.Token)
    member this.Headers (extraHeaders : seq<string * string> option) =
        [
            this.AuthorizationHeader
            ("Content-Type", "application/json")
        ] @ (extraHeaders
        |> Option.map List.ofSeq
        |> Option.defaultValue [])
        |> Seq.ofList

type ContentsQuery with
    member this.ToQuery
            (getQueryName : string -> string) =
        let param =
            [
                yield sprintf "top: %i" this.Top
                yield sprintf "skip: %i" this.Skip
                if this.Filter.IsSome then
                    yield sprintf "filter: %s" this.Filter.Value
                if this.Orderby.IsSome then
                    yield sprintf "orderby: %s" this.Orderby.Value
            ]|> String.concat ", "
        let fields =
            [
                for field in this.Fields do
                    for line in (field.ToQuery 1 this.Lang) do
                        yield line
            ]
        [
            "{"
            sprintf "  %s (%s) {" (getQueryName this.Schema) param
        ] @ fields @ [
            "  }"
            "}"
        ] |> String.concat "\n"
    member this.ToBody
            (getQueryName : string -> string) =
        let query = this.ToQuery getQueryName
        E.object [
            "query", E.string query
        ]|> E.encode 4
        |> TextRequest

type SquidexItem with
    static member WrapFields (fields : ContentField list) =
        [
            ContentField.CreateSimpleValue "id" S.string
            ContentField.CreateSimpleValue "version" S.int
            ContentField.CreateSimpleValue "created" S.dateTime
            ContentField.CreateSimpleValue "createdBy" S.string
            ContentField.CreateSimpleValue "lastModified" S.dateTime
            ContentField.CreateSimpleValue "lastModifiedBy" S.string
            ContentField.CreateSimpleValue "url" S.string
            ContentField.CreateSimpleChild "data" fields
        ]
    static member WrapContentsQuery (withTotal : bool) (query : ContentsQuery) : ContentsQuery =
        let fields = SquidexItem.WrapFields query.Fields
        let fields =
            if withTotal then
                [
                    ContentField.CreateSimpleValue "total" S.int
                    ContentField.CreateSimpleChild "items" fields
                ]
            else
                fields
        {query with Fields = fields}
    static member JsonDecoderWithFlatten (query : ContentsQuery) : JsonDecoder<SquidexItem> =
        fun path json ->
            SquidexItem.JsonDecoder path json
            |> Result.map (fun x -> x.WithDataFlatten query)
    member this.WithDataFlatten (query : ContentsQuery) =
        {this with DataFlatten = this.CalcDataFlatten query}
    member this.CalcDataFlatten (query : ContentsQuery) : Json =
        let mutable errors = []
        let result =
            E.object [
                for field in query.Fields do
                    match field.TryFlatten (query.Lang, this.Data, &errors) with
                    | Some (k, v) -> yield (k, v)
                    | None -> ()
            ]
        if (errors.Length > 0) then
            let logger = getLogger "SquidexItem.CalcDataFlatten"
            logError logger "Flatten_With_Errors" query.Schema
                (query.ToQuery, E.encode 4 this.Data, errors)
        result

type Squidex = SquidexHelper with
    static member SimpleValue key spec : ContentField =
        ContentField.CreateSimpleValue key spec
    static member InvariantValue key spec : ContentField =
        ContentField.CreateInvariantValue key spec
    static member LocalizedValue key spec : ContentField =
        ContentField.CreateLocalizedValue key spec
    static member SimpleChild key fields : ContentField =
        ContentField.CreateSimpleChild key fields
    static member InvariantChild key fields : ContentField =
        ContentField.CreateInvariantChild key fields
    static member LocalizedChild key fields : ContentField =
        ContentField.CreateLocalizedChild key fields
    static member SimpCreateSimpleChildleArray key fields : ContentField =
        ContentField.CreateSimpleArray key (SquidexItem.WrapFields fields)
    static member InvariantArray key fields : ContentField =
        ContentField.CreateInvariantArray key (SquidexItem.WrapFields fields)
    static member LocalizedArray key fields : ContentField =
        ContentField.CreateLocalizedArray key (SquidexItem.WrapFields fields)

type ContentsWithTotalResult with
    static member JsonDecoderWithFlatten (query : ContentsQuery) : JsonDecoder<ContentsWithTotalResult> =
        fun path json ->
            ContentsWithTotalResult.JsonDecoder path json
            |> Result.map (fun x ->
                let items =
                    x.Items
                    |> List.map (fun y -> y.WithDataFlatten query)
                {x with Items = items}
            )

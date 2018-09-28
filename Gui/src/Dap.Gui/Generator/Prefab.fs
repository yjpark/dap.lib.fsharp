[<AutoOpen>]
module Dap.Gui.Generator.Prefab

open Dap.Prelude
open Dap.Context
open Dap.Context.Meta
open Dap.Context.Generator
open Dap.Context.Generator.Util
open Dap.Gui

type Generator (gui : IGui, meta : IWidget) =
    let getParentPrefab () =
        gui.GetPrefab meta <| (meta.GetType ()) .Name
    let getChildPrefab (child : IWidget) =
        if child.Prefab.Value <> "" then
            child.Prefab.Value.AsCodeMemberName
        else
            gui.GetPrefab child <| (child.GetType ()) .Name
    let getAliases (_param : PrefabParam) =
        gui.Aliases
        |> List.map (fun alias ->
            sprintf "module %s = %s" (fst alias) (snd alias)
        )
    let getJson (param : PrefabParam) =
        [
            sprintf "let %sJson = parseJson \"\"\"" param.Name
            E.encodeJson 4 meta
            sprintf "\"\"\""
        ]
    let getClassHeader (param : PrefabParam) =
        [
            sprintf "type Prefab (owner : IOwner, key : Key) as this ="
            sprintf "    inherit %s.Prefab (owner, key)" <| getParentPrefab ()
        ]
    let getChildAdder (key : string) (child : IWidget) =
        let prefab = getChildPrefab child
        sprintf "    let %s = %s.Prefab.AddToCombo \"%s\" this.Children" key.AsCodeVariableName prefab key
    let getClassFields (param : PrefabParam) =
        [
            match meta with
            | :? IGroup as group ->
                for kv in group.Children.Value do
                    match kv.Value with
                    | :? IWidget as prop ->
                        yield getChildAdder kv.Key prop
                    | _ ->
                        ()
            | _ ->
                ()
        ]
    let getChildSetup (key : string) (child : IWidget) =
        let prefab = getChildPrefab child
        sprintf "        this.AddChild (%s.Widget)" key.AsCodeVariableName
    let getClassSetup (param : PrefabParam) =
        [
            yield "    do ("
            yield sprintf "        this.AsProperty.WithJson %sJson |> ignore" param.Name
            match meta with
            | :? IGroup as group ->
                for kv in group.Children.Value do
                    match kv.Value with
                    | :? IWidget as prop ->
                        yield getChildSetup kv.Key prop
                    | _ ->
                        ()
            | _ ->
                ()
            yield "    )"
        ]
    let getClassMiddle (param : PrefabParam) =
        [
            "    static member Create o k = new Prefab (o, k)"
            "    static member Default () = Prefab.Create noOwner NoKey"
            "    static member AddToCombo key (combo : IComboProperty) ="
            "        combo.AddCustom<Prefab> (Prefab.Create, key)"
        ]
    let getChildMember (key : string) (child : IWidget) =
        let prefab = getChildPrefab child
        sprintf "    member __.%s : %s.Prefab = %s" key.AsCodeMemberName prefab key.AsCodeVariableName
    let getClassMembers (param : PrefabParam) =
        [
            match meta with
            | :? IGroup as group ->
                for kv in group.Children.Value do
                    match kv.Value with
                    | :? IWidget as prop ->
                        yield getChildMember kv.Key prop
                    | _ ->
                        ()
            | _ ->
                ()
        ]
    interface IGenerator<PrefabParam> with
        member this.Generate param =
            [
                gui.Opens
                getAliases param
                [""]
                getJson param
                [""]
                getClassHeader param
                getClassFields param
                getClassSetup param
                getClassMiddle param
                getClassMembers param
            ]|> List.concat
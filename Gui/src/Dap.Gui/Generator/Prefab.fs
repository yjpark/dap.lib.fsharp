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
    let getKind (param : PrefabParam) =
        [
            sprintf "[<Literal>]"
            sprintf "let Kind = \"%s\"" param.Name
        ]
    let getJson (param : PrefabParam) =
        [
            sprintf "let Json = parseJson \"\"\""
            E.encodeJson 4 meta
            sprintf "\"\"\""
        ]
    let getClassHeader (param : PrefabParam) =
        let parent = getParentPrefab ()
        [
            sprintf "type Model = %s.Model" parent
            sprintf "type Widget = %s.Model" parent
            sprintf ""
            sprintf "type Prefab (logging : ILogging) ="
            sprintf "    inherit %s.Prefab (logging)" parent
        ]
    let getChildAdder (child : IWidget) =
        let key = child.Spec.Key
        let prefab = getChildPrefab child
        sprintf "    let %s = %s.Prefab.AddToGroup logging \"%s\" base.Model" key.AsCodeVariableName prefab key
    let getClassFields (param : PrefabParam) =
        [
            match meta with
            | :? IGroup as group ->
                for prop in group.Children.Value do
                    match prop with
                    | :? IWidget as prop ->
                        yield getChildAdder prop
                    | _ ->
                        ()
            | _ ->
                ()
        ]
    let getChildSetup (child : IWidget) =
        let key = child.Spec.Key
        let prefab = getChildPrefab child
        sprintf "        base.AddChild (%s.Widget)" key.AsCodeVariableName
    let getClassSetup (param : PrefabParam) =
        [
            yield sprintf "    do ("
            yield sprintf "        base.Model.AsProperty.WithJson Json |> ignore"
            match meta with
            | :? IGroup as group ->
                for prop in group.Children.Value do
                    match prop with
                    | :? IWidget as prop ->
                        yield getChildSetup prop
                    | _ ->
                        ()
            | _ ->
                ()
            yield "    )"
        ]
    let getClassMiddle (param : PrefabParam) =
        [
            "    static member Create l = new Prefab (l)"
            "    static member Create () = new Prefab (getLogging ())"
            "    static member AddToGroup l key (group : IGroup) ="
            "        let prefab = Prefab.Create l"
            "        group.Children.AddLink<Model> (prefab.Model, key) |> ignore"
            "        prefab"

        ]
    let getChildMember (child : IWidget) =
        let key = child.Spec.Key
        let prefab = getChildPrefab child
        sprintf "    member __.%s : %s.Prefab = %s" key.AsCodeMemberName prefab key.AsCodeVariableName
    let getClassMembers (param : PrefabParam) =
        [
            match meta with
            | :? IGroup as group ->
                for prop in group.Children.Value do
                    match prop with
                    | :? IWidget as prop ->
                        yield getChildMember prop
                    | _ ->
                        ()
            | _ ->
                ()
        ]
    interface IGenerator<PrefabParam> with
        member this.Generate param =
            [
                ["open Dap.Gui"]
                gui.Opens
                getAliases param
                [""]
                getKind param
                [""]
                getJson param
                [""]
                getClassHeader param
                getClassFields param
                getClassSetup param
                getClassMiddle param
                getClassMembers param
            ]|> List.concat
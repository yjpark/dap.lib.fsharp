# PREFAB_HEADER #
```F#
type Prefab (logging : ILogging) =
    inherit BasePrefab<Prefab, Model, Widget>
        (logging, Kind, Model.Create, new Widget ())
    do (
        let owner = base.AsOwner
        let model = base.Model
        let widget = base.Widget
```

# PREFAB_FOOTER #
```F#
static member Create l = new Prefab (l)
static member Create () = new Prefab (getLogging ())
static member AddToGroup l key (group : IGroup) =
    let prefab = Prefab.Create l
    group.Children.AddLink<Model> (prefab.Model, key) |> ignore
    prefab
override this.Self = this
override __.Spawn l = Prefab.Create l
```
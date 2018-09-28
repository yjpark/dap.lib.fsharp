# PREFAB_MIXIN #
```F#
static member Create o k = new Prefab (o, k)
static member Default () = Prefab.Create noOwner NoKey
static member AddToCombo key (combo : IComboProperty) =
    combo.AddCustom<Prefab> (Prefab.Create, key)
member __.Widget = widget
member __.Widget' = widget :> Control
interface IPrefab<Widget> with
    member this.Widget = this.Widget
member this.AsPrefab = this :> IPrefab<Widget>
```

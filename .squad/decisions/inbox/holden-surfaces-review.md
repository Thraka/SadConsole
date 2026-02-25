# Decision Note: architecture-surfaces.md Has Interface Stub Errors

**From:** Holden  
**Date:** 2026-02-25  
**Re:** `docs/architecture-surfaces.md` — inaccuracies found during review  
**Review file:** `docs/architecture-surfaces-review.md`

---

## Flag for Deckard

The surfaces architecture doc is broadly correct and a solid read, but the `IRenderer` and `IRenderStep` interface stubs contain factual errors that will mislead any developer trying to implement or extend those types. These are not paraphrase issues — the signatures are wrong.

### Highest-priority fixes

1. **`IRenderer.OnHostUpdated` wrong parameter type**  
   Doc shows `void OnHostUpdated(IScreenSurface surface)`.  
   Code has `void OnHostUpdated(IScreenObject host)`.  
   `IRenderer.cs:53` — parameter is `IScreenObject`, not `IScreenSurface`.

2. **`IRenderStep.Refresh` missing two parameters**  
   Doc shows `bool Refresh(IRenderer renderer, IScreenSurface screenObject)`.  
   Code has `bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)`.  
   `IRenderStep.cs:39`

3. **`IRenderer` interface stub is incomplete**  
   - `ITexture? Output` in doc is non-nullable `ITexture Output` in code  
   - `Steps { get; }` in doc is `{ get; set; }` in code  
   - Doc omits `string Name`, `byte Opacity`, `bool IsForced`  
   `IRenderer.cs`

### Secondary issues (route to doc author)

- Resize section says `Effects.DropInvalidCells()` always runs, but when `clear=true` it's `Effects.RemoveAll()` — `CellSurface.cs:410–413`
- Dirty chain diagram says `IsDirtySet` is "used by EffectsManager internally" — it isn't; nothing in `EffectsManager.cs` subscribes to `IsDirtySet`
- `SetSurface(...)` in the surface-replacement paragraph refers to `ICellSurfaceSettable.SetSurface` (remaps cell array) not an `ICellSurface` reference swap — conflates two different operations
- `LayeredScreenSurface` renderer claim: doc says subclasses override `DefaultRendererName`; `LayeredScreenSurface` actually disposes and directly assigns the renderer in its constructor

See the full review at `docs/architecture-surfaces-review.md` for verified items and what couldn't be confirmed from core sources alone.

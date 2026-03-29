# Decisions Archive — SadConsole Squad

**Archive period:** Entries older than 2026-02-04 (30 days before 2026-03-06)  
**Created:** 2026-03-06T0713Z

---

## Doc Verification Workflow Directive (ARCHIVED)

**Date:** 2026-02-25T14:08:44Z  
**By:** Thraka (via Copilot)  
**Category:** Process

When verifying an architecture doc, corrections should be applied directly to the source doc (e.g., `docs/architecture-surfaces.md`). Do not create intermediate verification files — the work product is the corrected doc, not a new review artifact.

**Rationale:** User request — captured for team memory.

---

## Priority Fixes for `docs/architecture-surfaces.md` (ARCHIVED)

**Date:** 2026-02-25  
**Author:** Deckard (Lead)  
**Category:** Architecture Documentation  
**Trigger:** Holden reviewed the doc; Deckard independently verified all findings against source.  
**Status:** All findings confirmed. Zero disputes. Fixes required before dev use.

### Summary

`docs/architecture-surfaces.md` contains accurate coverage of the CellSurface data model and ScreenSurface scene-graph behaviors, but its **`IRenderer` and `IRenderStep` interface stubs are wrong** in ways that would cause compilation failures or incorrect implementations for any developer working from them. Several secondary descriptions also misstate runtime semantics.

### Priority Fix List

#### P1 — Breaks implementation if followed (fix immediately before dev handoff)

**Fix 1: `IRenderStep.Refresh` signature**  
*Who:* Roy  
*Change:* Replace 2-parameter stub with 4-parameter signature:
```csharp
bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced);
```
Source: `IRenderStep.cs:39`

**Fix 2: `IRenderer.OnHostUpdated` parameter type**  
*Who:* Roy  
*Change:* Replace `IScreenSurface surface` with `IScreenObject host` in the stub.  
Source: `IRenderer.cs:53`

**Fix 3: `IRenderer` stub — nullability, accessor, and missing members**  
*Who:* Roy  
*Change:* Correct `ITexture?` → `ITexture`; correct `Steps { get; }` → `Steps { get; set; }`; add `string Name { get; set; }`, `byte Opacity { get; set; }`, `bool IsForced { get; set; }`.  
Source: `IRenderer.cs:14–34`

**Fix 4: `IRenderStep` stub — four missing members**  
*Who:* Roy  
*Change:* Add `string Name { get; }`, `void SetData(object data)`, `void Reset()`, `void OnHostUpdated(IScreenObject host)` to the documented stub.  
Source: `IRenderStep.cs:13–59`

#### P2 — Wrong semantic description (fix before any surface/renderer extension work)

**Fix 5: `SetSurface(...)` vs `Surface { set; }` distinction**  
*Who:* Deckard (architectural language) or Roy (technical detail)  
*Change:* Separate the two operations clearly. `SetSurface(...)` from `ICellSurfaceSettable` rebinds the internal cell array within a `CellSurface`. `Surface { set; }` from `ISurfaceSettable` replaces the `ICellSurface` reference held by `ScreenSurface`. They are not interchangeable alternatives.

**Fix 6: `LayeredScreenSurface` renderer selection mechanism**  
*Who:* Deckard  
*Change:* Replace "subclasses override `DefaultRendererName`" with the accurate description: the constructor disposes the default renderer and directly calls `Renderer = GameHost.Instance.GetRenderer(RendererNames.LayeredScreenSurface)`. No `DefaultRendererName` override is used.

**Fix 7: `CellSurface.Resize` effects cleanup**  
*Who:* Roy  
*Change:* Clarify that when `clear = true`, `Effects.RemoveAll()` is called (not `DropInvalidCells()`). `DropInvalidCells()` is only called when `clear = false`.

#### P3 — Misleading narrative (fix for correctness, low urgency for function)

**Fix 8: `IsDirtySet` parenthetical**  
*Who:* Roy  
*Change:* Remove or correct "(used by EffectsManager internally)". EffectsManager does not subscribe to `IsDirtySet`. It tracks cell changes via `ApplyToCell()` return values.

**Fix 9: `OnIsDirtyChanged()` "forwarding" description**  
*Who:* Roy  
*Change:* The base `ScreenSurface.OnIsDirtyChanged()` is an empty virtual no-op. The event subscription to `Surface.IsDirtyChanged` exists and calls `OnIsDirtyChanged()`, but the base does nothing with it. Replace "forwarding the signal up the display chain" with "providing a virtual hook for subclasses to respond to surface-level dirty changes."

#### P4 — Omissions (add when convenient)

**Fix 10:** Add `ICellSurface.ConnectedLineEmpty` to the static arrays table.  
**Fix 11:** Add a note that `ICellSurface` extends `IGridView<ColoredGlyphBase>` and `IEnumerable<ColoredGlyphBase>` — relevant for SadRogue interop.  
**Fix 12:** Add a note to the effects section that `EffectsManager` stores a `ColoredGlyphWithState` clone of the cell's original state for in-memory restoration on effect removal.

### Assignment

| Fix # | Owner | Priority |
|-------|-------|----------|
| 1–4   | Roy   | P1 — before any renderer extension work |
| 5–6   | Deckard / Roy | P2 |
| 7–9   | Roy   | P2–P3 |
| 10–12 | Roy   | P4 (backlog) |

Rachael: once P1 fixes are in, please spot-check the updated stubs against the actual interface files to confirm accuracy before the doc is shared externally.

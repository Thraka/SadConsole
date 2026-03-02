# Decisions — SadConsole Squad

## Doc Verification Workflow Directive

**Date:** 2026-02-25T14:08:44Z  
**By:** Thraka (via Copilot)  
**Category:** Process

When verifying an architecture doc, corrections should be applied directly to the source doc (e.g., `docs/architecture-surfaces.md`). Do not create intermediate verification files — the work product is the corrected doc, not a new review artifact.

**Rationale:** User request — captured for team memory.

---

## Priority Fixes for `docs/architecture-surfaces.md`

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

---

## RowFontSurface — Multi-Font Row Surface Architecture

**Date:** 2026-03-02T21  
**Author:** Deckard (Lead)  
**Category:** Architecture — New Surface Type  
**Status:** Specification complete. Implementation by Roy (core) and Gaff (hosts) in progress.

### Summary

A new surface type where **each row can use a different font**. Extends `ScreenSurface` without modifying existing surfaces. Enables rich typography (mixed font sizes per row) and per-row font overlays.

### Key Design Decisions

**1. No Cached Rectangles**  
Variable row heights prevent pre-caching destination rects. Renderers compute `destRect` on the fly:
```csharp
XnaRectangle destRect = new XnaRectangle(
    x * rowFontSize.X,
    rowYOffset,
    rowFontSize.X,
    rowFontSize.Y);
```

**2. Sparse Row Font Storage**  
`Dictionary<int, IFont>` and `Dictionary<int, Point>` provide sparse storage. Rows not in dictionary fall back to default `Font` and `FontSize` properties. Efficient for typical use cases where only a few rows have custom fonts.

**3. Pre-Calculated RowYOffsets Array**  
Array indexed by row number caches Y pixel offsets. Recalculated via `RecalculateRowOffsets()` whenever fonts or sizes change. Avoids redundant calculation during rendering.

**4. Variable Height Calculation**  
`HeightPixels` is no longer `View.Height * FontSize.Y`. New formula:
```csharp
public override int HeightPixels => 
    RowYOffsets[RowYOffsets.Length - 1] + GetRowHeight(RowYOffsets.Length - 1);
```

**5. Uniform Width Calculation**  
Width uses default `FontSize.X` for column alignment. All cells in a column align horizontally regardless of row font. Per-column fonts out of scope.

**6. Mouse Input Coordinate Mapping**  
`PixelToCell(Point pixelPosition)` method uses linear search through `RowYOffsets` to find which row contains Y coordinate. Necessary for variable-height row mouse input.

**7. Resize Behavior**  
Override `Resize()` to call `RecalculateRowOffsets()`. User responsible for managing `RowFonts` and `RowFontSizes` dictionaries if rows are added/removed.

**8. Serialization via Font Names**  
`RowFonts` and `RowFontSizes` marked with `[DataMember]` and `[JsonConverter(typeof(RowFontDictionaryConverter))]`. Fonts serialized by name, resolved from `GameHost.Fonts` at load time.

### Core Implementation (Roy)

**File:** `SadConsole/RowFontSurface.cs`  
**Extends:** `ScreenSurface`

**Key Properties:**
- `RowFonts: Dictionary<int, IFont>` — row to font mapping
- `RowFontSizes: Dictionary<int, Point>` — row to font size mapping
- `RowYOffsets: int[]` — cached Y pixel offsets per row

**Key Methods:**
- `SetRowFont(int row, IFont font, Point? fontSize = null)` — assign font to row
- `GetRowFont(int row): IFont` — get font with fallback
- `GetRowFontSize(int row): Point` — get size with fallback
- `RecalculateRowOffsets()` — pre-calculate Y offsets
- `GetRowYOffset(int row): int` — O(1) lookup
- `GetRowHeight(int row): int` — row height in pixels
- `PixelToCell(Point pixelPosition): Point` — mouse coordinate mapping

**Constants:** `SadConsole/Renderers/Constants.cs`
- `RendererNames.RowFontSurface = "rowfontsurface"`
- `RenderStepNames.RowFontSurface = "rowfontsurface"`
- `RenderStepSortValues.RowFontSurface = 50`

### Host Implementations (Gaff)

**MonoGame Host:**
- `SadConsole.Host.MonoGame/Renderers/RowFontSurfaceRenderer.cs`
- `SadConsole.Host.MonoGame/Renderers/Steps/RowFontSurfaceRenderStep.cs`
- Registration in `Game.Mono.cs`

**SFML Host:**
- `SadConsole.Host.SFML/Renderers/RowFontSurfaceRenderer.cs`
- `SadConsole.Host.SFML/Renderers/Steps/RowFontSurfaceRenderStep.cs`
- Registration in `Game.cs`

**FNA Host:**
- Shares MonoGame via compile includes; no separate files needed

**Renderer Pattern:**
1. Clear base `ScreenSurfaceRenderer` steps
2. Add `RowFontSurfaceRenderStep` (draws surface)
3. Add `OutputSurfaceRenderStep` (blits to output)
4. Add `TintSurfaceRenderStep` (applies tint)

**Render Loop Structure:**
```csharp
for (int y = 0; y < surface.View.Height; y++)
{
    IFont rowFont = rowFontSurface.GetRowFont(y);
    Point rowFontSize = rowFontSurface.GetRowFontSize(y);
    int rowYOffset = rowFontSurface.GetRowYOffset(y);
    
    for (int x = 0; x < surface.View.Width; x++)
    {
        Rectangle destRect = new Rectangle(
            x * rowFontSize.X,
            rowYOffset,
            rowFontSize.X,
            rowFontSize.Y);
        // Draw background, glyph, decorators...
    }
}
```

### Performance Implications

- **No cached rects:** Small performance hit vs. `ScreenSurfaceRenderer` due to on-the-fly rect calculation
- **Per-row font lookups:** Dictionary lookups on every row — negligible for typical row counts (< 100)
- **Multiple texture switches:** If rows use different fonts, texture switching per row — acceptable for < 5 unique fonts

Future optimization: Batch rows by font to minimize texture switches.

### Testing Strategy (Rachael)

1. Basic multi-font rendering — 3 rows with different fonts
2. Height calculation — `HeightPixels` matches sum of row heights
3. Mouse input — `PixelToCell` returns correct coordinates
4. Viewport scrolling — rows render correctly when scrolled
5. Runtime font change — `SetRowFont` triggers re-render
6. Surface resize — `RowYOffsets` recalculates correctly
7. Serialization — custom row fonts persist/restore
8. Fallback behavior — unset rows use default font

### Related Documents

- Specification: `.squad/decisions/inbox/deckard-multifont-surface.md` (detailed)
- Orchestration logs: `.squad/orchestration-log/2026-03-02T21-{deckard,roy,gaff}.md`
- Session log: `.squad/log/2026-03-02T21-rowfontsurface.md`

### Sign-Off

**Deckard (Lead):** Specification ready for implementation.  
**Roy (Core Dev):** Implementation in progress. Builds clean.  
**Gaff (Host Dev):** Implementation in progress. All hosts build clean.

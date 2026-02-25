# Review: architecture-surfaces.md
**Reviewer:** Holden  
**Date:** 2026-02-25  
**Status:** NEEDS REVISION

## Summary
The document is broadly accurate and a solid architectural overview, but contains several inaccuracies in the `IRenderer` and `IRenderStep` interface stubs that would cause confusion for anyone trying to implement or extend those types. A handful of smaller behavioral claims also need correction.

---

## Findings

### ✅ Verified

- `CellSurface` class declaration `public class CellSurface : ICellSurface, ICellSurfaceResize, ICellSurfaceSettable` — confirmed at `CellSurface.cs:15`
- All properties in the Data Model table (`Cells`, `_viewArea`, `DefaultForeground/Background`, `DefaultGlyph`, `IsDirty`, `Effects`, `UsePrintProcessor`, `TimesShiftedUp/Down/Left/Right`) — confirmed at `CellSurface.cs:17–159`
- Three indexers (`[x,y]`, `[index]`, `[Point]`) with `protected` setters; Range span indexer has no setter — confirmed at `CellSurface.cs:167–195`
- `ColoredGlyphBase` property list (Foreground, Background, Glyph, Mirror, IsVisible, Decorators, IsDirty) — confirmed at `ColoredGlyphBase.cs:29–96`
- `IsDirty` property on `CellSurface` guards on value change and calls `OnIsDirtyChanged()` which fires `IsDirtyChanged` event — confirmed at `CellSurface.cs:61–70, 479–480`
- `SetIsDirtySafe()` sets backing field without firing event — confirmed at `CellSurface.cs:473–474`
- `ColoredGlyphBase.IsDirty` raises `IsDirtySet` event when set to `true` — confirmed at `ColoredGlyphBase.cs:100–101`
- Viewport properties (`Area`, `View`, `ViewPosition`, `ViewWidth`, `ViewHeight`, `IsScrollable`) all present with documented semantics — confirmed at `CellSurface.cs:94–155`, `ICellSurface.cs:50–96`
- All `CellSurfaceEditor` extension methods documented are present on `ISurface` (`SetGlyph`, `SetForeground`, `SetBackground`, `SetMirror`, `SetCellAppearance`, `Print`, `Fill`, `Clear`, `Erase`, `Copy`, `GetSubSurface`, `ShiftUp/Down/Left/Right`, `ShiftRow`, `ShiftColumn`, `DrawLine`, `DrawBox`, `DrawCircle`, `ConnectLines`, `SetEffect`) — confirmed at `ICellSurface.Editor.cs`
- `ICellSurfaceResize` exposes exactly the two `Resize` overloads documented — confirmed at `ICellSurfaceResize.cs:17–26`
- `EffectsManager` dual-dictionary structure (`_effects` keyed by `ICellEffect`, `_effectCells` keyed by `ColoredGlyphBase`) — confirmed at `EffectsManager.cs:20–25`
- `EffectsManager.UpdateEffects` sets `_backingSurface.IsDirty = true` when `ApplyToCell` returns true — confirmed at `EffectsManager.cs:287`
- `EffectsManager.DropInvalidCells()` exists and prunes invalid cells — confirmed at `EffectsManager.cs:178–194`
- `ScreenSurface` class declaration `public partial class ScreenSurface : ScreenObject, IDisposable, IScreenSurfaceEditable, ISurfaceSettable, ICellSurfaceResize` — confirmed at `ScreenSurface.cs:16`
- `ScreenSurface` members (Surface, Renderer, Font, FontSize, Tint, UsePixelPositioning) — confirmed at `ScreenSurface.cs:79–171`
- Mouse events (MouseEnter, MouseExit, MouseMove, MouseButtonClicked) declared in `ScreenSurface.Input.cs` — confirmed at lines 15–24
- `QuietSurfaceHandling` suppresses `IsDirtyChanged` subscription — confirmed at `ScreenSurface.cs:62–74`
- `ScreenSurface` subscribes `_isDirtyChangedEventHandler` to `Surface.IsDirtyChanged`, which calls virtual `OnIsDirtyChanged()` — confirmed at `ScreenSurface.cs:370–376`
- `ScreenSurface.Render` pseudocode is accurate (components → renderer → children) — confirmed at `ScreenSurface.cs:335–357`
- `ScreenSurface.Update` calls `Surface.Effects.UpdateEffects(delta)` then `base.Update(delta)` — confirmed at `ScreenSurface.cs:362–368`
- `ScreenObject` contributes five component lists (`ComponentsUpdate`, `ComponentsRender`, `ComponentsKeyboard`, `ComponentsMouse`, `ComponentsEmpty`) — confirmed at `ScreenObject.IComponentHost.cs:14–35`
- `ScreenSurface.UpdateAbsolutePosition` code matches doc — confirmed at `ScreenSurface.cs:316–329`
- `PositionFontSize` property on `ScreenObject` (defaults to `(1,1)`) — confirmed at `ScreenObject.cs:24–72`
- `AbsoluteArea` formula and `WidthPixels`/`HeightPixels` via viewport — confirmed at `ScreenSurface.cs:159, 174–178`
- `ISurface` is a single-property `partial interface` — confirmed at `ISurface.cs`
- `IScreenSurfaceEditable = IScreenSurface + ISurface` — confirmed at `IScreenSurfaceEditable.cs`
- `ConnectedLineThin`, `ConnectedLineThick`, `Connected3dBox`, `ConnectedLineThinExtended` static arrays on `ICellSurface` — confirmed at `ICellSurface.Static.cs`
- `IFont` interface members documented are present (`Name`, `GlyphWidth`, `GlyphHeight`, `GlyphPadding`, `TotalGlyphs`, `SolidGlyphIndex`, `Image`, `GetGlyphSourceRectangle`, `GlyphRectangles`, `GlyphDefinitions`, `GetFontSize`) — confirmed at `IFont.cs`
- `SadFont` adds `Columns`, `Rows`, `FilePath` — confirmed at `SadFont.cs:42–61`
- `Console : ScreenSurface` adds `Cursor` as a `SadComponent` — confirmed at `Console.cs:61–62`
- `LayeredScreenSurface : ScreenSurface` adds `LayeredSurface` as a `SadComponent` and implements `ILayeredData` — confirmed at `LayeredScreenSurface.cs:13, 63`
- Partial file structure for `ICellSurface` (`.cs`, `.Static.cs`) and `ScreenSurface` (`.cs`, `.Input.cs`) — confirmed by file system

---

### ⚠️ Inaccurate or Misleading

**1. `IRenderer.OnHostUpdated` parameter type**
- **Doc says:** `void OnHostUpdated(IScreenSurface surface);`
- **Code says:** `public void OnHostUpdated(IScreenObject host)` — parameter is `IScreenObject`, not `IScreenSurface` (`IRenderer.cs:53`)
- **Severity:** Moderate — anyone implementing `IRenderer` will use the wrong signature

**2. `IRenderStep.Refresh` signature — two parameters missing**
- **Doc says:** `bool Refresh(IRenderer renderer, IScreenSurface screenObject);`
- **Code says:** `bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced);` (`IRenderStep.cs:39`)
- **Severity:** Moderate — signature is directly wrong; two required parameters are absent from the doc

**3. `IRenderer` interface is incomplete and has a nullable error**
- **Doc says:** `ITexture? Output { get; }` (nullable) and `List<IRenderStep> Steps { get; }` (get-only)
- **Code says:** `ITexture Output { get; }` (non-nullable, `IRenderer.cs:19`) and `List<IRenderStep> Steps { get; set; }` (get+set, `IRenderer.cs:34`)
- Doc also omits three members entirely: `string Name { get; set; }`, `byte Opacity { get; set; }`, `bool IsForced { get; set; }`
- **Severity:** Moderate — the documented interface stub is incomplete and has wrong nullability and accessor

**4. Resize + effects: `RemoveAll()` vs `DropInvalidCells()` omission**
- **Doc says:** "After resize, `Effects.DropInvalidCells()` prunes effect bindings referencing cells that no longer exist"
- **Code says:** When `clear = true`, `Effects.RemoveAll()` is called; `DropInvalidCells()` is only called when `clear = false`. (`CellSurface.cs:410–413`)
- **Severity:** Minor — the doc misrepresents the `clear=true` path, where all effects are removed entirely rather than pruned

**5. `IsDirtySet` event described as used by `EffectsManager`**
- **Doc says:** "`ColoredGlyphBase.IsDirty = true → IsDirtySet event (used by EffectsManager internally)`"
- **Code says:** `IsDirtySet` exists on `ColoredGlyphBase` (`ColoredGlyphBase.cs:16`) but **nothing in `EffectsManager.cs` subscribes to it**. The manager tracks cell changes via the return value of `ApplyToCell()`, not by observing `IsDirtySet`.
- **Severity:** Minor — the parenthetical is incorrect; will mislead anyone tracing the dirty chain

**6. `OnIsDirtyChanged()` described as "forwarding the signal up the display chain"**
- **Doc says:** "ScreenSurface subscribes to `Surface.IsDirtyChanged` and calls its own virtual `OnIsDirtyChanged()`, forwarding the signal up the display chain."
- **Code says:** `protected virtual void OnIsDirtyChanged() { }` — the base `ScreenSurface` implementation is an empty no-op (`ScreenSurface.cs:376`). No signal is forwarded unless a subclass overrides the method. The event subscription exists; calling "forward" is misleading.
- **Severity:** Minor — technically not wrong (subclasses can override), but the phrasing implies automatic propagation that doesn't exist by default

**7. `SetSurface(...)` described as a way to replace the `ICellSurface` reference on `ScreenSurface`**
- **Doc says:** "The surface can be replaced at runtime via `Surface { set; }` or `SetSurface(...)`."
- **Code says:** `SetSurface(...)` is from `ICellSurfaceSettable` (implemented by `CellSurface`) and remaps the cell array *within* a `CellSurface` — it does **not** replace the `ICellSurface` reference held by `ScreenSurface`. That replacement is done through the `Surface { set; }` property (from `ISurfaceSettable`). The two operations are distinct.
- **Severity:** Minor-Moderate — a reader will conflate a cell-array remap with an ICellSurface swap

**8. `LayeredScreenSurface` renderer selection described as a `DefaultRendererName` override**
- **Doc says:** "Subclasses override `DefaultRendererName` to request a different renderer (e.g., `LayeredScreenSurface` requests `"LayeredScreenSurface"`)"
- **Code says:** `LayeredScreenSurface` does **not** override `DefaultRendererName`. Instead, its constructor disposes the default renderer and directly assigns a new one: `Renderer = GameHost.Instance.GetRenderer(RendererNames.LayeredScreenSurface)` (`LayeredScreenSurface.cs:60–61`).
- **Severity:** Minor — the mechanism described is not what the code does

---

### ❌ Missing from Doc

- `ICellSurface.ConnectedLineEmpty` static array — exists at `ICellSurface.Static.cs:58–65` (doc lists the other four line arrays but not this one)
- `IRenderer` members `string Name`, `byte Opacity`, `bool IsForced` — present in `IRenderer.cs` but absent from the documented interface stub
- `IRenderStep` members `string Name`, `void SetData(object data)`, `void Reset()`, `void OnHostUpdated(IScreenObject host)` — present in `IRenderStep.cs` but absent from the documented interface stub
- `ICellSurface` base interfaces: it extends `IGridView<ColoredGlyphBase>` and `IEnumerable<ColoredGlyphBase>` (`ICellSurface.cs:10`) — not mentioned in the doc, relevant because it explains how the type interoperates with the SadRogue grid system
- Resize section does not mention the `clear=true` → `Effects.RemoveAll()` path (covered under ⚠️ #4 above but also a documentation gap)
- `EffectsManager` stores the original cell state in a `ColoredGlyphWithState` (with a `.State = cell.Clone()`) for possible restoration on effect removal (`EffectsManager.cs:392–409`). Doc says effects "do not persist the cell's original state to disk" — true, but the in-memory snapshot is worth noting for completeness.

---

### ❓ Unable to Verify

- "Refresh is guarded by `IsDirty || force`" — the actual guard logic is inside host renderer implementations (`SadConsole.Host.MonoGame/` etc.), not in the core interfaces or `ScreenSurface.cs`
- "Reset to `false` by the renderer after a successful `Refresh`" — same; requires checking host renderer implementations
- `UI.ControlsConsole` and `UI.WindowConsole` inheritance chain details — files not read during this review; the class existence and parent types were not checked

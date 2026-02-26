# Deckard — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### Architecture Overview
- `SadConsole/` — core library: object model, surfaces, entities, string parser, importers. Does NOT contain rendering code.
- `SadConsole.Host.*/` — all rendering lives here. Hosts implement interfaces defined in core.
- `SadConsole/Controls/` + `SadConsole/UI/` — large GUI system, separate from core surfaces.
- `templates/template_code/SadConsole.Examples.Demo.CSharp/` — massive demo code collection.
- `Tests/`, `PerformanceTests/` — test projects.

### Team
- Deckard (me) — Lead
- Roy — Core Dev (SadConsole main library)
- Pris — Controls Dev (SadConsole.Controls, themes, UI)
- Gaff — Host Dev (all rendering hosts)
- Rachael — Tester (tests, demo code)
- Scribe — Session logger
- Ralph — Work monitor

## Learnings

### 2026-02-25 — Correction Workflow Established

After verifying Holden's review and producing a separate verification file (`docs/architecture-surfaces-deckard-verification.md`), all corrections were applied directly to `docs/architecture-surfaces.md` and the verification file deleted.

**Going forward: verify → correct source doc directly. No intermediate verification files.**
The review file (`docs/architecture-surfaces-review.md`) may remain as a record of the review process, but any resulting corrections go straight into the source document.

**Task:** Independently verify Holden's findings about `docs/architecture-surfaces.md` against source code.

**Result: All of Holden's findings confirmed. Zero disputes.**

Key confirmed errors in the doc:

- `IRenderer.OnHostUpdated` — wrong parameter type (`IScreenSurface` vs actual `IScreenObject`). Confirmed at `IRenderer.cs:53`.
- `IRenderStep.Refresh` — two parameters missing from doc stub. Actual signature has 4 params. Confirmed at `IRenderStep.cs:39`.
- `IRenderer` stub — `Output` wrongly nullable, `Steps` wrongly get-only, three members absent (`Name`, `Opacity`, `IsForced`). Confirmed at `IRenderer.cs:14–34`.
- `IRenderStep` stub — four members absent: `Name`, `SetData`, `Reset`, `OnHostUpdated`. Confirmed at `IRenderStep.cs:13–59`.
- `LayeredScreenSurface` does NOT override `DefaultRendererName` — constructor directly assigns renderer via `GetRenderer(...)`. Confirmed at `LayeredScreenSurface.cs:60–61`.
- `SetSurface(...)` and `Surface { set; }` are distinct operations (cell-array rebind vs object reference swap). Confirmed via `ICellSurfaceSettable.cs` and `ISurfaceSettable.cs`.
- `Resize` effects: `RemoveAll()` called when `clear=true`, not `DropInvalidCells()`. Confirmed at `CellSurface.cs:409–413`.
- `IsDirtySet` parenthetical wrong — EffectsManager never subscribes to this event. Confirmed by grep with zero hits.
- `OnIsDirtyChanged()` is an empty no-op in base class — doc's "forwarding" description is misleading. Confirmed at `ScreenSurface.cs:376`.
- Missing: `ConnectedLineEmpty` static array, `ICellSurface` base interfaces (`IGridView<ColoredGlyphBase>`, `IEnumerable<ColoredGlyphBase>`), `EffectsManager` in-memory `ColoredGlyphWithState` clone.

**Pattern observed:** The `IRenderer` and `IRenderStep` interface stubs appear to have been written from an older or partial version of the interfaces. The core data model (CellSurface, ScreenSurface fields, viewport, indexers) was accurately documented; the contract stubs were not. Holden's review was high quality and thorough.

**Verification output:** `docs/architecture-surfaces-deckard-verification.md`  
**Decision file written:** `.squad/decisions/inbox/deckard-surfaces-doc-corrections.md`

### 2026 — Rendering Architecture Document

**Rendering pipeline facts confirmed:**
- Three-tier compositing: step private texture → renderer `_backingTexture` (== `IRenderer.Output`) → `Global.RenderOutput` → OS window.
- `ScreenSurface.Render(delta)` calls `renderer.Refresh(this)` then `renderer.Render(this)` per frame.
- `Refresh` = compositing phase (writes to `_backingTexture`); `Render` = draw-call enqueueing phase (writes to `GameHost.DrawCalls`).
- `IRenderStep` has three methods: `Refresh` (private texture), `Composing` (blit to renderer output), `Render` (enqueue to global queue).
- `Refresh` returns `bool` — `true` triggers the `Composing` pass for ALL steps; `false` skips it (caching).
- Steps sorted ascending by `SortOrder` (uint); `RenderStepComparer.Instance` is the shared `IComparer`.
- Default steps for `ScreenSurfaceRenderer`: `SurfaceRenderStep` (50), `OutputSurfaceRenderStep` (50), `TintSurfaceRenderStep` (90).
- Full sort order table: Window=10, Surface=50, Output=50, EntityManager=60, Cursor=70, ControlHost=80, Tint=90.
- `OptimizedScreenSurfaceRenderer` — no steps; draws directly to `_backingTexture` in `Refresh`. No extensibility.
- MonoGame adds `IRendererMonoGame` interface: `LocalSpriteBatch` (per-renderer), `MonoGameBlendState`, `CachedRenderRects`.
- SFML uses `RenderTexture` + `SharedSpriteBatch.Reset(target,...)` to target a texture; MonoGame uses `GraphicsDevice.SetRenderTarget(...)`.
- `GameHost._renderers` and `GameHost._rendererSteps` are `Dictionary<string, Type>` — host registers at startup, creates instances on demand via `Activator.CreateInstance`.

**Important files for rendering:**
- `SadConsole/Renderers/IRenderer.cs` — contract
- `SadConsole/Renderers/IRenderStep.cs` — step contract (3 phases)
- `SadConsole/Renderers/Constants.cs` — all string names + sort values
- `SadConsole/ScreenSurface.cs` (line ~335) — `Render()` override calling `Refresh`+`Render`
- `SadConsole.Host.SFML/Game.cs` — SFML game loop + final blit to window
- `SadConsole.Host.SFML/Renderers/ScreenSurfaceRenderer.cs` — canonical SFML renderer
- `SadConsole.Host.SFML/Renderers/Steps/` — all step implementations
- `SadConsole.Host.MonoGame/MonoGame/SadConsoleGameComponent.Mono.cs` — MonoGame draw phase
- `SadConsole.Host.MonoGame/Renderers/ScreenSurfaceRenderer.cs` — MonoGame renderer
- `SadConsole.Host.MonoGame/Renderers/IRendererMonoGame.cs` — MonoGame-specific renderer extras

**Document written:** `docs/architecture-rendering.md`

### 2026 — Architecture Document Pass

**Key files discovered:**
- `SadConsole/GameHost.cs` — abstract singleton; hosts subclass and register renderers, fonts, and steps here.
- `SadConsole/IScreenObject.cs` — root scene-graph contract; everything in the tree implements this.
- `SadConsole/IScreenSurface.cs` — extends IScreenObject with `ICellSurface`, `IRenderer`, `IFont`, dirty flag, tint, mouse events.
- `SadConsole/ICellSurface.cs` — flat `ColoredGlyph[]` data model with viewport support.
- `SadConsole/Renderers/IRenderer.cs` — `Refresh` (cache cells → texture) + `Render` (enqueue draw calls).
- `SadConsole/Renderers/IRenderStep.cs` — composable pipeline steps: `Refresh` → `Composing` → `Render` phases.
- `SadConsole/ITexture.cs` — host-provided pixel buffer abstraction.
- `SadConsole/IFont.cs` — host-provided sprite-sheet font abstraction.
- `SadConsole/UI/ControlHost.cs` — `IComponent` that manages all controls on a surface.
- `SadConsole/UI/ControlsConsole.cs` — convenience surface with ControlHost pre-attached.
- `SadConsole.Host.MonoGame/Game.Mono.cs` — MonoGame host entry point, subclasses `GameHost`.
- `SadConsole.Host.MonoGame/Renderers/ScreenSurfaceRenderer.cs` — concrete renderer using `RenderTarget2D` + `SpriteBatch`.

**Patterns confirmed:**
- Controls subsystem sits entirely above core via `IComponent` — `ControlHost` is just a component attached to `IScreenSurface`.
- Render pipeline is composable: `IRenderer` holds an ordered `List<IRenderStep>`. Steps are registered by name in the host and instantiated on demand.
- `SadConsole.Extended` is additive only — extra components, transitions, UI helpers; safe to ignore for core work.
- `SadRogue.Primitives` (external library) provides `Point`, `Rectangle`, `Color` across the whole stack.

**Architecture document written to:** `docs/architecture.md`

### 2026 — Game Loop Section

**Key files:**
- `SadConsole.Host.SFML/Game.cs` — the canonical game loop; `Initialize()` + `Run()` (explicit `while` loop). Best reference for understanding how a frame works.
- `SadConsole.Host.MonoGame/MonoGame/Game.Mono.cs` — MonoGame `Game` subclass; creates `SadConsoleGameComponent` in `Initialize()`.
- `SadConsole.Host.MonoGame/MonoGame/SadConsoleGameComponent.Mono.cs` — `DrawableGameComponent`; `Update(GameTime)` and `Draw(GameTime)` overrides implement the same update/draw phases as the SFML loop.

**Loop facts confirmed:**
- Update delta and draw delta are tracked with separate SFML clocks (SFML) / from `GameTime` (MonoGame) and may differ.
- `Settings.DoUpdate` and `Settings.DoDraw` gate each phase independently.
- Root entry points into the scene graph each frame: `Screen.Update(delta)` and `Screen.Render(delta)`.
- `RootComponent`s run before `Screen.Update` — global pre-scene logic.
- `GameHost.FrameUpdate` / `GameHost.FrameRender` events fire after the scene graph walk, giving user code a hook without subclassing.
- Draw calls are accumulated into `GameHost.DrawCalls` during `Screen.Render`, then flushed in a single `SharedSpriteBatch` pass to `Global.RenderOutput`, then blitted to the window (`DoFinalDraw`).
- MonoGame initialization is split: framework calls `SadConsoleGameComponent.Initialize()`, which calls into `SadConsole.Game.Initialize()` where renderer/step types are registered.

### 2026 — CellSurface and ScreenSurface Architecture Document

**Key findings:**

- `CellSurface` is a pure data object: flat `ColoredGlyphBase[]` array in row-major order (`index = y * Width + x`), a `BoundedRectangle _viewArea` tracking both buffer size and scrollable viewport, dirty flag, and `EffectsManager`.
- All editing/drawing API is **extension methods** on `ISurface` in the static class `CellSurfaceEditor` (`ICellSurface.Editor.cs`, 136 KB). This file is the largest in core — covers Print, Fill, Clear, Copy, Shift, DrawLine, DrawBox, DrawCircle, ConnectLines, SetEffect.
- `ICellSurface` is partial across three files: `ICellSurface.cs` (contract), `ICellSurface.Editor.cs` (extensions), `ICellSurface.Static.cs` (line-style arrays).
- `ISurface` is a single-property shim (`ICellSurface Surface { get; }`) that threads extension methods through composite types — both `CellSurface` and `ScreenSurface` satisfy it.
- `ScreenSurface` holds a **reference** (not ownership) to an `ICellSurface`. The surface can be swapped at runtime; the renderer is notified via `OnHostUpdated`.
- Inheritance chain: `ScreenObject` → `ScreenSurface` → `Console` / `LayeredScreenSurface` / `UI.ControlsConsole` → `UI.WindowConsole`.
- `ScreenSurface.Render` order: render components first (e.g., ControlHost draws controls), then `IRenderer.Refresh` + `IRenderer.Render`, then recurse into children.
- `UpdateAbsolutePosition` converts `Position` (cell units) to `AbsolutePosition` (pixels) using `FontSize * Position + Parent.AbsolutePosition`. `UsePixelPositioning = true` bypasses the font multiplication.
- `WidthPixels`/`HeightPixels` are based on `Surface.View` (viewport), not the full buffer — so scrollable surfaces present their viewport dimensions.
- `CellSurface.Resize` preserves existing cells into a new array; calls `Effects.DropInvalidCells()` then `OnCellsReset()` virtual hook.
- `QuietSurfaceHandling` on `ScreenSurface` suppresses `IsDirtyChanged` forwarding — useful when two surfaces share cell data.
- `SadFont.IsSadExtended` flag enables decorator overlay glyphs (named `GlyphDefinition` entries).
- `ForceRendererRefresh` is a one-shot flag to bypass the dirty check for one frame.

**Important files for surfaces:**
- `SadConsole/CellSurface.cs` — the core data type
- `SadConsole/ICellSurface.Editor.cs` — ALL editing/drawing operations (large file, search methods here)
- `SadConsole/ICellSurface.Static.cs` — static line-style arrays (ConnectedLineThin etc.)
- `SadConsole/ScreenSurface.cs` — displayable scene-graph node
- `SadConsole/ScreenSurface.Input.cs` — mouse input partial
- `SadConsole/ColoredGlyphBase.cs` — abstract per-cell data
- `SadConsole/ColoredGlyph.cs` — concrete per-cell type
- `SadConsole/ISurface.cs` — one-property shim enabling extension methods
- `SadConsole/IScreenSurfaceEditable.cs` — combines IScreenSurface + ISurface

**Document written:** `docs/architecture-surfaces.md`

### 2026 — Controls System Architecture Document

**Key findings:**

- Controls subsystem lives entirely in `SadConsole/UI/` and `SadConsole/UI/Controls/`. It uses `IComponent` to attach to any `IScreenSurface` — no special surface subclass required.
- `ControlBase` is the abstract root. Key contract: `UpdateAndRedraw(TimeSpan)` is abstract — each control self-paints into its own `ICellSurface`. The render step reads that buffer.
- Full hierarchy: `ControlBase` → `ButtonBase` → `Button`/`ButtonBox`; `ToggleButtonBase` → `CheckBox`/`RadioButton`; `TextBox` → `NumberBox`; `Label`, `ProgressBar`, `ScrollBar`, `ListBox`, `ComboBox`, `DrawingArea`, `SurfaceViewer`, `TabControl`, `TextEditor`, `ToggleSwitch`; `CompositeControl` → `Panel`/`TabItem`.
- `CompositeControl` implements `IContainer` — it's a control that hosts child controls (e.g., `TabControl` + `TabItem`, `ComboBox` internal layout). `Panel` is the general-purpose grouping widget.
- `ControlHost` is an `IComponent` that: holds the control list, routes keyboard to `FocusedControl`, routes mouse in reverse order (topmost first), manages tab order by `TabIndex`, manages mouse capture, injects `ControlHostRenderStep` (sort 80) on attach.
- `ControlsConsole` = `Console` + `ControlHost` pre-attached. `Window` adds title, border, drag, and modal support.
- Theme stack: `Colors` (palette + `Appearance_*` ColoredGlyphs) → `ThemeStates` (6-slot per-state appearances) → `ControlBase.ThemeState`. Resolution: control override → host `ThemeColors` → `Colors.Default`.
- `ControlStates` is a `[Flags]` enum (Normal=0, Disabled, Focused, Clicked, MouseOver, MouseLeftButtonDown, MouseRightButtonDown, Selected). Multiple flags can be active; `ThemeStates.GetStateAppearance` uses priority: Disabled > MouseDown > MouseOver > Focused > Selected > Normal.
- `AdjustableColor` maps a named `ColorNames` enum value to a `Color` with optional brightness offset; implicit cast to `Color`.
- `ControlHostRenderStep` owns GPU work: private `BackingTexture` → composited over renderer output in `Composing` phase. Iterates controls in forward draw order; recurses into nested `IContainer` controls. Respects `AlternateFont` per control.
- `IsMouseButtonStateClean` guard prevents button-down state if mouse entered with buttons already down.
- Tab wrapping supports cross-console tabbing via `CanTabToNextConsole` + `NextTabConsole`/`PreviousTabConsole`.
- `SadConsole.Extended` adds `Table`, `ColorPicker`, `ColorBar`, `HueBar`, `CharacterPicker`, `FileDirectoryListbox` — all follow the same `ControlBase` patterns.

**Important file paths for controls:**
- `SadConsole/UI/Controls/ControlBase.cs` — abstract root
- `SadConsole/UI/Controls/ControlState.cs` — `ControlStates` enum
- `SadConsole/UI/Controls/IContainer.cs` — container interface
- `SadConsole/UI/Controls/CompositeControl.cs` — controls-within-controls base
- `SadConsole/UI/Controls/Panel.cs` — generic grouping panel
- `SadConsole/UI/ControlHost.cs` — the IComponent manager
- `SadConsole/UI/ControlsConsole.cs` — convenience console
- `SadConsole/UI/WindowConsole.cs` — windowed console
- `SadConsole/UI/Colors.cs` — color palette
- `SadConsole/UI/ThemeStates.cs` — per-state appearance slots
- `SadConsole/UI/AdjustableColor.cs` — brightness-aware color
- `SadConsole.Host.MonoGame/Renderers/Steps/ControlHostRenderStep.cs` — GPU rendering (MonoGame)
- `SadConsole.Host.SFML/Renderers/Steps/ControlHostRenderStep.cs` — GPU rendering (SFML)

**Document written:** `docs/architecture-controls.md`

## Cross-Agent Updates

### 2026-02-25 — Holden: architecture-surfaces.md Requires Corrections

Holden reviewed `docs/architecture-surfaces.md` against source code and found factual errors in the `IRenderer` and `IRenderStep` interface stubs. These are wrong signatures — not paraphrase issues — and will mislead developers trying to implement or extend those types.

**Must fix before anyone uses the doc for implementation:**

1. **`IRenderer.OnHostUpdated` wrong parameter type** — doc says `IScreenSurface surface`; actual code (`IRenderer.cs:53`) uses `IScreenObject host`.
2. **`IRenderStep.Refresh` missing two parameters** — doc shows `(IRenderer renderer, IScreenSurface screenObject)`; actual signature (`IRenderStep.cs:39`) is `(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)`.
3. **`IRenderer` stub incomplete** — `Output` is non-nullable (`ITexture`, not `ITexture?`); `Steps` property is `{ get; set; }` not `{ get; }`; missing `string Name`, `byte Opacity`, `bool IsForced`.
4. **`IRenderStep` stub incomplete** — four members entirely absent: `Name`, `SetData`, `Reset`, `OnHostUpdated`.
5. **`LayeredScreenSurface` does NOT override `DefaultRendererName`** — code shows it disposes the default renderer and directly assigns a new one in its constructor; the described mechanism doesn't exist.
6. **`SetSurface(...)` vs `Surface { set; }` are conflated** — `SetSurface` (from `ICellSurfaceSettable`) remaps the cell array within a `CellSurface`; `Surface { set; }` (from `ISurfaceSettable`) replaces the `ICellSurface` reference on `ScreenSurface`. Different operations, not interchangeable.

**Secondary corrections (lower urgency):**
- Resize section: `Effects.DropInvalidCells()` is not always called — when `clear=true`, it's `Effects.RemoveAll()` (`CellSurface.cs:410–413`).
- `IsDirtySet` is not subscribed by anything in `EffectsManager.cs` — propagation only happens if a subclass overrides `OnIsDirtyChanged()`.
- `OnIsDirtyChanged()` base implementation is an empty no-op — doc implies it "forwards the signal up the display chain," which is misleading.
- `EffectsManager` keeps an in-memory clone of original cell state — not documented; relevant when reasoning about effect removal/restoration.
- `ICellSurface` base interfaces (`IGridView<ColoredGlyphBase>`, `IEnumerable<ColoredGlyphBase>`) not documented.
- `ConnectedLineEmpty` static array exists but is not listed in the doc.

**Action items (from Holden):**
- Roy or Pris: correct `IRenderer` and `IRenderStep` stubs — straightforward signature fixes.
- Deckard: clarify `SetSurface` vs `Surface { set; }` as a conceptual distinction; update `LayeredScreenSurface` renderer-selection description.

**Full review:** `docs/architecture-surfaces-review.md`

---
## Cross-Agent Note from Holden — surfaces review findings
**Date:** 2026-02-25

I reviewed `docs/architecture-surfaces.md` against the source. Here's what you need to know:

### Verified ✅
- `CellSurface` class declaration and full data model property table (`Cells`, `_viewArea`, `DefaultForeground/Background`, `DefaultGlyph`, `IsDirty`, `Effects`, `UsePrintProcessor`, `TimesShifted*`) — confirmed at `CellSurface.cs:15–159`
- Three indexers (`[x,y]`, `[index]`, `[Point]`) with `protected` setters; range span indexer has no setter — confirmed at `CellSurface.cs:167–195`
- `ColoredGlyphBase` property list (Foreground, Background, Glyph, Mirror, IsVisible, Decorators, IsDirty) — confirmed at `ColoredGlyphBase.cs:29–96`
- `IsDirty` guard + `OnIsDirtyChanged()` / `IsDirtyChanged` event chain on `CellSurface` — confirmed at `CellSurface.cs:61–70, 479–480`
- `SetIsDirtySafe()` sets backing field without firing event — confirmed at `CellSurface.cs:473–474`
- `ColoredGlyphBase.IsDirty = true` raises `IsDirtySet` event — confirmed at `ColoredGlyphBase.cs:100–101`
- All viewport properties (`Area`, `View`, `ViewPosition`, `ViewWidth`, `ViewHeight`, `IsScrollable`) — confirmed at `CellSurface.cs:94–155`, `ICellSurface.cs:50–96`
- All `CellSurfaceEditor` extension methods listed in the doc — confirmed at `ICellSurface.Editor.cs`
- `ICellSurfaceResize` two `Resize` overloads — confirmed at `ICellSurfaceResize.cs:17–26`
- `EffectsManager` dual-dictionary structure — confirmed at `EffectsManager.cs:20–25`
- `EffectsManager.UpdateEffects` sets `_backingSurface.IsDirty = true` on `ApplyToCell` returning true — confirmed at `EffectsManager.cs:287`
- `EffectsManager.DropInvalidCells()` prunes invalid cells — confirmed at `EffectsManager.cs:178–194`
- `ScreenSurface` class declaration, members (Surface, Renderer, Font, FontSize, Tint, UsePixelPositioning), mouse events, `QuietSurfaceHandling` — all confirmed
- `ScreenSurface.Render` order (components → renderer → children), `ScreenSurface.Update` calling `Effects.UpdateEffects` — confirmed at `ScreenSurface.cs:335–368`
- Five component lists on `ScreenObject` — confirmed at `ScreenObject.IComponentHost.cs:14–35`
- `UpdateAbsolutePosition` logic, `PositionFontSize`, `AbsoluteArea`, `WidthPixels`/`HeightPixels` via viewport — all confirmed
- `ISurface` single-property shim, `IScreenSurfaceEditable` combining `IScreenSurface + ISurface` — confirmed
- `ConnectedLineThin/Thick/3dBox/ThinExtended` static arrays — confirmed at `ICellSurface.Static.cs`
- `IFont` and `SadFont` member sets — confirmed at `IFont.cs`, `SadFont.cs:42–61`
- `Console` adds `Cursor` as a `SadComponent` — confirmed at `Console.cs:61–62`
- `LayeredScreenSurface` adds `LayeredSurface` as a `SadComponent` and implements `ILayeredData` — confirmed at `LayeredScreenSurface.cs:13, 63`

### Needs Correction ⚠️

**1. `IRenderer.OnHostUpdated` parameter type**
- **Doc says:** `void OnHostUpdated(IScreenSurface surface);`
- **Code says:** `void OnHostUpdated(IScreenObject host)` — parameter is `IScreenObject`, not `IScreenSurface` (`IRenderer.cs:53`)
- Anyone implementing `IRenderer` will use the wrong method signature.

**2. `IRenderStep.Refresh` signature is incomplete**
- **Doc says:** `bool Refresh(IRenderer renderer, IScreenSurface screenObject);`
- **Code says:** `bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced);` (`IRenderStep.cs:39`)
- Two required parameters are entirely absent from the documented stub.

**3. `IRenderer` interface stub has wrong nullability and missing members**
- **Doc says:** `ITexture? Output { get; }` (nullable) and `List<IRenderStep> Steps { get; }` (get-only)
- **Code says:** `ITexture Output { get; }` (non-nullable, `IRenderer.cs:19`) and `List<IRenderStep> Steps { get; set; }` (get+set, `IRenderer.cs:34`)
- Doc also omits three members entirely: `string Name { get; set; }`, `byte Opacity { get; set; }`, `bool IsForced { get; set; }`

**4. `Resize` + effects: `RemoveAll()` vs `DropInvalidCells()` conflated**
- **Doc says:** "After resize, `Effects.DropInvalidCells()` prunes effect bindings referencing cells that no longer exist"
- **Code says:** When `clear = true`, `Effects.RemoveAll()` is called; `DropInvalidCells()` is only called when `clear = false` (`CellSurface.cs:410–413`). Different semantics.

**5. `IsDirtySet` event parenthetical is wrong**
- **Doc says:** "`ColoredGlyphBase.IsDirty = true → IsDirtySet event (used by EffectsManager internally)`"
- **Code says:** `IsDirtySet` exists on `ColoredGlyphBase` (`ColoredGlyphBase.cs:16`) but nothing in `EffectsManager.cs` subscribes to it. The manager tracks cell changes via the return value of `ApplyToCell()`, not by observing `IsDirtySet`.

**6. `OnIsDirtyChanged()` described as "forwarding the signal up the display chain"**
- **Doc says:** "ScreenSurface subscribes to `Surface.IsDirtyChanged` and calls its own virtual `OnIsDirtyChanged()`, forwarding the signal up the display chain."
- **Code says:** `protected virtual void OnIsDirtyChanged() { }` is an empty no-op in the base `ScreenSurface` (`ScreenSurface.cs:376`). Nothing is forwarded unless a subclass overrides. The event subscription exists but "forwarding" overstates what the base class does.

**7. `SetSurface(...)` conflated with `ICellSurface` reference replacement**
- **Doc says:** "The surface can be replaced at runtime via `Surface { set; }` or `SetSurface(...)`."
- **Code says:** `SetSurface(...)` is from `ICellSurfaceSettable` and remaps the cell array *within* a `CellSurface` — it does **not** replace the `ICellSurface` reference held by `ScreenSurface`. That replacement is done through `Surface { set; }` from `ISurfaceSettable`. The two operations are entirely distinct.

**8. `LayeredScreenSurface` renderer selection uses the wrong mechanism**
- **Doc says:** "Subclasses override `DefaultRendererName` to request a different renderer (e.g., `LayeredScreenSurface` requests `"LayeredScreenSurface"`)"
- **Code says:** `LayeredScreenSurface` does **not** override `DefaultRendererName`. Its constructor disposes the default renderer and directly assigns a new one: `Renderer = GameHost.Instance.GetRenderer(RendererNames.LayeredScreenSurface)` (`LayeredScreenSurface.cs:60–61`).

### Missing from Doc ❌
- `ICellSurface.ConnectedLineEmpty` static array — present at `ICellSurface.Static.cs:58–65`; doc lists the other four line arrays but omits this one
- `IRenderer` members `string Name`, `byte Opacity`, `bool IsForced` — present in `IRenderer.cs` but absent from the documented interface stub
- `IRenderStep` members `string Name`, `void SetData(object data)`, `void Reset()`, `void OnHostUpdated(IScreenObject host)` — present in `IRenderStep.cs` but absent from the documented interface stub
- `ICellSurface` base interfaces: it extends `IGridView<ColoredGlyphBase>` and `IEnumerable<ColoredGlyphBase>` (`ICellSurface.cs:10`) — relevant because it explains SadRogue grid system interop
- `EffectsManager` stores a `ColoredGlyphWithState` snapshot (`.State = cell.Clone()`) for in-memory state restoration on effect removal (`EffectsManager.cs:392–409`) — doc says effects don't persist original state to disk (true) but omits that the in-memory snapshot exists

### Unable to Verify ❓
- "Refresh is guarded by `IsDirty || force`" — actual guard logic is inside host renderer implementations (`SadConsole.Host.MonoGame/` etc.), not in core interfaces or `ScreenSurface.cs`
- "Reset to `false` by the renderer after a successful `Refresh`" — same; requires checking host renderer implementations
- `UI.ControlsConsole` and `UI.WindowConsole` inheritance chain details — files not read during this review

**My recommendation:** The `IRenderer` and `IRenderStep` interface stubs are the highest-priority fixes — they contain wrong signatures that will directly break anyone trying to implement those interfaces. The `SetSurface` / `Surface { set; }` distinction (finding #7) is also worth correcting promptly since it describes two different operations as equivalent.
**Full review:** `docs/architecture-surfaces-review.md`

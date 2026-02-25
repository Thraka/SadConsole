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

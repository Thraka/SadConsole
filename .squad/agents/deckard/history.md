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

### 2026-02-24 — Initial Architecture Discovery (Summarized)

Completed comprehensive documentation of core architecture:

**Rendering pipeline:** Three-tier compositing (step private texture → renderer output → global queue → window). `ScreenSurface.Render` calls `renderer.Refresh()` (compositing) then `renderer.Render()` (draw enqueueing). `IRenderStep` composable pipeline with Refresh/Composing/Render phases. Steps sorted by `SortOrder`; defaults: Surface=50, Output=50, EntityManager=60, Cursor=70, ControlHost=80, Tint=90. MonoGame uses `LocalSpriteBatch` per renderer; SFML uses `RenderTexture` + `SharedSpriteBatch`.

**Data model:** `CellSurface` pure data object with flat `ColoredGlyph[]` in row-major order; `BoundedRectangle _viewArea` tracks viewport; dirty flag; `EffectsManager`. All editing via extension methods in `CellSurfaceEditor` (136KB file). `ScreenSurface` holds reference to `ICellSurface` (swappable at runtime). Inheritance: ScreenObject → ScreenSurface → Console/LayeredScreenSurface/ControlsConsole → WindowConsole. Position = cell units; converted to pixels via `FontSize * Position + Parent.AbsolutePosition`. `WidthPixels`/`HeightPixels` based on viewport.

**Controls:** Entirely separate subsystem via `IComponent` on any `IScreenSurface`. `ControlBase` abstract root; 20+ controls (Button, TextBox, Label, ProgressBar, ListBox, ComboBox, etc.). `CompositeControl` for nesting (Panel, TabControl). `ControlHost` manages list, focus routing, tab order, mouse capture; injects `ControlHostRenderStep` (sort 80). Theme stack: Colors palette → ThemeStates (6-slot per-state) → ControlBase.ThemeState. `ControlStates` [Flags] enum; priority: Disabled > MouseDown > MouseOver > Focused > Selected > Normal.

**Game loop:** SFML canonical reference (`SadConsole.Host.SFML/Game.cs`); MonoGame via `Game` subclass + `SadConsoleGameComponent`. Phases: `RootComponent` pre-scene logic → `Screen.Update(delta)` → event (`FrameUpdate`) → `Screen.Render(delta)` → event (`FrameRender`) → draw-call flush to `Global.RenderOutput` → final blit to window. `Settings.DoUpdate` / `Settings.DoDraw` gate each phase.

**Documents written:** `docs/architecture.md`, `docs/architecture-rendering.md`, `docs/architecture-surfaces.md`, `docs/architecture-controls.md`

## Learnings

### 2026-02-25 — Correction Workflow Established

## Cross-Agent Updates

### 2026-02-25 — Holden: architecture-surfaces.md Requires Corrections

Holden reviewed `docs/architecture-surfaces.md` against source code and found 12 factual errors:

**P1 (breaks implementation):** `IRenderer.OnHostUpdated` parameter type, `IRenderStep.Refresh` missing parameters, `IRenderer` nullability/accessors/missing members, `IRenderStep` missing 4 members.

**P2 (semantic errors):** `SetSurface(...)` vs `Surface { set; }` conflation, `LayeredScreenSurface` renderer selection mechanism wrong.

**P3 (misleading narrative):** `IsDirtySet` EffectsManager parenthetical, `OnIsDirtyChanged()` "forwarding" description.

**P4 (omissions):** `ConnectedLineEmpty` static array, `ICellSurface` base interfaces, `EffectsManager` in-memory state clone.

**Deckard action:** Independently verified all 12 findings against source — all confirmed, zero disputes. Applied corrections directly to `docs/architecture-surfaces.md` per user directive (no intermediate verification files). Decision record written to `.squad/decisions/inbox/deckard-surfaces-doc-corrections.md`.

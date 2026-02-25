# Holden — History

## Project Context

**Project:** SadConsole  
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI for rendering hosts  
**Lead developer:** Thraka  
**Description:** SadConsole is a C#-based .NET cross-platform terminal/tile-based game engine that simulates terminal/ASCII programs for modern platforms. The `SadConsole/` folder is the core library. Host libraries (`SadConsole.Host.MonoGame/`, `SadConsole.Host.SFML/`, etc.) provide all rendering. `SadConsole.Controls` is a large GUI system built on top of the core.

## My Role

I come in fresh. I read documentation and specs, then verify them against the actual source code. I flag anything that's wrong, outdated, missing, or misleading. I do not fix things — I find them and report them clearly so the right person can fix them.

## Learnings

### 2026-02-25 — Review of `docs/architecture-surfaces.md`

**Reviewed:** `docs/architecture-surfaces.md`  
**Output:** `docs/architecture-surfaces-review.md`, `.squad/decisions/inbox/holden-surfaces-review.md`

**Status:** NEEDS REVISION (8 inaccuracies found, 5 missing items)

**Key file paths verified:**
- `SadConsole/CellSurface.cs` — CellSurface data model, indexers, viewport, resize, SetIsDirtySafe
- `SadConsole/ICellSurface.cs` — partial interface, viewport props, IsDirtyChanged event
- `SadConsole/ICellSurface.Editor.cs` — CellSurfaceEditor static class (all extension methods on ISurface)
- `SadConsole/ICellSurface.Static.cs` — ConnectedLine arrays (includes ConnectedLineEmpty, not documented)
- `SadConsole/ICellSurfaceResize.cs` — Resize contract
- `SadConsole/ICellSurfaceSettable.cs` — SetSurface (remaps cell array, NOT ICellSurface reference swap)
- `SadConsole/ISurfaceSettable.cs` — Surface { get; set; } — this is the ICellSurface reference swap mechanism
- `SadConsole/ISurface.cs` — single-property partial interface
- `SadConsole/ColoredGlyphBase.cs` — IsDirtySet event fires on IsDirty=true; no EffectsManager subscription
- `SadConsole/ScreenSurface.cs` — OnIsDirtyChanged() is empty virtual { } in base class
- `SadConsole/ScreenSurface.Input.cs` — mouse events
- `SadConsole/ScreenObject.cs` — PositionFontSize field, UpdateAbsolutePosition
- `SadConsole/ScreenObject.IComponentHost.cs` — 5 component lists
- `SadConsole/Effects/EffectsManager.cs` — dual dictionaries; DropInvalidCells vs RemoveAll behavior
- `SadConsole/Renderers/IRenderer.cs` — OnHostUpdated takes IScreenObject not IScreenSurface; missing Name/Opacity/IsForced; Output non-nullable; Steps is get+set
- `SadConsole/Renderers/IRenderStep.cs` — Refresh has 4 params not 2; missing Name, SetData, Reset, OnHostUpdated
- `SadConsole/IFont.cs` — has IsSadExtended, UnsupportedGlyphIndex, GenerateGlyphSourceRectangle etc (doc only shows subset)
- `SadConsole/SadFont.cs` — Columns, Rows, FilePath confirmed
- `SadConsole/Console.cs` — Cursor added as SadComponent (not just property)
- `SadConsole/LayeredScreenSurface.cs` — does NOT override DefaultRendererName; directly replaces renderer in constructor

**Patterns observed:**
- Interface stubs in docs lag behind code — especially renderer interfaces. If a doc shows an abbreviated interface, assume it's missing members.
- `ICellSurfaceSettable.SetSurface` ≠ replacing ICellSurface on ScreenSurface. Keep these distinct.
- The 5-list component pattern (Update/Render/Keyboard/Mouse/Empty) is robust and correctly documented.
- EffectsManager tracks dirty state via `ApplyToCell()` return value, NOT by subscribing to ColoredGlyphBase.IsDirtySet.

### 2026-02-25 — Cross-agent brief for Deckard

**Output:** `.squad/decisions/inbox/holden-surfaces-findings-for-deckard.md`

Summarized all findings from `docs/architecture-surfaces-review.md` into a structured brief for Deckard. Covered 8 inaccuracies, 5 gaps, and a list of confirmed-correct items. Flagged action owners (Roy/Pris for interface stub corrections, Deckard for conceptual/architectural clarifications). Three items remain unverified pending host renderer knowledge.

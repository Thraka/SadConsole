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

## Architectural Knowledge Base (2026-02-24 to 2026-03-07)

**Rendering Pipeline (2026-02-24):** Three-tier compositing (step private texture → renderer output → global queue → window). `ScreenSurface.Render` calls `renderer.Refresh()` (compositing) then `renderer.Render()` (draw enqueueing). `IRenderStep` composable pipeline with Refresh/Composing/Render phases. Steps sorted by `SortOrder`; defaults: Surface=50, Output=50, EntityManager=60, Cursor=70, ControlHost=80, Tint=90. MonoGame uses `LocalSpriteBatch` per renderer; SFML uses `RenderTexture` + `SharedSpriteBatch`.

**Data Model (2026-02-24):** `CellSurface` pure data object with flat `ColoredGlyph[]` in row-major order; `BoundedRectangle _viewArea` tracks viewport; dirty flag; `EffectsManager`. All editing via extension methods in `CellSurfaceEditor` (136KB file). `ScreenSurface` holds reference to `ICellSurface` (swappable at runtime). Inheritance: ScreenObject → ScreenSurface → Console/LayeredScreenSurface/ControlsConsole → WindowConsole. Position = cell units; converted to pixels via `FontSize * Position + Parent.AbsolutePosition`. `WidthPixels`/`HeightPixels` based on viewport.

**Controls Subsystem (2026-02-24):** Entirely separate via `IComponent` on any `IScreenSurface`. `ControlBase` abstract root; 20+ controls (Button, TextBox, Label, ProgressBar, ListBox, ComboBox, etc.). `CompositeControl` for nesting. `ControlHost` manages list, focus routing, tab order, mouse capture; injects `ControlHostRenderStep` (sort 80). Theme stack: Colors palette → ThemeStates → ControlBase.ThemeState. `ControlStates` [Flags] enum; priority: Disabled > MouseDown > MouseOver > Focused > Selected > Normal.

**Game Loop (2026-02-24):** SFML canonical reference (`SadConsole.Host.SFML/Game.cs`); MonoGame via `Game` subclass + `SadConsoleGameComponent`. Phases: `RootComponent` pre-scene logic → `Screen.Update(delta)` → event (`FrameUpdate`) → `Screen.Render(delta)` → event (`FrameRender`) → draw-call flush to `Global.RenderOutput` → final blit to window. `Settings.DoUpdate` / `Settings.DoDraw` gate each phase.

**Documents written:** `docs/architecture.md`, `docs/architecture-rendering.md`, `docs/architecture-surfaces.md`, `docs/architecture-controls.md`, `docs/architecture-fonts.md`.

## Decision Analysis & Recommendations (2026-02-25 to 2026-03-07)

**Font Architecture (2026-02-26):** Document `docs/architecture-fonts.md` covering core/host boundary, serialization strategy (name-based), extended font system (GlyphDefinition + CellDecorator), SadConsole.Fonting excluded per directive.

**ANSI Parser Overhaul (2025-07-15):** Gap analysis identified 4 major defects: case-insensitive dispatch (H/h conflated), CSI-only (no DCS/OSC), `ValidAnsiCodes` string can't handle intermediate bytes/private prefixes, color system hard-coded to 8+8. Architecture decision: replace with ECMA-48 state machine (Parser → TypedCommands → Writer). 10-phase plan, 25 work items. Assigned Roy (impl), Rachael (tests).

**PendingWrap Clearing (2025-07-16):** Bug: blanket-clear after ALL CSI sequences caused line drift in ANSI art. Decision: invert to opt-in model — each cursor-moving handler clears PendingWrap; non-cursor handlers preserve by default. Pattern learned: dispatcher epilogues that blanket-clear are dangerous.

**ProcessTerminal Design (2025-07-16):** Process wrapper specification for PTY I/O + SadConsole rendering. Key decisions: ConPTY/POSIX PTY required (child processes disable ANSI on non-terminal), direct IComponent (needs Update + Keyboard), background reader thread with ConcurrentQueue, `IPseudoTerminal` platform abstraction, KeyboardEncoder static helper. 9 work items, 5 phases.

**Terminal Writer Architecture (2025-07-16):** Thraka's three questions analyzed. Decisions: (1) Writer.Cursor → injectable, nullable (enables data-stream mode without cursor); (2) Single Writer with optional ITerminalOutput response channel (1200+ lines ANSI identical for both modes); (3) TerminalConsole : Console (reuses Surface + Cursor + focus + keyboard, no duplication). Pattern: when two subsystems track shared concept (cursor position), designate one authoritative, make others read-only sync targets.

**Terminal Cursor Revision (2025-07-16):** Self-correction — Components.Cursor is text-editing cursor (1300 lines, 30+ members), terminal cursor only uses Position/IsVisible + Shape (6 DECSCUSR modes). New `TerminalCursor` data class (not IComponent) + `TerminalCursorRenderStep` per host. Pattern: when semantic gap is wide, create new type (don't reuse 2% of API surface).

**Surfaces Doc Corrections (2026-02-25):** Holden found 12 factual errors in `docs/architecture-surfaces.md`. Verified all 12, applied corrections directly per directive.

**RowFontSurface Coordination (2026-03-02):** All agents completed scope successfully. Roy's core implementation matches Gaff's host renderer expectations. Zero blockers, zero design conflicts.

**Roy/Deckard Alignment (2026-03-07):** Roy's parallel technical deep-dive achieved complete alignment across all three architecture questions. Zero disputes on cursor injection, single Writer pattern, or TerminalConsole design.

## Learnings

### State Synchronization Patterns

**One-way sync is clearer than bidirectional:** When two subsystems both track a shared concept (e.g., cursor position), designate exactly one as authoritative. Push model (authority → display) eliminates state divergence bugs. Avoid bidirectional mutation or "pull" models.

**API surface reuse threshold is low when semantic gap is wide:** Exposing 2 members from a 30-member public API is not "lightweight reuse" — it's exposing 28 misleading/dangerous members. Better to create a new, focused type.

**Dispatcher epilogues that blanket-clear state are dangerous:** Side effects should be declared by handlers, not assumed. Default should be the safe/no-op path (opt-in clearing beats opt-out).

**When components need multi-category capabilities, implement IComponent directly:** When a component needs both Update and Keyboard (not just one), directly implement IComponent. Don't try to inherit from single-capability base classes or create hacks. The interface is small.

## Learnings


## Addin System Architecture Design (2026-03-29)

**Requested by:** Thraka | **Status:** Approved — ready for Roy's implementation

Thraka requested an extensible addin system for the Editor. Designed complete architecture with entry point (`IEditorAddin` interface), discovery mechanism (`EditorAddinAttribute` + `AddinLoader`), menu item contribution (`AddinMenuItem` records in `Core.State.AddinMenuItems`, rendered by `GuiTopBar`), and deployment model (dedicated `addins/` subfolder).

### Key Design Decisions

1. **Entry point:** `IEditorAddin` interface lives in Editor.exe (Editor/Addins/IEditorAddin.cs) — contract for all addins
2. **Discovery:** `EditorAddinAttribute` assembly-level marker + `AddinLoader.LoadAndRegisterAddins()` invoked at program startup
3. **Menu items:** `AddinMenuItem` contribution records appended to `Core.State.AddinMenuItems` list; `GuiTopBar` renders them each frame
4. **Deployment:** Dedicated `addins/` subfolder next to Editor.exe executable
5. **References:** Addins reference Editor.exe directly (no SDK NuGet; Editor is a downloadable app, not a package)
6. **Trust model:** Fully trusted — addins have direct access to `Core.State` and `Core.ImGuiComponent`
7. **Sample:** Editor.Addin project becomes template/reference implementation

### Changes to Editor Codebase

- `Core.State.cs` — add `AddinMenuItems` list, make `DocumentBuilders` mutable post-init
- `GuiTopBar.cs` — render addin menu items from `Core.State.AddinMenuItems`
- `Program.cs` — call `AddinLoader.LoadAndRegisterAddins()` during startup sequence

### Pattern Note

Addins are fully trusted extensions with access to Editor's full internal state, enabling tightly integrated custom document types and tools. No need for sandboxing or capability-based contracts. Parallels: VS Code extensions, Sublime Text plugins.

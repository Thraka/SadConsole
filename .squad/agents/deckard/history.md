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

### 2025-07-15 — ANSI Parser Overhaul Plan

Produced comprehensive implementation plan for overhauling `SadConsole.Ansi` to support the CTerm/SyncTERM spec (`docs/cterm.adoc`). Plan written to `.squad/decisions/inbox/deckard-ansi-overhaul-plan.md`.

**Key findings from gap analysis:**
- Current parser is fundamentally broken: case-insensitive dispatch means `'H'` (CUP) and `'h'` (DECSET) are conflated, `'M'` (DL) and `'m'` (SGR) are conflated. This is the #1 fix.
- Parser only recognizes `ESC [` (CSI) — no Fp/Fe/Fs escape sequences, no DCS, no OSC strings.
- `ValidAnsiCodes` string approach cannot distinguish intermediate bytes (space before final char) or private prefixes (`?`, `=`).
- Color system is hard-coded to 8 ANSI colors + 8 bright — no 256-color palette, no true color, no palette redefinition.
- State tracking is minimal (bold, reverse, concealed, fg, bg) — needs scroll margins, tab stops, origin mode, auto-wrap mode, DEC mode flags, and full saved cursor state.

**Architecture decision:** Replace inline parsing with a proper ECMA-48 state machine (`AnsiParser`) that emits typed commands, consumed by the refactored `AnsiWriter`. This cleanly separates parsing from execution, makes the parser unit-testable, and scales to the full spec.

**Plan:** 10 phases, 25 work items. Phase 0 (state machine parser) is the load-bearing XL item — everything depends on it. Phases 3-7 are parallelizable. Phase 8 (scroll margins + DEC modes) is highest-risk due to behavioral tentacles into all other operations.

**Assigned:** Roy for all core implementation, Rachael for all test suites. Pris and Gaff not needed — ANSI parsing is entirely within core, no host or controls impact.

### 2026-02-26 — Font System Architecture Document

Wrote `docs/architecture-fonts.md` — comprehensive font system architecture reference covering all 10 sections requested. Synthesized Roy's core analysis (IFont, SadFont, glyph system, serialization) and Gaff's host analysis (MonoGame/SFML/FNA/KNI texture loading, rendering pipeline) into a single unified document.

Key architectural insights documented:
- **Font/FontSize independence** is the core scaling design: `Font` is the atlas, `FontSize` is the display scale, they are decoupled on `ScreenSurface`. Multiple surfaces share one font at different scales.
- **Core/host boundary** is clean: core owns `IFont`/`SadFont` (metadata + glyph rectangles), hosts own `ITexture`/`GameTexture` (GPU textures) and `SurfaceRenderStep` (per-cell glyph blitting).
- **Serialization strategy** is name-based: surface saves store only font name, resolved from `GameHost.Fonts` at load time. Font files themselves use `TypeNameHandling.All` for polymorphic JSON.
- **Extended font system** (`IsSadExtended`, `GlyphDefinition`, `CellDecorator`) enables overlay glyphs without extra surface layers — decorators are rendered on top of base glyphs in the same render pass.
- **SadConsole.Fonting excluded** per directive — it's an unfinished experiment not part of the documented architecture.

### 2026-02-25 — Correction Workflow Established

## Cross-Agent Updates

### 2026-02-25 — Holden: architecture-surfaces.md Requires Corrections

Holden reviewed `docs/architecture-surfaces.md` against source code and found 12 factual errors:

**P1 (breaks implementation):** `IRenderer.OnHostUpdated` parameter type, `IRenderStep.Refresh` missing parameters, `IRenderer` nullability/accessors/missing members, `IRenderStep` missing 4 members.

**P2 (semantic errors):** `SetSurface(...)` vs `Surface { set; }` conflation, `LayeredScreenSurface` renderer selection mechanism wrong.

**P3 (misleading narrative):** `IsDirtySet` EffectsManager parenthetical, `OnIsDirtyChanged()` "forwarding" description.

**P4 (omissions):** `ConnectedLineEmpty` static array, `ICellSurface` base interfaces, `EffectsManager` in-memory state clone.

**Deckard action:** Independently verified all 12 findings against source — all confirmed, zero disputes. Applied corrections directly to `docs/architecture-surfaces.md` per user directive (no intermediate verification files). Decision record written to `.squad/decisions/inbox/deckard-surfaces-doc-corrections.md`.

## Cross-Agent Updates

### 2026-03-02 — RowFontSurface Implementation Coordination

**Status:** All agents completed their scope successfully. Zero blockers.

**Roy's Coordination with Gaff:**
- Core `RowFontSurface` class implements `GetRowFont(int row)`, `GetRowFontSize(int row)`, `GetRowYOffset(int row)`, `GetRowHeight(int row)` methods
- Host renderers reference these core methods in their render loops
- Roy's `DefaultRendererName` override returns `Constants.RendererNames.RowFontSurface` to signal host to use specialized renderer
- Gaff verified core methods match host renderer expectations

**Gaff's Coordination with Roy:**
- Implemented renderers in MonoGame, SFML, FNA following Deckard's architecture specification
- All hosts build successfully; no blocking issues with Roy's core implementation
- Host render steps correctly call core methods and handle variable row heights
- Registered in each host's `Game.cs` file via `SetRenderer()` and `SetRendererStep()`

**Deckard's Oversight:**
- Provided complete specification with clear separation of core vs. host responsibilities
- All implementation followed spec without deviations
- Zero design conflicts between core and hosts

**Next Phase:**
- Rachael (Tester) — integration testing per testing strategy
- Monitor performance if many unique fonts per surface
- Future optimization: batch rows by font if texture switching overhead detected

### 2025-07-16 — PendingWrap Clearing Model Analysis

**Bug:** `Writer.OnCsiDispatch` (line 342) blanket-clears `State.PendingWrap = false` after ALL CSI sequences, including SGR. Per ECMA-48, only cursor-moving sequences should clear pending-wrap. This caused line drift in ANSI art files that set colors at column-79 boundaries.

**Root cause:** Original implementation used "opt-out" model — clear by default, exceptions must be carved out. The clearing was placed in the dispatcher epilogue, the wrong abstraction level.

**Decision:** Invert to "opt-in" model. Remove blanket clears from lines 342 and 224. Each cursor-moving handler clears PendingWrap itself. Non-cursor-moving handlers (SGR, erase, scroll, DSR, tab management) preserve it by default. Decision record: `.squad/decisions/inbox/deckard-pendingwrap-clearing-model.md`.

**Key files:** `SadConsole/Terminal/Writer.cs` (lines 219-344 OnCsiDispatch), `SadConsole/Terminal/State.cs` (PendingWrap property).

**Pattern learned:** Dispatcher epilogues that blanket-clear state are dangerous. Side effects should be declared by handlers, not assumed by the dispatcher. This is analogous to database auto-commit: the default should be the safe/no-op path.

### 2025-07-16 — ProcessTerminal Design Document

Produced comprehensive design specification for a process wrapper that launches OS processes (PowerShell, bash, etc.) with pseudo-terminal I/O and renders output onto SadConsole surfaces. Written to `.squad/decisions/inbox/deckard-process-wrapper.md`.

**Key architectural decisions:**
- **ConPTY / POSIX PTY required** — plain `Process.RedirectStdout` is insufficient because child processes disable ANSI output when they detect non-terminal I/O. ConPTY (Windows 10 1809+) and POSIX PTY (Unix/macOS) are the correct abstraction.
- **Direct IComponent, not InstructionBase** — `ProcessTerminal` needs both `IsUpdate` (drain output buffer) and `IsKeyboard` (capture keystrokes). No existing base class supports this combination; `UpdateComponent`'s `IsKeyboard => false` is an explicit interface impl that can't be overridden.
- **Does NOT compose with Reader** — Reader is designed for finite BPS-throttled streams. Live process output should render immediately. Self-contained buffer drain via `ConcurrentQueue<byte[]>` on the game thread.
- **Background reader thread** — PTY output is blocking I/O. Background thread reads and queues; `Update()` drains on game thread. Only `ConcurrentQueue` needs thread safety; `Writer.Feed()` is game-thread-only.
- **`KeyboardEncoder` as static helper** — SadConsole `Keys` → ANSI escape sequences (arrow keys, F-keys, Ctrl+key, application cursor mode) is non-trivial, reusable, and independently testable.
- **Platform abstraction via `IPseudoTerminal`** — WindowsConPty, UnixPty, and ProcessRedirectPty (fallback) behind a single interface. Factory auto-detects platform. First P/Invoke in core — justified because PTY is inherently OS-level.
- **Namespace:** `SadConsole.Terminal` with PTY impls in `Pty/` subfolder. Cohesive with existing Parser/Writer/Reader/State.

**Work items:** 9 items across 5 phases for Roy, testing strategy for Rachael. Dependency-ordered. Start with ProcessRedirectPty (simplest) to get end-to-end flow working, then layer in ConPTY/UnixPty.

**Open questions for Thraka:** (1) All three PTY backends or Windows+fallback first? (2) Core vs separate NuGet for P/Invoke code? (3) Auto-detect resize vs explicit `Resize()` calls?

**Pattern learned:** When a component needs capabilities from two different IComponent categories (Update + Keyboard), implement IComponent directly. Don't try to inherit from the single-capability base classes or create multi-inheritance hacks. The interface is small enough that direct implementation is cleaner than fighting the type hierarchy.

### 2025-07-16 — Terminal Writer Architecture Analysis

Thraka asked three architecture questions about Writer/Cursor/Console relationships. Full analysis in `.squad/decisions/inbox/deckard-terminal-writer-architecture.md`.

**Key decisions:**
1. **Writer.Cursor → injectable, nullable.** Writer creates its own Cursor internally (bypasses IComponent lifecycle, no render step). State.CursorRow/CursorColumn is the real authority; SyncCursorPosition() is a one-way push to Cursor.Position. Making Cursor nullable means data-stream use (ANSI art) doesn't need one, interactive use injects Console's Cursor.
2. **Do NOT split Writer into two classes.** 1200+ lines of ANSI sequence handling are identical for both data-stream and interactive use. The only difference is a response channel (DA, DSR) — solved by adding `ITerminalOutput?` property. Null = silent (current behavior), set = responses forwarded.
3. **TerminalConsole : Console** is the right approach (not composition). Console already provides Surface + Cursor + focus + keyboard. TerminalConsole wires Writer to Console's Surface + Cursor, overrides keyboard for ANSI encoding. Follows existing pattern (ControlsConsole, Window both extend Console).

**Dual cursor state resolution:** State is always authoritative during ANSI processing. Data flow: ANSI input → Parser → State mutation → SyncCursorPosition() → Cursor.Position. User input in TerminalConsole goes through the ANSI protocol (keypresses encoded as escape sequences), never directly to Cursor.Position.

**Key file relationships:**
- `SadConsole/Terminal/Writer.cs` — ECMA-48 renderer, owns State/Parser/Palette, sync target is Cursor
- `SadConsole/Components/Cursor.cs` — full IComponent with print pipeline, keyboard, mouse; `_editor` set via OnAdded lifecycle
- `SadConsole/Console.cs` — ScreenSurface + Cursor (added as SadComponent, gets full lifecycle)
- `SadConsole/Terminal/State.cs` — pure data: CursorRow/CursorColumn, SGR, modes, scroll region
- `SadConsole/Terminal/Reader.cs` — InstructionBase, finite stream → Writer.Feed(), BPS throttling

**Pattern learned:** When two subsystems both track a shared concept (cursor position), designate exactly one as authoritative and make all others read-only sync targets. Don't allow bidirectional mutation — it creates state divergence bugs. The "push" model (authority → display) is clearer than "pull" or "two-way binding".

## Cross-Agent Updates

### 2026-03-07 — Roy's Technical Deep-Dive: Cursor Architecture Analysis

Roy completed parallel technical analysis of the same three architecture questions. Results merged to decisions.md.

**Roy's Findings:**
- **State vs Components.Cursor Duality:** Documented one-way sync contract and write-only Cursor usage from Writer perspective.
- **External Cursor Risk Assessment:** Confirmed LOW-RISK — Surface is already external; Cursor is the only internal dependency. Requires null checks in SyncCursorPosition() and DECTCEM handler (2 places only).
- **Reader Role:** Clarified as pure transport layer (throttles bytes to Writer). Decoupled from terminal logic.
- **Two Writer Modes:** Technical justification for single Writer with optional Cursor vs splitting into two classes. 1200+ lines of parsing are identical; differences are configuration toggles (Cursor, output channel, keyboard input).
- **TerminalConsole Pattern:** Confirmed subclass Console is minimal-duplication approach. Inherits Surface, Cursor, focus, keyboard, rendering.
- **Phased Implementation:** Proposed Phase 1 (external Cursor, low-risk), Phase 2 (TerminalConsole, medium effort), Phase 3 (optional split if complexity grows).

**Alignment:** Zero disputes between Deckard and Roy — complete technical alignment across all three recommendations.

**Next:** Awaiting Thraka design decisions before Phase 1 implementation (cursor injection).

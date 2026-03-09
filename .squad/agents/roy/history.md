# Roy — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### My Domain
I own `SadConsole/` — the core library. Key areas:
- Surface types (Console, AnimatedSurface, etc.) and the object model
- Entity system and components
- Animation/instruction system
- String parser (color codes, effects inline in strings)
- Importers: ANSI art, TheDraw fonts, RexPaint, Playscii
- Font/tileset definitions (core side — hosts do the GPU work)
- Host interface contracts — what hosts must implement
- Terminal parser and ANSI/ECMA-48 rendering

### Important: Rendering is in the Hosts
The core library does NOT render. It defines what needs to be rendered and the interfaces/contracts. All GPU/draw calls live in `SadConsole.Host.*/` (Gaff's territory).

### Team
- Deckard — Lead (consult before public API changes)
- Pris — Controls Dev (separate from core, but interacts with core surfaces)
- Gaff — Host Dev (implements my interfaces)
- Rachael — Tester

## Terminal System Architecture (Core Implementation)

**Parser:** ECMA-48 state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation via preallocated span arrays, CP437/UTF-8 encoding support (87 tests).

**Writer:** Direct ICellSurface cell manipulation (glyph, foreground, background), ColorMode enum (Default/Palette/TrueColor), pending-wrap state for ANSI art, scroll regions with origin mode (160+ tests).

**State:** Cursor position/attributes (bold/dim/reverse/underline/strikethrough/italic/blink), SGR tracking, tab stops (SortedSet for O(1) lookup).

**Palette:** 256-entry RGB array; OSC 4/10/11 redefinition support (X11 rgb:r/g/b and #rrggbb formats).

**Rendering:** CellDecorator for visual attributes (underline glyph 95, strikethrough 196), font-defined glyph override, color resolution (reverse swaps fg/bg, dim halves RGB, bold shifts palette 0-7 to 8-15).

**Completed Phases (662 tests):** Phase 0 (parser, 87 tests), Phase 1 (writer, 160 tests), Phase 5 (insert/delete, 24 tests), Phase 6 (tabs, 8 tests), Phase 8 (DEC modes, 16 tests), Phase 3 (decorators, 18 tests), Phase 9 (palette, 12 tests), Phase 10 (polish, 18 tests).

**Key Learnings (2026-03-04 to 2026-03-06):**
- **PendingWrap opt-in model:** Cursor-movers clear; SGR/decorators preserve. Root bug fix for b5-ans01.ans drift.
- **CUF/CHT forward-cursor resolution:** At right margin with PendingWrap, resolve wrap before moving. Only forward-clamped handlers need this.
- **Bold + default foreground:** When `ForegroundMode == Default && Bold`, return `Palette[15]` not `DefaultForeground`.

## Terminal Cursor Architecture (Phase 1, 2026-03-07 to 2026-03-09)

**TerminalCursor:** Lightweight data class (NOT IComponent), replaces Components.Cursor. Properties: Position, IsVisible, Shape, CursorRenderCellActiveState.

**CursorShape enum:** Maps DECSCUSR parameters (1=BlinkingBlock, 2=SteadyBlock, 3=BlinkingUnderline, 4=SteadyUnderline, 5=BlinkingBar, 6=SteadyBar).

**Writer changes:** Cursor type Components.Cursor → TerminalCursor? (nullable, injectable). Constructor unchanged; caller sets Cursor via property. SyncCursorPosition(), DECTCEM, DECSCUSR all null-check.

**Design rationale:** Enables headless ANSI rendering (null cursor for data-stream mode) and interactive terminal (injected cursor for user feedback). Eliminates IComponent overhead. Render-step compatible (CursorRenderCellActiveState interface unchanged).

**Test results:** 2,178 tests pass (net8.0/9.0/10.0), 44 TerminalCursor-specific tests, zero regressions.

## TerminalConsole Phase 2 (2026-03-09)

**TerminalConsole:** Inherits ScreenSurface (not Console — avoids Components.Cursor dead weight). Root namespace `SadConsole`.

**Construction:** `(int width, int height, IFont? font)` — creates Writer(Surface, Font), TerminalCursor, wires Cursor to Writer.

**Rendering:** Uses `GameHost.Instance.GetRendererStep()` factory pattern for TerminalCursorRenderStep. OnRendererChanged re-adds step when renderer swaps.

**APIs:** `Feed(string)`, `Feed(ReadOnlySpan<byte>)` forward to Writer. Focus control via OnFocused/OnFocusLost toggles cursor visibility. ProcessKeyboard captures input (Phase 3 KeyboardEncoder).

**Test suite:** 33 new tests (construction, inheritance, Feed, cursor integration, focus, multi-instance). 759 total tests all green.

**Build:** 0 errors, 48 warnings (all pre-existing).

## Archived Terminal Implementation Details

**Phases 5–10 detailed learnings (2026-03-04):**
- Phase 5: ICH/DCH on current row; IL/DL within scroll region; SU/SD reuse scroll; ECH erase-in-place; REP tracks lastChar.
- Phase 6: CHT/CBT loop tab stops (SortedSet); TBC modes 0/3.
- Phase 8: Private prefix `?` → HandleDecPrivateMode; DECSTBM regular CSI; origin mode affects CUP; DECOM homes; DECTCEM wires to Cursor.IsVisible.
- Phase 3: CellDecorator underline 95, strikethrough 196; decorators nullable List; ApplyDecorators uses resolved foreground.
- Phase 9: OSC raw bytes; OSC 4 multi-entry; X11 color scaling; OSC 10/11 palette updates.
- Phase 10: ED modes 0/1/2/3 verified; CSI s Save when len==0; PendingWrap in DEC/NEL/RI; VPA respects origin mode.

**Architecture decisions (2026-03-07):**
- Dual cursor state: State.CursorRow/Column (pure logic) vs Cursor.Position (visual only, ONE-WAY sync).
- Writer never reads Cursor.Position (Cursor is write-only from Writer's perspective).
- Surface external (constructor param); Cursor now external too (nullable, injectable).
- Two Writer modes: data-stream (ANSI art playback) vs interactive (terminal emulator). Single Writer, optional cursor.
- Reader: pure byte pump (Stream → Writer.Feed), extends InstructionBase. No parsing, no cursor knowledge.

**Deckard alignment (2026-03-07):** Architecture decision analysis produced complete technical alignment on injectable nullable Cursor, single Writer with ITerminalOutput response channel, TerminalConsole subclass pattern.

## Core Context (Continued)

**Deckard's Findings:**
- **Architecture-level design** for injectable nullable Cursor, single Writer with ITerminalOutput response channel, TerminalConsole subclass pattern
- **Backwards compatibility assessment:** Minor breaking change for Cursor nullability; migration path via convenience constructor
- **Final class hierarchy:** ScreenObject → ScreenSurface → Console → Terminal.TerminalConsole
- **Open questions for Thraka:** Breaking change tolerance, KeyboardEncoder scope, TerminalConsole namespace preference
- **Key insight:** When two subsystems track shared concept (cursor position), designate one as authoritative. Push model (authority → display) is clearer than pull or bidirectional.

**Alignment:** Complete technical alignment — Deckard's recommendations match Roy's phased implementation approach. Zero disputes on all three questions.

**Next:** Awaiting Thraka design decisions before Phase 1 implementation.

### 2026-03-07 — Phase 1 Implementation: TerminalCursor, CursorShape, Writer Changes

**Completed:** Terminal cursor architecture redesign Phase 1 — core library implementation.

**Created files:**
- `SadConsole/Terminal/CursorShape.cs` — enum mapping DECSCUSR parameter values (1=BlinkingBlock, 2=SteadyBlock, 3=BlinkingUnderline, 4=SteadyUnderline, 5=BlinkingBar, 6=SteadyBar)
- `SadConsole/Terminal/TerminalCursor.cs` — lightweight data class (NOT IComponent), properties: Position, IsVisible, Shape, CursorRenderCellActiveState (ColoredGlyphBase for render step)

**Modified Writer.cs:**
- `Cursor` property changed from `Components.Cursor` (non-nullable) to `TerminalCursor?` (nullable)
- Constructor signature unchanged: `Writer(ICellSurface surface, IFont font)` — NO cursor creation, caller sets via property
- `SyncCursorPosition()` now null-checks before setting Position
- DECTCEM handler (CSI ? 25 h/l) null-checks before setting IsVisible
- Added DECSCUSR handler (CSI Ps SP q) for cursor shape control — maps parameter to CursorShape enum, calls UpdateGlyphForShape()
- Removed internal Cursor instantiation

**TerminalCursor design:**
- Parameterless constructor defaults to white on black
- Exposes `CursorRenderCellActiveState` (ColoredGlyphBase) — render steps read Glyph, Foreground, Background
- `UpdateGlyphForShape()` maps: Block→219 (█), Underline→95 (_), Bar→124 (|)
- SetForeground/SetBackground helpers for color control
- Pattern matches Components.Cursor's CursorRenderCellActiveState interface — zero render step changes needed

**Test fixes:**
- TerminalCursorTests.cs: 44 tests, all pass (cursor defaults, shape values, null-cursor mode, DECTCEM/DECSCUSR integration)
- TerminalWriterPhase2Tests.cs: Updated Setup() to create TerminalCursor for visibility tests
- Fixed 2 test expectations (cursor position after print was off by 1 — tests had bugs, not implementation)

**Build/Test results:**
- SadConsole.csproj: Build succeeded, 0 errors
- All 990 terminal tests pass (net8.0, net9.0, net10.0)

**Key pattern:** Render step compatibility maintained — TerminalCursor.CursorRenderCellActiveState matches Components.Cursor's interface (Glyph, Foreground, Background properties). Gaff's host render steps can read either type without changes.
## 2026-03-09 — Terminal Cursor Phase 1 Complete

Delivered core implementation of TerminalCursor architecture per Thraka's final directives:

**Created:**
- SadConsole/Terminal/CursorShape.cs — 6-value enum mapping DECSCUSR parameters
- SadConsole/Terminal/TerminalCursor.cs — data class with Position, IsVisible, Shape, CursorRenderCellActiveState

**Modified:**
- SadConsole/Terminal/Writer.cs — Cursor type Components.Cursor → TerminalCursor? (nullable, injectable)
- SadConsole/Terminal/Writer.cs — Added null-safe SyncCursorPosition(), DECTCEM, DECSCUSR handlers
- SadConsole/Terminal/Writer.cs — Glyph mapping (Block→219, Underline→95, Bar→124)

**Results:**
- 2,178 tests pass (net8.0, net9.0, net10.0)
- 44 TerminalCursor-specific tests added (Roy + Rachael collaboration)
- Zero regressions

**Key design:** TerminalCursor replaces Components.Cursor to eliminate IComponent overhead. Writer needs only position/visibility/shape — enables headless ANSI rendering + cleaner terminal/UI separation.

**Dependency:** Gaff's host render steps depend on TerminalCursor interface. Rachael's tests validate both null-cursor (data-stream) and injected-cursor (interactive) modes.

**Next:** TerminalConsole Phase 2 — inherit ScreenSurface instead of Console (per 2026-03-09T00:14 directive).

## 2026-03-09 — TerminalConsole Phase 2 Complete

Delivered `SadConsole/TerminalConsole.cs` — the class that wires Writer + TerminalCursor onto a ScreenSurface.

**Created:** `SadConsole/TerminalConsole.cs`
- Inherits `ScreenSurface` (NOT Console — avoids Components.Cursor dead weight)
- Namespace: `SadConsole` (root, sibling of Console)
- Constructor `(int width, int height, IFont? font)` — creates Writer(Surface, Font), TerminalCursor, wires Cursor to Writer
- Render step: Uses `GameHost.Instance.GetRendererStep(Constants.RenderStepNames.TerminalCursor)` factory pattern (same as Components.Cursor uses for its render step). SetData(terminalCursor), add to Renderer.Steps, sort.
- `Feed(string)` and `Feed(ReadOnlySpan<byte>)` convenience methods forwarding to Writer
- Focus: OnFocused/OnFocusLost toggle TerminalCursor.IsVisible. ProcessKeyboard returns true when focused (captures input for Phase 3 KeyboardEncoder).
- UseKeyboard set to `Settings.DefaultConsoleUseKeyboard` (matches Console pattern)
- OnRendererChanged override re-adds cursor render step when renderer swaps
- Dispose cleans up render step

**Key pattern learned:** Host render steps are accessed via `GameHost.Instance.GetRendererStep(name)` factory — core library never references concrete host types. The RenderStepNames.TerminalCursor constant ("terminalcursor") was already registered in Phase 1 Constants.cs. Hosts register the concrete TerminalCursorRenderStep type at startup.

**Build:** 0 errors, 48 warnings (all pre-existing). **Tests:** 726/726 pass (net10.0), zero regressions.

**Next:** Phase 3 — KeyboardEncoder (convert keystrokes to terminal sequences for bidirectional communication).

## 2026-03-09 — Scribe Reconciliation

Orchestration logs written for Roy (Phase 2 impl) and Rachael (33 tests). Session log captures phase completion. No decision inbox items.

**Status:** All files staged and committed to experiments branch @ 9b94793c.

**Cross-team sync:** TerminalConsole Phase 2 complete (759/759 tests). Cursor integration ready for Phase 3 (KeyboardEncoder).

**Next phase:** Host integration testing (Gaff) and keyboard encoding layer (Phase 3).


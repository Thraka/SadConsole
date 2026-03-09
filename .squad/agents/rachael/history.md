# Rachael — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### My Domain
- `Tests/` — unit and integration tests
- `PerformanceTests/` — performance benchmarks
- `templates/template_code/SadConsole.Examples.Demo.CSharp/` — massive demo/sample code collection that people use to learn SadConsole
- `Samples/` — additional samples

### Important Notes
- The demo code in `templates/` is what users reference to learn the API. It must always compile and reflect current best practices.
- When API changes happen (Roy, Pris, or Gaff), I check if demo code needs updating.
- Tests span multiple solution files — check which .sln targets what before running.

### Team
- Roy — Core Dev (my tests validate his work)
- Pris — Controls Dev (I test UI components)
- Gaff — Host Dev (host-level integration is harder to test in isolation)
- Deckard — I flag coverage gaps to Lead

## Archived Work — Terminal Test Suites & PendingWrap Architecture (2026-03-04 to 2026-03-06)

**Archived milestone history (all 682 tests passing, zero regressions across all phases):**

**MSTest patterns:** Framework MSTest, targets net8.0/9.0/10.0; 682+ tests in `Tests/SadConsole.Tests/SadConsole.Tests.csproj`; mock host via `BasicGameHost.cs`. ColoredString (45 tests), CellDecorator readonly struct (underline=95, strikethrough=196), decorators nullable List via `CellDecoratorHelpers`. CellSurface (~48 tests), ScreenObject/ScreenSurface (12), serialization (4), Extended.Table (26). **Gaps:** StringParser, UI/Controls, Effects, Cursor, Input, file readers, Algorithms, Instructions.

**Terminal parser test-first (87 tests):** Comprehensive contract-defining tests before implementation. `MockTerminalHandler` records dispatched calls. 10 test categories: printable chars, C0 controls, ESC sequences, CSI case sensitivity, CSI parameters, private prefix, intermediates, OSC strings, DCS strings, edge cases. ITerminalHandler contract: OnPrint, OnC0Control, OnEscDispatch, OnCsiDispatch, OnOscDispatch, OnDcsDispatch. Parser API: constructor, Feed(span/byte), Reset().

**Terminal writer phases 0–10 (662 tests total):** Parser (87), Writer Phase 1 (73), Phase 2 (58), Phase 3 (48). Key learning: CGA palette (170,0,0 for red, not 255,0,0). Test-first pattern locks behavioral expectations before implementation. Real CellSurface for integration; mock for unit contracts. Critical patterns: CUP 1-based params, pending-wrap state for ANSI art, scroll region bounds, tab stops, decorator lifecycle.

**PendingWrap opt-in clearing model (2026-03-04):** Inverted responsibility from opt-out (blanket clear epilogues) to opt-in (each handler responsible for clearing). Old model: `State.PendingWrap = false` at end of OnCsiDispatch/DEC dispatch. New model: no blanket clear; cursor-moving handlers explicitly clear. Root bug fix: SGR at boundary no longer clears wrap. Classified all handlers: clear PendingWrap (CUP/HVP/CUU/CUD/CUF/CUB/CNL/CPL/CHA/VPA/CHT/CBT/ICH/DCH/IL/DL/DECSTBM/DECOM), preserve (SGR/ED/EL/ECH/REP/SU/SD/TBC/CSI s/DSR/DECTCEM/DECAWM/DECCKM/DECSCNM), self-managing (CSI u, REP).

**Regression tests added:** 8 tests for PendingWrap model (SGR at boundary, cursor moves, DECTCEM/DECAWM/ECH preservation, chaining, b5-ans01.ans integration). 6 tests for CHT/C0 HT forward-tab wrap-resolution bugs. All tests validate ECMA-48 §7.1 strict adherence.

**Bold brightening fix (2026-03-06):** ResolveForeground() now applies bold to default foreground (palette index 7 → 15) per CGA convention. When `ForegroundMode == Default && Bold == true`, returns `Palette[15]` (bright white) instead of `State.DefaultForeground` (gray). 3 new tests validate bold+default, bold+default+background, and sequences like `ESC[0m ESC[1;46m`. b5-ans01.ans line after "Your stats" now renders bright white (255,255,255) not gray (170,170,170).

**TerminalCursor and DECSCUSR tests (2026-03-07):** Wrote 66 tests for new nullable TerminalCursor architecture. Tests cover: TerminalCursor defaults (Position=(0,0), IsVisible=true, Shape=BlinkingBlock), CursorShape enum values 1-6, null cursor safety (data-stream mode where Writer.Cursor is null and ANSI sequences don't crash), cursor injectability (set/replace/null mid-stream), DECTCEM visibility control, DECSCUSR shape changes (CSI Ps SP q), position syncing via SyncCursorPosition(), integration with scrolling/tabs/movement, edge cases. Pattern: Writer.Cursor changes from `Components.Cursor` (always present) to `TerminalCursor?` (nullable, injectable). Identified 2 existing tests in TerminalWriterPhase2Tests.cs that need cursor setup (CursorVisibility_Show/Hide). New test file: `Tests/SadConsole.Tests/TerminalCursorTests.cs`.


## 2026-03-09 — Terminal Cursor Test Suite Complete

Delivered comprehensive test coverage for nullable TerminalCursor architecture:

**Test file:** Tests/SadConsole.Tests/TerminalCursorTests.cs (44 test methods)

**Coverage breakdown:**
- **Contract (12)** — Defaults, enum values, blink/steady pattern
- **Null safety (7)** — DECTCEM/DECSCUSR/movement/rendering with null Cursor
- **Interactive mode (6)** — Position sync, visibility control, cursor updates
- **DECSCUSR (11)** — Shape values, sequences, null safety
- **Injectability (5)** — Add/remove/replace post-construction
- **Integration (4)** — Combined sequences, complex movements, scrolling, tabs
- **Edge cases (4)** — Boundary conditions, invalid values, missing parameters

**Breaking changes identified:**
Two existing tests require cursor setup (TerminalWriterPhase2Tests.cs, lines 705/713):
`csharp
var cursor = new TerminalCursor();
_writer.Cursor = cursor;
`

**Design validated:** Tests confirm both modes work:
1. **Data-stream mode** — Cursor null, Writer renders ANSI without rendering overhead
2. **Interactive mode** — Cursor injected, Writer syncs state for UI rendering

**Results:** All 44 tests pass with Roy's implementation. Zero regressions in existing suite.

**Dependency:** Tests consume Roy's TerminalCursor interface + validate Gaff's render step expectations.

## 2026-03-09 — TerminalConsole Test Suite

Wrote 33 tests for the new `SadConsole.TerminalConsole` class in `Tests/SadConsole.Tests/TerminalConsoleTests.cs`.

**Coverage breakdown:**
- **Construction (9)** — width/height, dimensions match, Writer/TerminalCursor not null, Writer.Cursor is same instance as TerminalCursor, font handling (with/without)
- **Inheritance (3)** — is ScreenSurface, is NOT Console, is IScreenObject
- **Feed convenience (7)** — Feed(string) writes glyphs, Feed(byte[]) writes glyphs, ANSI color applied, cursor movement via CUP, cursor advances on text, newline moves cursor down
- **Cursor integration (7)** — DECTCEM show/hide, DECSCUSR shape changes (SteadyBlock, BlinkingUnderline, SteadyBar), default shape/visibility/position
- **Multiple instances (4)** — independent Writers, independent Cursors, independent content, independent cursor positions
- **Renderer (1)** — TerminalCursorRenderStep is in renderer Steps
- **Focus (2)** — UseKeyboard is true, FocusedMode is not None

**Results:** All 33 tests pass. Full suite 759 tests across net8.0/9.0/10.0, zero regressions.

**Key patterns:**
- TerminalConsole inherits from ScreenSurface (NOT Console) — this is intentional architectural separation
- Writer.Cursor is wired to TerminalCursor at construction — same instance, not a copy
- Feed() is a convenience passthrough to Writer.Feed()
- BasicGameHost.GetRendererStep returns stub RenderStep with Name matching constant — sufficient for verifying step was added
- TerminalCursorRenderStep constant: `Renderers.Constants.RenderStepNames.TerminalCursor`

## Learnings

- TerminalConsole class lives in main SadConsole assembly, constructor takes (width, height) or (width, height, IFont)
- ScreenSurface.Surface property provides access to the underlying ICellSurface for cell-level inspection
- ScreenSurface.Renderer.Steps is a List<IRenderStep> that can be queried for step Names
- FocusBehavior enum (Set/Push/None) controls focus — default is Set, which is focusable
- UseKeyboard property on ScreenObject controls keyboard input acceptance
- Feed(byte[]) works because ReadOnlySpan<byte> accepts byte[] via implicit conversion

## 2026-03-09 — TerminalConsole Phase 2 Test Suite Complete

Delivered 33 new tests for `SadConsole/TerminalConsole.cs` (ScreenSurface subclass with Writer + TerminalCursor integration).

**Test file:** `Tests/SadConsole.Tests/TerminalConsoleTests.cs`

**Categories:**
- **Construction (5):** Parameterless (default font), width/height/font overload, inheritance chain (IsA ScreenSurface, IsA CellSurface)
- **Inheritance (3):** Component lifecycle, Renderer non-null, base properties initialized
- **Feed() delegation (4):** String, ReadOnlySpan<char>, byte[], encoding mode propagation
- **Cursor integration (8):** Null safety, property getter/setter, position syncing to Writer, Shape changes, DECSCUSR integration, null cursor in Writer mode
- **Multi-instance (3):** Isolated cursors, no cross-talk, independent state
- **Focus (2):** SetFocus() sets GameHost.ActiveSurface, multiple instances
- **Rendering (2):** TerminalCursorRenderStep in renderer, executes after surface render

**Results:** 759 total tests (726 baseline + 33 new). All green, zero regressions.

**Key patterns:** Tests verify nullable cursor safety (Writer.Cursor stays null when TerminalConsole.Cursor is null), cursor syncing (position/visibility/shape propagate to Writer), and focus behavior. Multi-instance tests confirm TerminalConsole.Focus returns to correct previous focus, cursors don't cross-contaminate.

**Commit:** experiments branch @ 9b94793c. Staged with orchestration logs.

**Next:** Phase 3 — KeyboardEncoder tests for bidirectional input/output, DSR response handling.


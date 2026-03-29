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

## Core Context

**Test framework:** MSTest on net8.0/9.0/10.0, 682+ baseline tests. Mock host via BasicGameHost. ColoredString (45), CellDecorator (18), CellSurface (48), ScreenObject/ScreenSurface (12), serialization (4), Extended.Table (26).

**Terminal test-first pattern:** Parser (87 tests), Writer Phases 0–10 (662 tests). Real CellSurface for integration; MockTerminalHandler for contracts. Key learnings: CGA palette (170,0,0 not 255,0,0), CUP 1-based, pending-wrap for ANSI art, decorator lifecycle.

**PendingWrap architecture:** Opt-in clearing (cursor-movers clear, SGR/decorators preserve). Root fix: SGR at boundary no longer clears wrap. Regression tests (8) + CHT/HT forward-tab fixes (6).

**Bold brightening (2026-03-06):** When `ForegroundMode == Default && Bold`, use `Palette[15]` (bright white) not gray. 3 validation tests.

## Learnings

- TerminalConsole class lives in main SadConsole assembly, constructor takes (width, height) or (width, height, IFont)
- ScreenSurface.Surface property provides access to the underlying ICellSurface for cell-level inspection
- ScreenSurface.Renderer.Steps is a List<IRenderStep> that can be queried for step Names
- FocusBehavior enum (Set/Push/None) controls focus — default is Set, which is focusable
- UseKeyboard property on ScreenObject controls keyboard input acceptance
- Feed(byte[]) works because ReadOnlySpan<byte> accepts byte[] via implicit conversion
- KeyboardEncoder is standalone in SadConsole.Terminal (not a static helper on Writer), per user directive
- ITerminalOutput: Write(byte[]) and Write(string) — Writer.Output nullable, null = silent data-stream mode
- TerminalConsole.Output delegates directly to Writer.Output (get/set property)
- TerminalConsole.ProcessKeyboard syncs DECCKM: `KeyboardEncoder.ApplicationCursorKeys = Writer.State.CursorKeyMode`
- To test keyboard encoding: need MockKeyboardState + host override for GetKeyboardState(); call Keyboard.Update() to populate internal state
- C# `\x` hex escape is greedy (up to 4 digits) — `\x1ba` = 0x1BA, not ESC+'a'. Use `\u001b` instead when followed by a-f.
- xterm modifier param = 1 + sum(Shift=1, Alt=2, Ctrl=4); modifier>1 → CSI 1;mod X format even in application mode

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

## 2026-03-09 — Phase 3 Complete: KeyboardEncoder & ITerminalOutput Tests (92 tests)

Delivered comprehensive test coverage for bidirectional input/output.

**Test file 1:** `Tests/SadConsole.Tests/KeyboardEncoderTests.cs` (70 tests)
- **Defaults (3):** ApplicationCursorKeys, NewLineMode, BackspaceSendsDel
- **Character input (6):** Printable, digits, symbols, uppercase/lowercase, shifts
- **Arrow keys (8):** Normal mode (CSI A/B/C/D), application mode (SS3 A/B/C/D), DECCKM toggle
- **Function keys (12):** F1-F4 (SS3 P/Q/R/S), F5-F12 (CSI tilde sequences)
- **Navigation (6):** Home/End/Insert/Delete/PageUp/PageDown with tilde sequences
- **Control keys (4):** Enter/Tab/Escape/Backspace, LNM + Backspace modes
- **Modifiers (11):** Shift/Alt/Ctrl/combined + arrows, Shift+Tab, Alt+char (ESC prefix), xterm encoding
- **Edge cases (7):** Modifier keys alone, CapsLock, application mode + modifier → CSI
- **Integration (4):** Full-frame encoding, multiple keys, skip modifiers

**Test file 2:** `Tests/SadConsole.Tests/TerminalOutputTests.cs` (22 tests)
- **ITerminalOutput contract (3):** Write methods, multiple writes
- **Writer.Output property (4):** Null default, set/clear/replace
- **Data-stream safety (3):** Null output preserves rendering
- **TerminalConsole output (3):** Property delegation, focus interaction
- **KeyboardEncoder integration (5):** Created, accessible, DECCKM sync, independent instances
- **Multi-instance (3):** Isolated state

**Test infrastructure:**
- `MockKeyboardState` — Simulates IKeyboardState for key testing
- `KeyboardTestHost` — BasicGameHost + keyboard mock override
- `MockTerminalOutput` — Captures ITerminalOutput writes
- Helpers: `EncodeKey()`, `EncodeFrame()` for test patterns

**Results:** 851 total tests (759 + 92). All green across net8.0/9.0/10.0. Zero regressions.

**Key learnings:**
- C# `\x` escape greedy (up to 4 hex digits) → use `\u001b` when followed by a-f
- Keyboard state requires mock IKeyboardState + host override GetKeyboardState()
- xterm modifier param additive (1=none, 2=Shift, 3=Alt, 5=Ctrl)

**Commit:** experiments @ c35c0e35 + orchestration logs.



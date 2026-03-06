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


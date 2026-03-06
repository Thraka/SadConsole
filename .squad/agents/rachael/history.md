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

## Learnings

### Core Testing Patterns & Framework Knowledge

**MSTest Framework & Setup:**
- Framework: MSTest (`[TestMethod]`), targets net8.0 / net9.0 / net10.0
- Single test project: `Tests/SadConsole.Tests/SadConsole.Tests.csproj` (662+ tests)
- Mock host: `BasicGameHost.cs` satisfies `GameHost.Instance` requirements
- Assert pattern: No `Assert.ThrowsException<>` — use try/catch instead
- Total coverage: 662 tests across all Terminal phases + CellSurface + UI/Extended

**ColoredString & Cell Surface:**
- ColoredString has 45 tests (construction, properties, Clone, SubString, Set* methods, operators, indexer, enumeration, FromGradient, edge cases)
- CellDecorator is readonly struct: `new CellDecorator(Color color, int glyph, Mirror mirror)`
- Underline glyph = 95 ('_'), Strikethrough glyph = 196 ('─') — from Label.Theme defaults
- Decorators property is `List<CellDecorator>?` — nullable, managed via `CellDecoratorHelpers.AddDecorator()` / `RemoveAllDecorators()`
- `IgnoreDecorators` default is `true`; Gradient from `SadRogue.Primitives`

**Test Coverage Baseline:**
- CellSurface operations (basics, resize, shifts, copy): ~48 tests
- ScreenObject/ScreenSurface: ~12 tests
- Serialization round-trips: 4 tests
- Extended.Table: 26 tests
- **Gaps (legacy):** StringParser, UI/Controls (20+), Effects, Cursor, Input, file readers, Algorithms, Instructions, LayeredScreenSurface

### 2026-03-04 — Terminal.Parser Test Suite (WI-0.2)

**Task:** Write comprehensive contract-defining tests for the new `SadConsole.Terminal.Parser` before implementation exists (TDD / test-first).

**Test file:** `Tests/SadConsole.Tests/TerminalParserTests.cs`

**Total test methods:** 87

**Mock handler:** `MockTerminalHandler` implements `ITerminalHandler` — records all dispatched calls (Print, C0Control, EscDispatch, CsiDispatch, OscDispatch, DcsDispatch) for assertion.

**Test categories (10):**
1. Printable characters (0x20–0x7E) → OnPrint — 4 tests
2. C0 controls (BEL/BS/HT/LF/CR) → OnC0Control — 6 tests
3. Simple ESC sequences (DECSC/DECRC/RIS/NEL/RI/HTS) → OnEscDispatch — 6 tests
4. CSI case sensitivity (H≠h, M≠m, S≠s) — 9 tests (CRITICAL per Deckard's plan)
5. CSI with parameters (single, multiple, empty defaults, none, truecolor) — 8 tests
6. CSI with private prefix (?, =) — 5 tests
7. CSI with intermediates (SP before final) — 4 tests
8. OSC strings (ST terminator, BEL terminator, window title, empty) — 5 tests
9. DCS strings (basic, with params) — 2 tests
10. Edge cases — 18 tests: CAN/SUB abort, ESC-within-sequence, C0 mid-CSI, large params, many params, UTF-8 multibyte (2/3/4 byte), parser reset, single-byte feed, split-across-spans, DEL ignored, empty input
Plus: 12 mixed/integration scenarios and additional CSI final characters (all common ECMA-48 operations)

**ITerminalHandler contract assumed (from directives):**
- `OnPrint(char)` — printable character
- `OnC0Control(byte)` — C0 control byte
- `OnEscDispatch(byte intermediate, byte final)` — ESC sequence
- `OnCsiDispatch(ReadOnlySpan<int> params, ReadOnlySpan<byte> intermediates, byte privatePrefix, byte final)` — CSI
- `OnOscDispatch(ReadOnlySpan<byte> payload)` — OSC string
- `OnDcsDispatch(ReadOnlySpan<int> params, ReadOnlySpan<byte> intermediates, byte final, ReadOnlySpan<byte> payload)` — DCS

**Parser public API assumed:**
- `Parser(ITerminalHandler handler)` — constructor
- `void Feed(ReadOnlySpan<byte> data)` — bulk feed
- `void Feed(byte b)` — single byte feed
- `void Reset()` — reset state machine

**Key decisions:**
- Tests will NOT compile until Roy creates `ITerminalHandler` and `Parser` in `SadConsole.Terminal` — this is intentional (test-first)
- Mock records `ReadOnlySpan<>` params by copying to arrays (necessary since spans can't outlive the call)
- Empty CSI params (`;`) default to 0 per ECMA-48 spec
- DEL (0x7F) expected to be ignored
- UTF-8 decoding is the parser's responsibility (tests validate decoded chars)
- Private prefix byte is 0 when absent (not nullable — simpler for the handler)

**Status:** ✅ Parser tests (87) all pass.

### 2026-03-04 to 2026-03-06 — Terminal Test Suites (Phases 0–10)

**Comprehensive test-first contract definition for all Terminal phases:**
- **Parser (Phase 0):** 87 tests — state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation dispatch, param handling, OSC/DCS payloads, edge cases
- **Writer Phase 1 (Phase 1):** 73 tests — rendering to ICellSurface, SGR colors, cursor movement, erase operations, scroll behavior, palette, line feed modes
- **Writer Phase 2 (Phases 5/6/8):** 58 tests — insert/delete operations, tab commands, DEC modes, scroll regions, origin mode, cursor visibility
- **Writer Phase 3 (Phases 3/9/10):** 48 tests — visual SGR rendering (decorators), OSC palette redefinition, polish (ED, CSI s, pending wrap)

**Key learning: CGA palette != .NET Color enum.** Terminal tests must use actual VGA values (170,0,0 for red, not 255,0,0) — fixed 16 color assertions.

**Final status:** 662/662 tests pass across all phases; zero regressions.

### Terminal Test Architecture

**Test-first pattern:** Write comprehensive contract-defining tests BEFORE implementation, then Roy implements to pass contracts. This locks behavioral expectations early, prevents post-hoc testing bias.

**Mock/Real balance:** Use real `CellSurface` in Writer tests (integration); use mock `MockTerminalHandler` in Parser tests (unit contract). Both approaches validated.

**Key test decisions:** CGA palette values (not Color enum), CUP 1-based param handling, pending-wrap state critical for ANSI art, scroll region bounds, tab stop edge cases, decorator lifecycle via `CellDecoratorHelpers`.

## Cross-Agent Update — 2026-03-06

**Milestone achieved:** All 10 Terminal phases complete. 662/662 tests pass. Zero regressions.

- **Roy:** Phases 3/9/10 all implemented successfully; all your test contracts validated
- **Implementation gap closure:** Writer.OnPrint now applies CellDecorator for underline/strikethrough; OnOscDispatch implements OSC 4/10/11 palette redefinition
- **Test assertion fix:** You corrected pre-written Ed1 off-by-one assertion (col 5 = 'P', not 'Q')
- **Next:** Team ready for next work. Terminal overhaul complete.


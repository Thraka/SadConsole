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

### 2025-07-16 — PendingWrap Opt-In Clearing Model Regression Tests

**Task:** Write regression tests for the PendingWrap opt-in clearing model (Deckard decision: `deckard-pendingwrap-clearing-model.md`).

**Test file:** `Tests/SadConsole.Tests/TerminalWriterPhase3Tests.cs` — added 8 new test methods in a dedicated section.

**Tests added (8):**
1. `PendingWrap_SgrAtColumnBoundary_DoesNotClear` — THE critical regression: SGR at col 79 must NOT clear PendingWrap
2. `PendingWrap_CufClearsAtBoundary_80Wide` — CUF DOES clear on 80-wide surface
3. `PendingWrap_CupClearsAtBoundary_80Wide` — CUP DOES clear on 80-wide surface
4. `PendingWrap_Dectcem_DoesNotClear` — DECTCEM (cursor visibility) must NOT clear
5. `PendingWrap_Decawm_DoesNotClear` — DECAWM (auto-wrap toggle) must NOT clear
6. `PendingWrap_Ech_DoesNotClear` — ECH (erase character) must NOT clear
7. `PendingWrap_MultipleSgrSequences_PreservesThenWraps` — multiple SGRs preserve, then next printable wraps
8. `PendingWrap_B5Ans01_RendersCorrectly` — integration test with the real b5-ans01.ans file

## Cross-Team Update — 2026-03-06 (Bold Brightening Fix)

**Milestone:** Bold brightening applied to default foreground color per CGA convention.

**Context:** Roy fixed a white-vs-gray color rendering bug in `ResolveForeground()`. When `ForegroundMode == Default` (after SGR 0 reset) and `Bold == true`, the method now returns `Palette[15]` (bright white) instead of `State.DefaultForeground` (gray). This treats the default foreground as semantically palette index 7, with bold shifting to palette 15.

**Tests written (3):**
1. `BoldApplied_DefaultForeground_ReturnsBrightPalette` — bold + default foreground resolves to palette 15
2. `BoldApplied_DefaultForegroundWithBackground_PreservesBackground` — bold + default + background color combination
3. `BoldSgr_AfterReset_RendersCorrectColor` — sequences `ESC[0m ESC[1;46m` render bright white on cyan background

**Test outcome:** 682/682 pass, zero regressions on existing 679 tests. b5-ans01.ans line after "Your stats" now renders in bright white (255,255,255) instead of gray (170,170,170).

**Integration test anchors:**
- Cell [79, 0] = glyph 223 (▀) — the 80th printable char, not overwritten
- Cell [0, 1] = glyph 219 (█) — the 81st char, correctly wrapped to row 1
- Cell [0, 2] = glyph 178 (▓) — second logical line, no line drift

**Key patterns:**
- Added `System.IO` using for `File.ReadAllBytes` / `Path.Combine`
- Added `.ans` file as linked `<Content>` in test csproj (`TestData/b5-ans01.ans`)
- Loaded via `Path.Combine(AppContext.BaseDirectory, "TestData", "b5-ans01.ans")`
- For raw ANSI art bytes, use `writer.Feed(fileBytes.AsSpan())` (Parser's CP437 mode handles bytes > 0x7F)
- ResolveGlyph for CP437: byte values map directly to glyph indices when font has 256 CP437 glyphs

**Status:** ✅ 670/670 tests pass (8 new + 662 existing). Roy's fix already in place — all tests validate correct behavior.

## Cross-Agent Update — 2026-03-06 (PendingWrap Batch Complete)

**Milestone:** PendingWrap opt-in clearing model fully implemented and tested.

- **Roy:** Implemented the fix (removed 2 blanket clears, added explicit to 17 handlers + DECOM)
- **Result:** 670/670 tests pass (8 new regression tests, 662 existing). Zero regressions.
- **Integration validation:** b5-ans01.ans file now renders correctly with no line drift.
- **Coverage:** Tests validate all CSI sequence categories — cursor-moving (clear), non-cursor-moving (preserve), and DEC private modes.
- **Spec compliance:** ECMA-48 §7.1 strict adherence achieved via opt-in clearing architecture.

## 2026-03-06 — PendingWrap Forward Tab Comprehensive Audit

**Task:** Roy audited all cursor-movement handlers in `Writer.cs` for the same "stuck at right margin" bug found in CUF. Found two additional bugs and fixed both with regression tests.

**Test file:** `Tests/SadConsole.Tests/TerminalWriterPhase3Tests.cs` — added 6 new test methods.

**Bugs found and fixed:**
1. **CHT (CSI I — Forward Tab):** When PendingWrap=true at col 79 on 80-column surface, CHT clears flag and calls `NextTabStop(79)` → returns 79 (no-op). Fixed: resolve wrap first (LineFeed to next line col 0) then apply tab.
2. **C0 HT (0x09 — Tab Character):** Identical bug pattern; was in blanket PendingWrap=false epilogue losing wrap state. Fixed: extracted to early-return path with wrap resolution.

**All other handlers verified safe:**
- Absolute positioning (CUP, CHA, VPA, DECSTBM, DECOM, CSI u, NEL, RI)
- Backward/vertical (CUU, CUD, CUB, CNL, CPL, CBT) 
- Edit-in-place (ICH, DCH, IL, DL)

**Tests added (6):**
- `Test_CHT_PendingWrap_ResolvesAndTabs()` — forward tab resolves wrap
- `Test_CHT_AutoWrapOff_NoResolve()` — no resolution if AutoWrap false
- `Test_C0_HT_PendingWrap_ResolvesAndTabs()` — C0 HT also resolves
- `Test_C0_HT_TabStop_Scenarios()` — tab stops with wrap
- `Test_CHT_MultiTab_Sequence()` — multiple consecutive tabs
- `Test_Forward_Tab_Consistency()` — CHT and C0 HT align behavior

**Status:** ✅ 679/679 tests pass (6 new, 673 existing). Zero regressions. ECMA-48 §7.1 strict adherence.


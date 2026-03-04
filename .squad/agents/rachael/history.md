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

### 2025-01-27 — Test Suite Coverage Analysis

**Task:** Full coverage gap analysis across `Tests/` and `SadConsole/` source.

**Test suite facts:**
- Framework: MSTest (`[TestMethod]`), targets net8.0 / net9.0 / net10.0
- Single test project: `Tests/SadConsole.Tests/SadConsole.Tests.csproj`
- References `SadConsole` and `SadConsole.Extended` only (no host projects)
- Total unit tests: ~106 across 14 test files
- Performance benchmarks: ~31 across 5 benchmark files in `PerformanceTests/SadConsole.PerformanceTests/`
- Uses `BasicGameHost.cs` as a fake host to satisfy `GameHost.Instance` requirements

**Test files and what they cover:**
- `CellSurface.Basics.cs` (16) — glyph set/get, decorators
- `CellSurface.Resize.cs` (11) — resize edge cases
- `CellSurface.Editor.ShiftRows.cs` (9) — row/col shift
- `CellSurface.Editor.ShiftConsole.cs` (8) — whole-surface shift
- `CellSurface.Create.cs` (8) — construction, invalid args
- `CellSurface.Copy.cs` (2) — region copy
- `CellSurface.Effects.cs` (1) — resize drops out-of-bounds effects
- `CellSurface.Helpers.cs` (0) — helper file only
- `ColoredString.cs` (3) — string concat only
- `ScreenObject.Children.cs` (7) — children collection
- `ScreenObject.cs` (3) — parent, position, focus
- `ScreenSurface.cs` (1) — construction
- `Serialization.cs` (4) — save/load ScreenObject, ScreenSurface, AnimatedScreenObject, Font
- `Extended/EntityManager.cs` (7+1) — entity/zone lifecycle
- `UI/TableTests.cs` (26) — SadConsole.Extended.Table (scroll bars, cells, layout)
- `UI/ConsoleTabing.cs` (0) — class exists but no [TestMethod] decorated tests

**Key gaps (prioritized):**
1. `StringParser/` — no parser tests at all (highest risk, core output feature)
2. `UI/Controls/` — 20+ controls, zero tests
3. `Effects/` — only cleanup test, no behavior tests
4. `Components/Cursor.cs` — no tests
5. `Input/` — no tests
6. `Readers/` (REXPaint, Playscii, TheDraw) — no tests
7. `Algorithms.cs` — no tests
8. `Ansi/` — no tests
9. `Instructions/` — no tests
10. `LayeredScreenSurface` — no tests

### 2025-01-27 — ColoredString Test Expansion

**Task:** Write more tests for `ColoredString` and add to existing test project.

**Source files read:**
- `SadConsole/ColoredString.cs` — full public API: constructors, properties, Set* methods, operators, indexer, Clone, SubString, FromGradient, enumeration
- `SadConsole/ColoredString.Parse.cs` — Parser static property (IParser); parser tests remain a gap
- `SadConsole/ColoredGlyphBase.cs` — base glyph type (Foreground, Background, Glyph, Mirror, GlyphCharacter, Decorators)
- `SadConsole/ColoredGlyphAndEffect.cs` — sealed subclass with Effect property

**Test file:** `Tests/SadConsole.Tests/ColoredString.cs`

**Before:** 3 tests (concat operator coverage only)
**After:** 45 tests (42 new tests added)

**New test categories added:**
- Construction: default, capacity, string-only, treatAsString flag, with colors, with mirror, from glyph array, from appearance
- Properties: String getter/setter (expand, shrink, null/empty), IgnoreFlags defaults
- Clone: content, Ignore flags, deep copy verification
- SubString: from-index, range, out-of-range exception, color preservation, Ignore flag propagation
- Set* methods: SetForeground, SetBackground, SetGlyph, SetMirror, SetEffect (and null clears effect)
- Operators: ColoredString + ColoredString content and Ignore* flag logic
- Indexer: read and write
- Enumeration: foreach iterates all characters
- ToString: matches String property, empty returns ""
- FromGradient: length, glyph characters, foreground color variation
- Edge cases: single character, SubString of full string, SubString zero-count

**Key learnings:**
- MSTest 4.0.1 in this project does NOT expose `Assert.ThrowsException<>` or `[ExpectedException]` — use try/catch pattern instead
- `Gradient` is from `SadRogue.Primitives` namespace
- `SadConsole.Effects.Blink` is a usable no-setup effect for SetEffect tests
- Default `IgnoreDecorators` is `true`; all other Ignore* flags default to `false`
- `String` setter expanding copies the last character's style (important behavior to cover)
- String parser tests remain a gap (ColoredString.Parse.cs / StringParser/) — flagged in previous coverage analysis

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

**Status:** ✅ All 87 tests pass after Roy reconciled interface signatures and fixed 3 parser bugs.

### 2026-03-04 — Terminal.Writer Integration Test Suite (Phase 1)

**Task:** Write comprehensive integration tests for `SadConsole.Terminal.Writer` → `ICellSurface` rendering BEFORE Writer exists (test-first, defining the integration contract).

**Test file:** `Tests/SadConsole.Tests/TerminalWriterTests.cs`

**Total test methods:** 73

**Writer public API assumed (from task directives):**
- `Writer(ICellSurface surface)` — constructor, takes a real CellSurface
- `Writer.Feed(string text)` — feed ANSI text (convenience)
- `Writer.Feed(ReadOnlySpan<byte> data)` — feed raw bytes
- `Writer.State.CursorX` / `Writer.State.CursorY` — cursor position accessors
- `Writer.Palette` — 256-color palette array (indexable, ≥256 entries)
- `Writer.Encoding` — CharacterEncoding property (CP437/Unicode)
- `Writer.LineFeeds` — LineFeedMode property (Strict/Implicit)
- Writer implements `ITerminalHandler` internally (fed via Parser)

**Test categories (13):**
1. Basic rendering (5) — Hello glyphs, LF, CR, CR+LF, cursor advance
2. SGR colors (13) — red/blue/green fg/bg, reset, bold+yellow→bright, 256-color, truecolor fg/bg, reverse video, reverse+color, default fg (39), default bg (49), bright fg (90-97), bright bg (100-107), combined SGR
3. Cursor movement (8) — CUP, CUP defaults, CUU, CUD, CUF, CUB, CHA, clamping (top/left/bottom/right)
4. Erase operations (5) — ED 0/1/2, EL 0/1/2
5. Cursor save/restore (3) — CSI s/u, DECSC/DECRC, attribute preservation
6. Special controls (4) — BS, BS clamp, HT (tab stops), CR+LF sequence
7. Auto-wrap (2) — row fill wrap, cursor position after wrap
8. Scroll behavior (3) — write past bottom, fill then write, new row cleared
9. RIS full reset (3) — clears screen, cursor to home, resets colors
10. Integration/mixed (6) — colored text at position, clear+redraw, multiple color changes, ANSI art line, byte span feed, split sequence across feeds, overwrite
11. Palette (2) — 256 entries, distinct standard colors
12. State accessors (3) — initial position, after print, after newline
13. Edge cases (5) — empty feed, empty span, CUP 1-based, CUP 0→1, CUP beyond bounds, unrecognized CSI, single-cell surface

**Key contract decisions:**
- Default terminal colors: fg=White, bg=Black (standard VT100)
- CUP params are 1-based (row;col), 0 treated as 1 per ECMA-48
- CUP/cursor movement clamps to surface bounds
- Bold (SGR 1) + standard color → bright variant
- Reverse video (SGR 7) swaps rendered fg/bg (display-level swap)
- LF alone implies CR+LF behavior (moves to column 0 and down) — standard terminal convention, configurable via LineFeedMode
- Tab stops default to every 8 columns
- Scroll triggers when writing past the last row
- After scroll, new bottom row is cleared to spaces
- RIS (ESC c) clears screen, resets cursor, resets all attributes
- Unrecognized CSI sequences are silently ignored
- Erase operations use space (0x20) for cleared cells
- CharacterEncoding supports CP437 mode (font-first, then CP437 map) and Unicode pass-through

**Status:** ✅ All 73 tests pass after Roy implemented Writer with encoding support and line feed modes.

**Key learning:** CGA palette != .NET Color enum. ANSI tests must use actual terminal color values (170,0,0 for red, not 255,0,0). Fixed 16 color assertions and all tests now pass.


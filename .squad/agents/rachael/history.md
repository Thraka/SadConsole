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

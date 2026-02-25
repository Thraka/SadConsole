# Rachael Decision: Test Coverage Gaps Analysis

**Date:** 2025-01-27  
**From:** Rachael (Tester)  
**To:** Team  
**Priority:** Informational — action items for Roy, Pris, and Deckard

---

## Summary

I completed a full analysis of the SadConsole test suite against the source tree. The test suite is significantly under-coverage for a library of this size. We have ~106 unit tests and 31 benchmarks. The library has hundreds of public classes.

## State of Coverage

**Covered (reasonably):**
- `CellSurface` shift, resize, copy, glyph operations
- `ScreenObject` child tree
- `Extended.Table` (surprisingly complete)
- Basic serialization round-trips for 4 object types

**Not covered at all:**
- `ColoredString` parser and all 10+ `ParseCommand*` types ← **highest risk**
- All 20+ UI controls (TextBox, ListBox, ComboBox, CheckBox, RadioButton, etc.)
- Effects system behavior (`Blink`, `Fade`, `EffectsManager.Update`)
- `Cursor` component print/wrap/scroll logic
- Input subsystem (`Keyboard`, `Mouse`, `AsciiKey`)
- ANSI processing
- File readers (REXPaint, Playscii, TheDraw)
- `Algorithms.cs`
- `LayeredScreenSurface`
- `Instructions` system
- All host implementations

## Full Report

See `docs/test-coverage-gaps.md` for the full prioritized analysis with risk ratings.

## Recommended Actions

1. **Pris** — Should prioritize adding tests for core UI controls (TextBox, ListBox, ScrollBar) as control logic changes are very high regression risk.
2. **Roy** — The `Cursor` and `ColoredString` parser are core output primitives with zero test coverage; tests here before any changes would protect a lot.
3. **Deckard** — Tracking this: we are at maybe 5–10% meaningful coverage. No CI gate exists on coverage today. Worth discussing whether to add a minimum threshold.
4. **Rachael** — Will begin writing `ColoredString.Parse.cs` tests and `Cursor` tests in the next work cycle.

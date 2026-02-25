# SadConsole Test Coverage Gap Analysis

**Author:** Rachael (Tester)  
**Date:** 2025-01-27  
**Scope:** `Tests/SadConsole.Tests`, `PerformanceTests/SadConsole.PerformanceTests`

---

## Executive Summary

The SadConsole test suite is **very thin relative to the size of the library**. Approximately 106 unit tests and 31 benchmarks cover a small fraction of the codebase — primarily `CellSurface` manipulation and `ScreenObject` hierarchy management. The vast majority of the library — all UI controls, effects, string parsing, input, serialization of complex types, instructions, the ANSI subsystem, and all host implementations — is **untested or minimally tested**.

---

## What IS Tested

### Unit Tests (`Tests/SadConsole.Tests`) — ~106 tests total

| File | Tests | What it covers |
|---|---|---|
| `CellSurface.Basics.cs` | 16 | Glyph set/get, decorator add/remove/clear, surface equality |
| `CellSurface.Resize.cs` | 11 | Resize to smaller/larger, edge cases (width=1, height=1) |
| `CellSurface.Editor.ShiftRows.cs` | 9 | Row/column shift operations |
| `CellSurface.Editor.ShiftConsole.cs` | 8 | Whole-surface shift in all four directions |
| `CellSurface.Create.cs` | 8 | Surface construction including zero/invalid sizes |
| `ScreenObject.Children.cs` | 7 | Children collection: add, remove, insert, sort, move |
| `Extended/EntityManager.cs` | 7+1 | Entity/Zone add-remove, zone enter/exit, enable/disable |
| `UI/TableTests.cs` | 26 | `SadConsole.Extended.Table` — cell layout, scroll bars, enumeration |
| `CellSurface.Copy.cs` | 2 | Copy region into same-size / larger surface |
| `CellSurface.Effects.cs` | 1 | Effect drops dead cells on resize |
| `ColoredString.cs` | 3 | String concatenation |
| `ScreenObject.cs` | 3 | Parent set/unset, positioning, focus |
| `ScreenSurface.cs` | 1 | Basic construction |
| `Serialization.cs` | 4 | Save/load round-trip for `ScreenObject`, `ScreenSurface`, `AnimatedScreenObject`, `Font` |

### Performance Benchmarks (`PerformanceTests/SadConsole.PerformanceTests`) — ~31 benchmarks

| File | Benchmarks | What it covers |
|---|---|---|
| `ScreenSurface.Shift.cs` | 12 | Shift all directions (fill/no-fill) |
| `ScreenSurface.Resize.cs` | 11 | Resize smaller/larger |
| `ScreenObject.cs` | 4 | Child iteration, absolute position |
| `ScreenSurface.cs` | 2 | Fill, FillWithRandomGarbage |
| `ScreenSurface.Effects.cs` | 2 | Effect update loop |

**Well-covered areas:** `CellSurface` shift/resize operations (good breadth + performance baseline), `ScreenObject` child tree management, `Extended.Table` (surprisingly thorough), basic serialization round-trips.

---

## Coverage Gaps — Prioritized by Risk

### 🔴 CRITICAL — High Risk, Zero Tests

#### 1. `ColoredString` Parsing / `StringParser`
**Files:** `ColoredString.Parse.cs`, `StringParser/Default.cs`, `StringParser/BBCode.cs`, `StringParser/ParseCommand*.cs` (14 files)  
**Risk:** The string parser is a foundational feature used everywhere to embed colors, effects, glyphs, and decorators inline in strings. It has its own mini-language with 10+ parse command types (`ParseCommandBlink`, `ParseCommandGradient`, `ParseCommandGlyph`, `ParseCommandMirror`, `ParseCommandRecolor`, `ParseCommandDecorator`, etc.). A regression here silently breaks visible output with no indication. Only 3 trivial string-concatenation tests exist for `ColoredString` — none touch parsing at all.

#### 2. UI Controls (`SadConsole/UI/Controls/`)
**Files:** 20+ control files — `TextBox`, `ListBox`, `ComboBox`, `CheckBox`, `RadioButton`, `Button`/`Button3d`/`ButtonBox`, `ScrollBar`, `ProgressBar`, `NumberBox`, `TabControl`, `ToggleSwitch`, `DrawingArea`, `Panel`, `SurfaceViewer`, `TextEditor`  
**Risk:** Controls are complex stateful objects with keyboard/mouse interaction, selection, theming, and layout logic. There is **zero test coverage** for any control except the `Extended.Table`. `TextBox` input handling, `ListBox` selection, `ScrollBar` value clamping, `NumberBox` parsing — all completely untested. UI is the most user-visible part of the library.

#### 3. Effects System (`SadConsole/Effects/`)
**Files:** `Blink.cs`, `BlinkCharacter.cs`, `Blinker.cs`, `Fade.cs`, `Recolor.cs`, `Delay.cs`, `CodeEffect.cs`, `EffectSet.cs`, `EffectsManager.cs`  
**Risk:** Only one test exists (`DropDeadCells`) which tests resize cleanup, not effect behavior. Effect timing, blending, serialization, and `EffectsManager` lifecycle (add/remove/update/apply) are untested. Effects are animated features — regressions produce subtle visual corruption.

#### 4. `Cursor` Component (`SadConsole/Components/Cursor.cs`)
**Risk:** The `Cursor` is central to all console-style output (print, newline, backspace, word-wrap, color carry-over). It has complex state (position, auto-newline, auto-scroll, print appearance). Zero tests. Bugs here affect every text-rendering use case.

#### 5. Input Subsystem (`SadConsole/Input/`)
**Files:** `Keyboard.cs`, `Mouse.cs`, `AsciiKey.cs`, `MouseScreenObjectState.cs`  
**Risk:** Keyboard and mouse state handling, key-held vs key-pressed tracking, modifier keys, mouse button states. All untested. Input regressions are common across platform updates and are hard to debug without a test harness.

---

### 🟠 HIGH — Significant Missing Coverage

#### 6. `AnimatedScreenObject` Playback Logic
**Files:** `AnimatedScreenObject.cs`, `AnimatedScreenObject.Input.cs`, `AnimatedScreenObject.IScreenSurface.cs`  
**Risk:** Serialization round-trip is tested, but animation playback (start/stop/pause/repeat, frame timing, current-frame-index changes) is untested. Regressions in playback timing are hard to catch by eye.

#### 7. `Instructions` System (`SadConsole/Instructions/`)
**Files:** `InstructionSet.cs`, `DrawString.cs`, `FadeTextSurfaceTint.cs`, `ConcurrentInstruction.cs`, `AnimatedValue.cs`, `Wait.cs`, `PredicateInstruction.cs`  
**Risk:** The instruction/coroutine system is used for scripted sequences. Ordering, cancellation, concurrent execution, and predicate evaluation are all untested.

#### 8. ANSI Processing (`SadConsole/Ansi/`)
**Files:** `AnsiWriter.cs`, `Document.cs`, `State.cs`  
**Risk:** ANSI escape code interpretation for importing terminal output. Parsing errors produce garbled output. Zero tests.

#### 9. Readers (`SadConsole/Readers/`)
**Files:** `REXPaint.Image.cs` and related, `Playscii.cs`, `TheDraw.cs`  
**Risk:** File format readers for REXPaint, Playscii, and TheDraw. Import regressions silently corrupt loaded content. Zero tests. These are popular external tool integrations.

#### 10. `Algorithms.cs`
**Risk:** Contains geometric/spatial algorithms used for line-drawing, flood-fill, etc. No tests. Algorithmic bugs are high-confidence regressions.

#### 11. `LayeredScreenSurface` / `LayeredSurface` (Component)
**Risk:** Layered rendering is a core composition primitive. Zero tests for layer add/remove/reorder or rendering order.

---

### 🟡 MEDIUM — Limited or Shallow Coverage

#### 12. `Serialization` — Incomplete Object Coverage
**What's tested:** `ScreenObject`, `ScreenSurface`, `AnimatedScreenObject`, `Font` basic round-trips.  
**What's missing:** `ControlsConsole`, `WindowConsole`, `Console`, `LayeredScreenSurface`, entity serialization with components, effects on surfaces, color themes — the `SerializedTypes/` folder has 12 type files with no corresponding restore-validation tests.

#### 13. `ScreenSurface` — Surface Interactions Beyond Basics
**What's tested:** Construction, one resize.  
**What's missing:** View-port scrolling (`ViewPosition`), tint/tearing, `IsDirty` state tracking, font/font-size changes, child surface layering.

#### 14. `ColoredGlyph` / `ColoredGlyphBase` / `CellDecorator`
**What's tested:** Decorators set/add/remove on `CellSurface`. `ColoredGlyph.Matches()` indirectly via serialization.  
**What's missing:** `ColoredGlyphState` save/restore, `ColoredGlyphAndEffect` lifecycle, decorator rendering edge cases.

#### 15. `EasingFunctions` (`Bounce`, `Circle`, `Expo`, `Linear`, `Quad`, `Sine`)
**Risk:** Mathematical functions — easy to introduce floating-point regressions. Pure functions are cheap to test.

#### 16. `UI/Colors` / `ThemeStates` / `AdjustableColor`
**Risk:** Color theme system drives all control appearance. No tests for color derivation, theme inheritance, or predefined palette values.

---

### 🔵 LOW (Host implementations, hard to unit-test in isolation)

#### 17. `SadConsole.Host.*` — MonoGame, SFML, FNA, KNI, MonoGameWPF
**Risk:** Platform-specific rendering hosts. Correct isolation behind interfaces makes these hard to unit-test, but integration smoke tests (boot, render one frame, dispose cleanly) would catch common init-order bugs. Currently zero coverage.

#### 18. `SadConsole.Extended` — Non-Table Components
**Files:** `AnimatedBox`, `Border`, `ColorPicker`, `ColorPickerPopup`, `GlyphSelectPopup`, `MouseDrag`, `SmoothMove`, `FileDirectoryListbox`, `ClassicConsoleKeyboardHandler`, `C64KeyboardHandler`  
**Risk:** Only `Table` is tested from Extended. All other components are untested.

---

## Recommended Next Steps — Top 5

> Ordered by risk reduction per effort invested.

### 1. 🥇 `ColoredString` Parser Test Suite
Create `ColoredString.Parse.cs` tests covering the default parser, BBCode parser, and each `ParseCommand*` type. This protects the most-used visible output mechanism. Expected: ~30–40 test cases, no platform dependencies.

### 2. 🥈 `Cursor` Component Tests
Test print, newline, backspace, auto-wrap, auto-scroll, and `PrintAppearance` carry-over. All can run against a `CellSurface` with the `BasicGameHost`. Expected: ~20 tests.

### 3. 🥉 UI Control Basics: `TextBox`, `ListBox`, `ScrollBar`
Even minimal state-transition tests (focus→type→value, scroll min/max clamping) would catch the most common regressions. Target the three most-used controls first. Expected: ~15–20 tests per control.

### 4. Effects System Behavior Tests
Test `Blink`, `Fade`, and `EffectsManager.Update()` with a mock time-delta. Verify state transitions, serialization round-trips, and that removed effects are cleaned up. Expected: ~15 tests.

### 5. REXPaint / Reader Tests
Load known-good `.xp` and Playscii files from test fixtures and assert expected glyph/color data. These are pure I/O with no platform dependency. Expected: ~5–10 tests with fixture files.

---

*Generated by Rachael (Tester) via automated analysis of `Tests/` and `SadConsole/` source trees.*

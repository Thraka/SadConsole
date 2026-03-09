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

## Core Context

**Terminal parser:** ECMA-48 state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation design, CP437/UTF-8 encoding. **Writer:** Direct cell manipulation, pending-wrap for ANSI art, scroll regions. **State:** Cursor position/attributes (SGR), tab stops. **Palette:** 256-entry RGB, OSC redefinition (X11 formats). **Rendering:** CellDecorator (underline 95, strikethrough 196), color resolution (reverse, dim, bold shifts).

**Phases 0–10 (662 tests):** Parser (87), Writer (160), Insert/Delete (24), Tabs (8), DEC modes (16), Decorators (18), Palette (12), Polish (18).

**PendingWrap architecture:** Opt-in clearing model (cursor-movers clear, SGR/decorators preserve). Root fix for ANSI art drift. CUF/CHT forward-resolution at margins. Bold+default foreground → Palette[15].

**Deckard alignment (2026-03-07):** Architecture design for injectable nullable Cursor, Writer with ITerminalOutput response channel, TerminalConsole subclass pattern. Zero disputes on design direction.

## Implementation History (2026-03-07 to 2026-03-09) — Cursor/Console/Encoder

**Phase 1 (Cursor):** TerminalCursor lightweight data class, CursorShape enum (6 modes), Writer.Cursor nullable injectable. Null-checks added to SyncCursorPosition/DECTCEM/DECSCUSR. 44 tests.

**Phase 2 (TerminalConsole):** Inherits ScreenSurface, wires Writer+TerminalCursor, Feed() delegation, focus toggling, ProcessKeyboard placeholder. GameHost.GetRendererStep() factory pattern. 33 tests.

**Phase 3 (KeyboardEncoder+ITerminalOutput):** KeyboardEncoder standalone encoder (arrows/F1-F12/navigation/modifiers), ITerminalOutput response channel (Write methods). Writer: Output property, DSR/DA1 handlers (null-guarded). TerminalConsole: Output delegation, KeyboardEncoder property, ProcessKeyboard DECCKM sync + encode/feed-back. 92 tests (70 encoder + 22 output).

## 2026-03-09 — Phase 3 Complete: KeyboardEncoder & ITerminalOutput

Delivered bidirectional input/output infrastructure for interactive terminal support.

**Created:**
- `SadConsole/Terminal/KeyboardEncoder.cs` (242 lines) — Converts SadConsole AsciiKey/Keys to ANSI escape sequences. Handles arrows (normal + DECCKM modes), F1-F12 (SS3 + CSI), Home/End/Nav keys, Enter (LNM configurable), Backspace (DEL/BS mode), Tab/Shift+Tab, Escape, Ctrl+letter, Alt+key, xterm modifier combinations.
- `SadConsole/Terminal/ITerminalOutput.cs` (22 lines) — Response channel: `Write(byte[])`, `Write(string)`. Writer.Output nullable (null = data-stream, non-null = interactive).

**Modified:**
- `SadConsole/Terminal/Writer.cs` — `ITerminalOutput? Output` property. DSR (CSI n): Ps 5 → `\x1b[0n` (OK), Ps 6 → `\x1b[row;colR` (position). DA1 (CSI c): Ps 0 → `\x1b[?62;1;2;6;7;8;9c` (VT220). All null-guarded.
- `SadConsole/TerminalConsole.cs` — KeyboardEncoder property, Output property delegation, ProcessKeyboard syncs DECCKM → encoder.ApplicationCursorKeys, encodes and feeds back.

**Key patterns:**
- KeyboardEncoder standalone (no Writer dep) — usable for remote terminals.
- DECCKM push-model (ProcessKeyboard syncs state to encoder).
- SS3 for F1-F4/application-mode arrows; CSI for F5-F12/navigation.
- xterm modifier: base 1 + Shift(1) + Alt(2) + Ctrl(4).
- VT220 identification scope aligns with Writer's ECMA-48 feature set.

**Build:** 0 errors, 48 warnings (all pre-existing). **Tests:** 759/759 baseline pass, zero regressions.

## Learnings

### Phase 3 — KeyboardEncoder, ITerminalOutput, Writer Response Channel (2026)

**Architecture patterns (archived for reference):**
- KeyboardEncoder is standalone (no Writer dependency) — can be used independently for remote terminal scenarios.
- DECCKM sync happens in ProcessKeyboard (push from Writer.State → encoder) rather than the encoder polling state.
- F1-F4 use SS3 (`ESC O`) in normal mode; F5-F12 use CSI tilde sequences.
- Arrow/Home/End in application mode use `ESC O` prefix; in normal mode `ESC [`.
- xterm modifier encoding: base 1 + Shift(+1) + Alt(+2) + Ctrl(+4).
- DA1 response identifies as VT220 — matches the level of escape sequence support the Writer implements.


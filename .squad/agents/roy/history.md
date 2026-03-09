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

### SadBBSClient Sample — Telnet BBS Client (2026)

**Architecture patterns:**
- **Option A for remote keyboard:** Set `TerminalConsole.UseKeyboard = false` and handle keyboard at the container (BbsScreen) level using `KeyboardEncoder.Encode()` directly. Sends encoded bytes to TelnetClient instead of local Writer.Feed(). Cleanest approach — no subclassing needed.
- **Thread safety:** TelnetClient read loop runs on background thread. Used `ConcurrentQueue<byte[]>` to buffer incoming data, drained in `BbsScreen.Update()` on the game thread before calling `TerminalConsole.Feed()`.
- **ITerminalOutput dual use:** TelnetClient implements ITerminalOutput, wired as `TerminalConsole.Output`. This means both DA/DSR responses from Writer AND user keystrokes go through the same socket — correct for BBS interaction.
- **Telnet IAC in-band:** Processed IAC sequences inline during read, stripping them from data before enqueueing. WILL/WONT/DO/DONT handled with accept/reject logic. NAWS sends terminal dimensions, TTYPE identifies as "ANSI".
- **Writer has no Reset():** Use `\x1b[2J\x1b[H` (ED + CUP) to clear screen via escape sequences.
- **SadConsole startup:** Use `SetWindowSizeInCells()` (not deprecated `SetScreenSize()`). Standard Builder flow: `Game.Create(startup)` / `Game.Instance.Run()` / `Game.Instance.Dispose()`.

**Key files:**
- `Samples/SadBBSClient/Program.cs` — SadConsole Builder startup, 80×25
- `Samples/SadBBSClient/TelnetClient.cs` — TCP/telnet client, ITerminalOutput impl, IAC negotiation
- `Samples/SadBBSClient/BbsScreen.cs` — ScreenObject container, keyboard→telnet routing, connection UI

### Phase 3 — KeyboardEncoder, ITerminalOutput, Writer Response Channel (2026)

**Architecture patterns (archived for reference):**
- KeyboardEncoder is standalone (no Writer dependency) — can be used independently for remote terminal scenarios.
- DECCKM sync happens in ProcessKeyboard (push from Writer.State → encoder) rather than the encoder polling state.
- F1-F4 use SS3 (`ESC O`) in normal mode; F5-F12 use CSI tilde sequences.
- Arrow/Home/End in application mode use `ESC O` prefix; in normal mode `ESC [`.
- xterm modifier encoding: base 1 + Shift(+1) + Alt(+2) + Ctrl(+4).
- DA1 response identifies as VT220 — matches the level of escape sequence support the Writer implements.

### SadBBSClient Validation — Remote Terminal Use Case (2026-03-09)

Real-world sample demonstrating that KeyboardEncoder + ITerminalOutput architecture works for remote terminal clients.

**Sample structure (Samples/SadBBSClient/, 741L, 6 files):**
- `TelnetClient.cs` (355L) — TCP socket, IAC negotiation (NAWS/TTYPE/SGA/ECHO), background read thread with ConcurrentQueue buffering
- `BbsScreen.cs` (288L) — ScreenObject container, keyboard→telnet routing, preset BBS connection menu UI
- `Program.cs` (20L) — SadConsole Builder startup with SetWindowSizeInCells(80, 25)

**Key findings:**
- Option A keyboard architecture proved correct: `TerminalConsole.UseKeyboard = false`, handle keyboard at BbsScreen level using `KeyboardEncoder.Encode()` directly, route encoded bytes to TelnetClient instead of Writer.Feed()
- ConcurrentQueue + game-thread drain pattern validates thread-safe async socket handling
- ITerminalOutput dual use (both keystroke echoes + Writer response data through same socket) works as designed
- Telnet IAC negotiation (in-band, processed during read, stripped before enqueue) seamless integration
- No Writer.Reset() method — use escape sequences `\x1b[2J\x1b[H` (ED + CUP) to clear screen

**Validation:** Architecture patterns from Phase 1-3 confirmed sound for BBS/SSH/serial clients. Committed as d0ee4c50.


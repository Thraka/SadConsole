# Roy — History Archive

Archived verbose learnings and implementation details from main history.md to reduce file size while preserving learnings.

## Learnings — SadBBSClient Sample (2026)

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

## Implementation Phase 3 Details — KeyboardEncoder, ITerminalOutput, Writer (2026)

**Architecture patterns:**
- KeyboardEncoder is standalone (no Writer dependency) — can be used independently for remote terminal scenarios.
- DECCKM sync happens in ProcessKeyboard (push from Writer.State → encoder) rather than the encoder polling state.
- F1-F4 use SS3 (`ESC O`) in normal mode; F5-F12 use CSI tilde sequences.
- Arrow/Home/End in application mode use `ESC O` prefix; in normal mode `ESC [`.
- xterm modifier encoding: base 1 + Shift(+1) + Alt(+2) + Ctrl(+4).
- DA1 response identifies as VT220 — matches the level of escape sequence support the Writer implements.

**Files created:**
- `SadConsole/Terminal/KeyboardEncoder.cs` (242 lines) — Converts SadConsole AsciiKey/Keys to ANSI escape sequences. Handles arrows (normal + DECCKM modes), F1-F12 (SS3 + CSI), Home/End/Nav keys, Enter (LNM configurable), Backspace (DEL/BS mode), Tab/Shift+Tab, Escape, Ctrl+letter, Alt+key, xterm modifier combinations.
- `SadConsole/Terminal/ITerminalOutput.cs` (22 lines) — Response channel: `Write(byte[])`, `Write(string)`. Writer.Output nullable (null = data-stream, non-null = interactive).

**Modified:**
- `SadConsole/Terminal/Writer.cs` — `ITerminalOutput? Output` property. DSR (CSI n): Ps 5 → `\x1b[0n` (OK), Ps 6 → `\x1b[row;colR` (position). DA1 (CSI c): Ps 0 → `\x1b[?62;1;2;6;7;8;9c` (VT220). All null-guarded.
- `SadConsole/TerminalConsole.cs` — KeyboardEncoder property, Output property delegation, ProcessKeyboard syncs DECCKM → encoder.ApplicationCursorKeys, encodes and feeds back.

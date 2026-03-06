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

**Key Architecture Patterns:**
- **Surfaces:** `CellSurface` is pure data (no rendering); `ScreenSurface` wraps it with rendering; effects system is mutation-aware via dirty flags
- **Fonts:** IFont is core metadata; hosts own GPU textures. Extended fonts via GlyphDefinition + CellDecorator. Pre-computed glyph rects, registered in GameHost.Fonts
- **RowFontSurface:** Per-row font support via sparse Dictionary; pixel-to-cell lookup via Y offset caching
- **Terminal system:** Standalone under `SadConsole/Terminal/`. Parser (state machine), Writer (renders to ICellSurface), Measurer (dimensions only). Handler callback pattern, zero allocation, CP437/UTF-8 encoding support

**Key Learnings:**
- Font system is sound across all hosts (no cross-host issues)
- Parser contracts defined via test-first (87 tests); Writer contracts via integration tests (160+ tests)
- Encoding matters: Parser.Encoding (byte→char), Writer.Encoding (char→glyph)
- LF defaults to implicit (CR+LF) — standard terminal behavior
- SGR 0 = full reset, Bold shifts palettes 0-7 only, Dim halves RGB
- Auto-wrap defers until next printable character (pending-wrap state critical for ANSI art)

## Core Context (Continued)

**Terminal System Architecture:**
- **Parser:** ECMA-48 state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation via preallocated span arrays, CP437/UTF-8 encoding modes
- **Writer:** Direct ICellSurface cell manipulation (glyph, foreground, background), ColorMode enum (Default/Palette/TrueColor), pending-wrap state for ANSI art, scroll regions with origin mode support
- **State:** Cursor position/attributes (bold/dim/reverse/underline/strikethrough/italic/blink), SGR tracking, tab stops (SortedSet for O(1) lookup)
- **Palette:** 256-entry RGB array; OSC 4/10/11 redefinition support (X11 rgb:r/g/b and #rrggbb formats)
- **Rendering:** CellDecorator for visual attributes (underline glyph 95, strikethrough 196), font-defined glyph override, color resolution (reverse video swaps fg/bg, dim halves RGB, bold shifts palette 0-7 to 8-15)
- **Phases delivered:** 0 (parser, 87 tests), 1 (writer, 160 tests), 5 (insert/delete, 24 tests), 6 (tabs, 8 tests), 8 (DEC modes, 16 tests), 3 (decorators, 18 tests), 9 (palette, 12 tests), 10 (polish, 18 tests) = 662/662 tests total

## Learnings — Terminal Phases 5, 6, 8 (2025-07-14)

### Phase 5 — Insert/Delete/Scroll Operations
- **ICH/DCH operate on current row only** — Insert shifts right, delete shifts left. Clamped to `width - col` to prevent over-shifting. Blanks use current SGR background.
- **IL/DL operate within scroll region** — Guard: cursor must be within `ScrollTop..ScrollBottom`. Insert shifts down, delete shifts up. Lines pushed past region boundaries are lost.
- **SU/SD reuse existing ScrollUp/ScrollDown** — No new scroll logic needed; the existing methods already handle scroll region bounds correctly.
- **ECH is erase-in-place** — Unlike DCH, does not shift. Overwrites N cells at cursor with blanks. Uses current background color (same as ED/EL convention).
- **REP needs last-printed-char tracking** — Added `_lastPrintedChar` field to Writer. Updated in `OnPrint`. REP calls `OnPrint` in a loop to get full SGR/wrapping behavior for free.

### Phase 6 — Tab Stop Commands
- **CHT/CBT iterate tab stops** — Forward tabulation loops `NextTabStop` N times; backward tabulation loops `PreviousTabStop` N times.
- **PreviousTabStop walks SortedSet forward** — Iterates the sorted set and tracks the last stop before `currentColumn`. Returns 0 if no prior stop exists. Simple and correct since SortedSet is already ordered.
- **TBC supports mode 0 (clear at column) and mode 3 (clear all)** — Other TBC modes are nonstandard and ignored.

### Phase 8 — DEC Private Modes + Scroll Margins
- **Private prefix routing redesign** — Removed blanket `if (privatePrefix is not null) return;` guard. Now routes `?` to `HandleDecPrivateMode`, ignores other unknown prefixes. Clean separation.
- **DECSTBM (CSI r) is NOT a private sequence** — Common misconception. It's a regular CSI sequence with no `?` prefix. Added to the main switch as `case 'r':`.
- **Origin mode affects CUP only** — When DECOM is set, `HandleCursorPosition` adds `ScrollTop` to the row parameter and clamps within the scroll region. Cursor movement (CUU/CUD) already respects `ScrollTop`/`ScrollBottom` naturally.
- **DECOM set/reset homes cursor** — Setting origin mode homes to `(ScrollTop, 0)`. Resetting homes to `(0, 0)`. Same behavior after DECSTBM.
- **SavedCursorState includes OriginMode** — Per DEC spec, DECSC/DECRC save and restore origin mode along with position and SGR attributes.
- **DECTCEM wires directly to Cursor.IsVisible** — Mode 25 set/reset updates both `State.CursorVisible` and the visual `Cursor.IsVisible` property immediately.
- **CursorKeyMode and ScreenReverseVideo are state-only** — Tracked in State for consumer queries, but no Writer rendering behavior needed yet.
- **Build:** 0 errors, 48 pre-existing warnings. **Tests:** 556/556 pass on net8.0 (no regressions).

## Learnings — Terminal Phases 3, 9, 10 (2025-07-14)

### Phase 3 — Visual SGR Rendering via Cell Decorators
- **CellDecorator is a readonly struct** — `CellDecorator(Color color, int glyph, Mirror mirror)`. Attached to cells via `ColoredGlyphBase.Decorators` (nullable `List<CellDecorator>?`).
- **Underline glyph index 95, strikethrough 196** — Same convention as BBCode parser and Label.Theme. Font-defined decorators take priority via `IFont.HasGlyphDefinition("underline")` / `GetDecorator()`.
- **ApplyDecorators uses resolved fg color** — The decorator color matches the cell's foreground (post reverse-video, post-dim). Consistent with how users expect underline/strikethrough to look.
- **Italic and blink are tracked but not rendered** — SadConsole fonts are tile-based (no true italic). Blink needs timer/component integration (tracked as TODO). Both attributes are correctly stored in State for future use.
- **Reverse video was already implemented** — `ResolveColors()` swaps fg/bg when `State.Reverse` is true. No additional work needed.
- **CopyCell and ClearCell updated for decorators** — CopyCell deep-copies the decorator list. ClearCell sets decorators to null. Prevents shared-reference bugs across cells.

### Phase 9 — OSC Palette Redefinition + DCS
- **OSC payload is raw bytes** — Command number is parsed from digits before first `;`. Data follows after the separator.
- **OSC 4 supports multi-entry format** — `4;idx1;color1;idx2;color2` sets multiple palette entries in one sequence. Parser loops through `{index};{color}` pairs delimited by `;`.
- **X11 color spec scaling** — `rgb:rr/gg/bb` components are 1–4 hex digits. 1-digit scales ×17, 2-digit as-is, 3-digit >>4, 4-digit >>8. Also supports `#rrggbb` format.
- **OSC 10/11 update State.DefaultForeground/DefaultBackground** — These affect cells printed with `ColorMode.Default`.
- **DCS is a stub** — Font loading is future work per decisions.md.

### Phase 10 — Polish
- **ED audit: all modes correct** — ED 0/1/2/3 verified. ED 3 (scrollback) aliases to ED 2 as documented.
- **CSI s disambiguated** — `parameters.Length == 0` → Save Cursor; otherwise ignore (DECSLRM not supported).
- **PendingWrap clearing audit** — Added to DEC private mode path (was skipping line 318 via early return), NEL, and RI. C0 controls and normal CSI dispatch already cleared correctly.
- **VPA ('d') implemented** — Line Position Absolute, respects origin mode (like CUP).
- **Test fix: Ed1 off-by-one** — Pre-written test expected 'Q' at col 5 but correct value is 'P' (CUP 1-based col 5 → 0-based col 4, ED 1 erases through col 4, col 5 retains 'P').
- **TODO comments added** — CSI c (DA), CSI h/l (SM/RM standard modes), CSI t (window), CSI !p (DECSTR) documented as intentionally unhandled.
- **Build:** 0 errors, 48 pre-existing warnings. **Tests:** 662/662 pass on net8.0 (no regressions).

## Cross-Agent Update — 2026-03-06

**Milestone achieved:** All 10 Terminal phases complete. 662/662 tests pass. Zero regressions.

- **Rachael:** Verified Phases 3/9/10 test contracts in `TerminalWriterPhase3Tests.cs` (48 tests) — all pass after your implementation
- **Test assertion fix:** Pre-written `Ed1_EraseStartToCursor_ClearsFromStartToCursor` had off-by-one; corrected from 'Q' to 'P' at col 5
- **Next:** Team ready for next work. Terminal overhaul complete.
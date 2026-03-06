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

### Terminal Learnings (Phases 5–10 Summary, 2025-07-14 to 2026-03-06)
**Phase 5:** ICH/DCH on current row only; IL/DL within scroll region; SU/SD reuse existing scroll; ECH erase-in-place; REP tracks `_lastPrintedChar`.  
**Phase 6:** CHT/CBT loop tab stops (SortedSet for O(1)); TBC modes 0 (at column) and 3 (all).  
**Phase 8:** Private prefix routing via `?` → `HandleDecPrivateMode`; DECSTBM is regular CSI (not private); origin mode affects CUP only; DECOM homes cursor; SavedCursorState includes OriginMode; DECTCEM wires to Cursor.IsVisible; CursorKeyMode/ScreenReverseVideo state-only.  
**Phase 3:** CellDecorator readonly struct; underline glyph 95, strikethrough 196; ApplyDecorators uses resolved fg; italic/blink tracked (no render yet); reverse video already implemented; CopyCell deep-copies decorators, ClearCell nulls them.  
**Phase 9:** OSC payload raw bytes; OSC 4 supports multi-entry; X11 color scaling (1–4 hex digits, 1-digit ×17); OSC 10/11 update defaults; DCS stub.  
**Phase 10:** ED modes 0/1/2/3 verified; CSI s Save when len==0; PendingWrap added to DEC path/NEL/RI; VPA ('d') respects origin mode; Test fix: Ed1 off-by-one ('P' at col 5, not 'Q'). **Build:** 0 errors. **Tests:** 662/662 pass, zero regressions.

## Learnings — PendingWrap Opt-In Clearing Model (2025-07-16)

### Architecture Decision: Inverted PendingWrap Responsibility
- **Old model (opt-out):** Blanket `State.PendingWrap = false` at end of `OnCsiDispatch` (line 342) and after DEC private mode dispatch (line 224). Every CSI sequence cleared PendingWrap; exceptions had to be carved out.
- **New model (opt-in):** No blanket clear. Each cursor-moving handler is responsible for clearing PendingWrap itself. Non-cursor-moving sequences (SGR, erase, ECH, REP, etc.) preserve PendingWrap by default.
- **Root cause of b5-ans01.ans drift:** 85 SGR sequences hit while PendingWrap was true, causing 84 characters placed at wrong positions.

### Classification Applied
- **Clear PendingWrap:** CUP/HVP (H/f), CUU (A), CUD (B), CUF (C), CUB (D), CNL (E), CPL (F), CHA (G), VPA (d), CHT (I), CBT (Z), ICH (@), DCH (P), IL (L), DL (M), DECSTBM (r), DECOM (mode 6 in DEC private)
- **Preserve PendingWrap:** SGR (m), ED (J), EL (K), ECH (X), REP (b), SU (S), SD (T), TBC (g), CSI s (save), DSR (n), DECTCEM (25), DECAWM (7), DECCKM (1), DECSCNM (5)
- **Self-managing:** CSI u (restore) — `State.RestoreCursor()` clears internally. REP (b) — calls `OnPrint` which manages wrap itself.
- **OnEscDispatch verified:** NEL (E) and RI (M) clear. RIS (c) clears via `State.Reset()`. HTS (H) and DECSC (7) don't clear. DECRC (8) clears via `RestoreCursor()`.
- **OnOscDispatch:** Pure data/palette ops — no PendingWrap interaction needed.

### Key Pattern
- When a dispatcher epilogue does "clean up everything," it's an opt-out model. Bugs are created by passive omission (forgetting to exempt a case). Inverting to opt-in makes the *safe default* correct for the majority of cases, and only the minority must take affirmative action.
- **Tests:** 662/662 pass with zero regressions after the change.

## Cross-Agent Update — 2026-03-06 (PendingWrap Batch Complete)

**Milestone:** PendingWrap opt-in clearing model fully implemented and tested.

- **Rachael:** Wrote 8 regression tests (670 total pass):
  1. SGR-at-boundary preservation (the critical regression)
  2. CUF cursor-move clearing
  3. CUP cursor-move clearing
  4. DECTCEM preservation
  5. DECAWM preservation
  6. ECH preservation
  7. Multiple SGR chaining + wrapping
  8. Integration test with real b5-ans01.ans file (end-to-end validation)
  
- **Implementation outcome:** Removed 2 blanket `State.PendingWrap = false` epilogues, added explicit clears to 17 cursor-moving handlers + DECOM. Zero regressions.
- **Bug fixed:** b5-ans01.ans now renders correctly — no progressive line drift.
- **Specification compliance:** ECMA-48 §7.1 strict adherence via opt-in architecture.

## Learnings — CUF PendingWrap Resolution (2025-07-17)

### Bug: CUF at Right Margin During PendingWrap
- **Symptom:** In b5-ans01.ans, the line after "Messages" text was indented wrong (shorter). Characters rendered at col 79 instead of wrapping.
- **Root cause:** `CSI 6C` (CUF 6) arrived while `PendingWrap = true` at col 79. Writer cleared PendingWrap and tried `Math.Min(79, 79+6) = 79` — effectively a no-op. The cursor stayed stuck at col 79 on row 13 instead of moving to col 6 on row 14.
- **Fix:** CUF now resolves the pending wrap when `PendingWrap && AutoWrap`: advances to col 0 of next row (via `LineFeed()`), then applies the forward movement. This matches ANSI.SYS immediate-wrap semantics used by BBS art.
- **Scope:** Only CUF (`C`) gets this resolve behavior. CUB/CUU/CUD/CUP/CHA/VPA continue to just clear the flag (absolute/backward/vertical moves don't need wrap resolution).
- **Key insight:** CUF from the right margin with PendingWrap is always a no-op without resolution. The only useful behavior is to resolve the wrap first, making this fix strictly correct for all terminal styles.
- **Tests:** 673/673 pass (3 new regression tests added). Zero regressions on existing 670.
- **Specification compliance:** ECMA-48 §7.1 strict adherence via opt-in architecture.

## Learnings — PendingWrap Comprehensive Audit (2025-07-17)

### Full Audit of All Cursor-Movement Handlers

Thraka requested audit of every cursor-movement handler for the same "stuck at right margin" bug found in CUF. The bug pattern: PendingWrap=true at col width-1, handler clears flag but forward movement clamps to width-1 → no-op.

### Categorization

**✅ SAFE — Absolute positioning (position set regardless of current col):**
- CUP/HVP (H/f) — absolute row+col
- CHA (G) — absolute column
- VPA (d) — absolute row
- DECSTBM (r) — homes cursor
- DECOM (mode 6) — homes cursor
- CSI u (restore) — restores saved position
- NEL (ESC E) — explicit col 0 + linefeed
- RI (ESC M) — vertical reverse index

**✅ SAFE — Movement is meaningful from right margin (not forward-clamped):**
- CUU (A) — moves up, cursor moves to different row
- CUD (B) — moves down, cursor moves to different row
- CUB (D) — moves backward, cursor moves to different column
- CNL (E) — moves down + col 0, cursor moves to different row and column
- CPL (F) — moves up + col 0, cursor moves to different row and column
- CBT (Z) — backward tab, PreviousTabStop(79) returns previous stop
- ICH (@), DCH (P), IL (L), DL (M) — edit-in-place, no forward movement

**⚠️ AT RISK — Fixed in this audit (same stuck-at-margin bug as CUF):**
- CHT (CSI I) — forward tab: NextTabStop(width-1) returns width-1 → no-op. Fixed: resolve wrap first.
- C0 HT (0x09) — tab character: same NextTabStop clamping. Fixed: resolve wrap first.

### Key Insight: The Bug Pattern Requires "Forward + Clamp"
Only forward-moving handlers that clamp at the right margin exhibit this bug. Backward/vertical/absolute handlers can't get stuck because their target position is different from the margin. This narrows the at-risk set to: CUF (already fixed), CHT, and C0 HT (both fixed here).

### Implementation
- CHT: Added PendingWrap resolution block (same pattern as CUF) in CSI dispatcher
- C0 HT: Extracted from blanket PendingWrap=false in OnC0Control, added early-return path with wrap resolution
- **Tests:** 679/679 pass (6 new tests added). Zero regressions on existing 673.
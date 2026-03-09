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

## Archived Work — Terminal Implementation & PendingWrap Architecture (2026-02-24 to 2026-03-06)

**Archived terminal system implementation (all 662/682 tests passing, zero regressions):**

**Terminal architecture (core patterns):** Standalone Parser, Writer, Measurer under `SadConsole/Terminal/`. Parser is ECMA-48 state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation via preallocated span arrays, CP437/UTF-8 encoding modes. Writer: direct ICellSurface cell manipulation, ColorMode enum (Default/Palette/TrueColor), pending-wrap state for ANSI art, scroll regions with origin mode support. State tracks cursor position/attributes (bold/dim/reverse/underline/strikethrough/italic/blink), SGR tracking, tab stops (SortedSet for O(1) lookup). Palette: 256-entry RGB array; OSC 4/10/11 redefinition (X11 rgb:r/g/b and #rrggbb formats). Rendering: CellDecorator for visual attributes (underline glyph 95, strikethrough 196), font-defined glyph override, color resolution (reverse swaps fg/bg, dim halves RGB, bold shifts palette 0-7 to 8-15). Phases delivered: 0 (parser, 87 tests), 1 (writer, 160 tests), 5 (insert/delete, 24 tests), 6 (tabs, 8 tests), 8 (DEC modes, 16 tests), 3 (decorators, 18 tests), 9 (palette, 12 tests), 10 (polish, 18 tests) = 662/662 tests total.

**Phase learnings:** Phase 5: ICH/DCH on current row only; IL/DL within scroll region; SU/SD reuse existing scroll; ECH erase-in-place; REP tracks `_lastPrintedChar`. Phase 6: CHT/CBT loop tab stops (SortedSet for O(1)); TBC modes 0 (at column) and 3 (all). Phase 8: Private prefix routing via `?` → `HandleDecPrivateMode`; DECSTBM is regular CSI (not private); origin mode affects CUP only; DECOM homes cursor; SavedCursorState includes OriginMode; DECTCEM wires to Cursor.IsVisible; CursorKeyMode/ScreenReverseVideo state-only. Phase 3: CellDecorator readonly struct; underline glyph 95, strikethrough 196; ApplyDecorators uses resolved fg; italic/blink tracked (no render yet); reverse video already implemented; CopyCell deep-copies decorators, ClearCell nulls them. Phase 9: OSC payload raw bytes; OSC 4 supports multi-entry; X11 color scaling (1–4 hex digits, 1-digit ×17); OSC 10/11 update defaults; DCS stub. Phase 10: ED modes 0/1/2/3 verified; CSI s Save when len==0; PendingWrap added to DEC path/NEL/RI; VPA ('d') respects origin mode; Test fix: Ed1 off-by-one ('P' at col 5, not 'Q'). Build: 0 errors. Tests: 662/662 pass, zero regressions.

**PendingWrap opt-in clearing model (2026-03-04):** Inverted responsibility from opt-out (blanket clear epilogues) to opt-in (each handler responsible). Old model: `State.PendingWrap = false` at end of OnCsiDispatch (line 342) and DEC dispatch (line 224). New model: no blanket clear; cursor-moving handlers clear explicitly. Root cause of b5-ans01.ans drift: 85 SGR sequences hit while PendingWrap was true, causing 84 characters placed at wrong positions. Classification: Clear PendingWrap (CUP/HVP/CUU/CUD/CUF/CUB/CNL/CPL/CHA/VPA/CHT/CBT/ICH/DCH/IL/DL/DECSTBM/DECOM), Preserve (SGR/ED/EL/ECH/REP/SU/SD/TBC/CSI s/DSR/DECTCEM/DECAWM/DECCKM/DECSCNM), Self-managing (CSI u, REP). OnEscDispatch: NEL/RI clear; RIS clears via State.Reset(); HTS/DECSC don't clear; DECRC clears via RestoreCursor(). OnOscDispatch: pure data/palette ops — no PendingWrap interaction. Key insight: opt-in makes safe default (preserve) correct for majority; only minority (cursor-movers) take affirmative action.

**CUF pending-wrap resolution bug (2026-03-06):** At right margin with PendingWrap=true, CSI 6C (CUF 6) cleared PendingWrap but clamped to Math.Min(79, 79+6) = 79 — no-op, cursor stuck. Fix: CUF resolves wrap when `PendingWrap && AutoWrap`: LineFeed to col 0 of next row, then apply forward movement. Only CUF gets resolve behavior; CUB/CUU/CUD/CUP/CHA/VPA just clear (absolute/backward/vertical don't need wrap resolution). Key: CUF from right margin with PendingWrap is always a no-op without resolution — fix strictly correct. Tests: 673/673 pass (3 new). Zero regressions.

**CHT/C0 HT forward-tab pending-wrap bugs (2026-03-06):** Comprehensive audit of all cursor-movement handlers for stuck-at-margin pattern. Safe: absolute positioning (CUP/HVP/CHA/VPA/DECSTBM/DECOM/CSI u/NEL/RI) — position set regardless of current col. Safe: backward/vertical (CUU/CUD/CUB/CNL/CPL/CBT/ICH/DCH/IL/DL) — meaningful from right margin, not forward-clamped. At risk & fixed: CHT (forward tab clamps at width-1 → no-op), C0 HT (same NextTabStop clamping). Pattern requires "forward + clamp". Fix: resolve wrap first (same as CUF). Tests: 679/679 pass (6 new: CHT wrap resolution, auto-wrap off, C0 HT wrap resolution, tab stop scenarios, multi-tab sequences, CHT/C0 HT consistency). Zero regressions. ECMA-48 §7.1 strict adherence.

**Bold + default foreground bug (2026-03-06):** After "Your stats" in b5-ans01.ans, line started gray (170,170,170) not bright white (255,255,255). `ResolveForeground()` only applied bold brightening (palette 0-7 → 8-15) when `ForegroundMode == Palette`. With `ForegroundMode == Default` (after SGR 0 reset) and bold, returned `State.DefaultForeground` unchanged. Sequence: `ESC[0m` (reset) → `ESC[1;46m` (bold + bg cyan) — no explicit foreground, ForegroundMode stays Default, bold ignored in Default path. Fix: in `ResolveForeground()`, Default case now checks `State.Bold` and returns `Palette.GetColor(15)` (bright white) when bold active. CGA convention: default foreground IS palette index 7; bold shifts it to palette 15. Tests: 682/682 pass (3 new: bold+default resolves to palette 15, bold+default+background combination, sequences like ESC[0m ESC[1;46m render bright white on cyan). Zero regressions on 679. b5-ans01.ans validates correct behavior.

## Learnings

**Terminal Cursor Architecture (2026-03-07):**

**Dual cursor state model:**
- **State.CursorRow/CursorColumn:** Pure tracking integers, source of truth for terminal emulator logic. Updated by all cursor-moving handlers. Zero dependency on Components.Cursor. Persisted via DECSC/DECRC.
- **Components.Cursor.Position:** Visual echo for rendering the blinking cursor. ONE-WAY sync from State → Cursor via `Writer.SyncCursorPosition()`. Writer never reads Cursor.Position. Cursor is write-only from Writer's perspective.
- **Synchronization point:** `SyncCursorPosition()` (Writer.cs:490) — called after every cursor-moving operation. If Cursor.Position is changed externally (e.g., mouse click), Writer never sees it — State position is unaffected. This is by design: Writer owns terminal state, Cursor is purely for display.

**Writer's relationship to Cursor:**
- Writer uses Cursor for **visual display sync only** — updates Position and IsVisible (via DECTCEM CSI ? 25 h/l)
- Cursor is a **passive output device** — never read by Writer, never participates in parsing/writing/scrolling
- Cursor owns Print() methods for interactive text entry — but Writer never calls them
- Writer creates Cursor internally (constructor line 91-96) and enforces `AutomaticallyShiftRowsUp = false` because Writer handles scrolling via State.ScrollUp()

**Surface is already external:**
- ICellSurface passed to Writer constructor (externally provided)
- Writer uses it for: direct cell access `[col, row]`, dimensions (Width/Height), dirty flag, optional resize (ICellSurfaceResize for auto-grow)
- Cursor, however, is internally created — unlike surface

**Making Cursor external is low-risk:**
- Change constructor: `Writer(ICellSurface surface, IFont font, Components.Cursor? cursor = null)`
- Add null checks in SyncCursorPosition() and DECTCEM handler
- Enables headless mode (data-stream ANSI art rendering without visual cursor)
- Enables cursor sharing, custom cursor subclasses
- Risk: surface mismatch (Cursor wired to different surface than Writer's target), initialization responsibility (caller must set AutomaticallyShiftRowsUp = false)

**Two Writer modes: data-stream vs interactive:**
- **Data-stream (ANSI art browser):** Static art rendering, no user input, cursor irrelevant (art already contains final state), instant or throttled playback via Reader
- **Interactive (terminal emulator/BBS):** Bi-directional communication, keyboard/mouse input, cursor critical for user feedback, DSR responses, needs output channel
- Current Writer is data-stream-ready with optional cursor. Interactive layer needs: input handling (Keys → terminal sequences), output channel (DSR/query responses), state query API
- Recommendation: Single Writer with optional cursor + separate InteractiveTerminal class that composes Writer and adds I/O

**TerminalConsole design (subclass Console):**
- Subclass Console (inherits surface, cursor, focus, component lifecycle, rendering)
- Wire Writer to Console.Surface and Console.Cursor (pass Cursor to Writer constructor when external)
- Override ProcessKeyboard to convert keystrokes to terminal sequences
- Add bi-directional API: `Feed(string)` (expose Writer.Feed), `OnSendData` event (upstream output for DSR/keystrokes)
- Pattern minimizes duplication — all console infrastructure reused, override points well-defined

**Reader relationship:**
- Reader is pure byte pump (Stream → Writer.Feed), throttles via BytesPerSecond, extends InstructionBase (IComponent)
- Reader does NOT parse ANSI, knows nothing about surfaces/cursors/State — it's transport layer only
- Reader is optional — can call Writer.Feed(data) directly without Reader
- Reader + Writer = animation-over-time playback; without Reader = instant rendering (tests, static art)

## Cross-Agent Updates

### 2026-03-07 — Deckard's Architecture Analysis: Terminal Writer Restructuring

Deckard completed architecture decision analysis covering the same three questions. Results merged to decisions.md.

**Deckard's Findings:**
- **Architecture-level design** for injectable nullable Cursor, single Writer with ITerminalOutput response channel, TerminalConsole subclass pattern
- **Backwards compatibility assessment:** Minor breaking change for Cursor nullability; migration path via convenience constructor
- **Final class hierarchy:** ScreenObject → ScreenSurface → Console → Terminal.TerminalConsole
- **Open questions for Thraka:** Breaking change tolerance, KeyboardEncoder scope, TerminalConsole namespace preference
- **Key insight:** When two subsystems track shared concept (cursor position), designate one as authoritative. Push model (authority → display) is clearer than pull or bidirectional.

**Alignment:** Complete technical alignment — Deckard's recommendations match Roy's phased implementation approach. Zero disputes on all three questions.

**Next:** Awaiting Thraka design decisions before Phase 1 implementation.

### 2026-03-07 — Phase 1 Implementation: TerminalCursor, CursorShape, Writer Changes

**Completed:** Terminal cursor architecture redesign Phase 1 — core library implementation.

**Created files:**
- `SadConsole/Terminal/CursorShape.cs` — enum mapping DECSCUSR parameter values (1=BlinkingBlock, 2=SteadyBlock, 3=BlinkingUnderline, 4=SteadyUnderline, 5=BlinkingBar, 6=SteadyBar)
- `SadConsole/Terminal/TerminalCursor.cs` — lightweight data class (NOT IComponent), properties: Position, IsVisible, Shape, CursorRenderCellActiveState (ColoredGlyphBase for render step)

**Modified Writer.cs:**
- `Cursor` property changed from `Components.Cursor` (non-nullable) to `TerminalCursor?` (nullable)
- Constructor signature unchanged: `Writer(ICellSurface surface, IFont font)` — NO cursor creation, caller sets via property
- `SyncCursorPosition()` now null-checks before setting Position
- DECTCEM handler (CSI ? 25 h/l) null-checks before setting IsVisible
- Added DECSCUSR handler (CSI Ps SP q) for cursor shape control — maps parameter to CursorShape enum, calls UpdateGlyphForShape()
- Removed internal Cursor instantiation

**TerminalCursor design:**
- Parameterless constructor defaults to white on black
- Exposes `CursorRenderCellActiveState` (ColoredGlyphBase) — render steps read Glyph, Foreground, Background
- `UpdateGlyphForShape()` maps: Block→219 (█), Underline→95 (_), Bar→124 (|)
- SetForeground/SetBackground helpers for color control
- Pattern matches Components.Cursor's CursorRenderCellActiveState interface — zero render step changes needed

**Test fixes:**
- TerminalCursorTests.cs: 44 tests, all pass (cursor defaults, shape values, null-cursor mode, DECTCEM/DECSCUSR integration)
- TerminalWriterPhase2Tests.cs: Updated Setup() to create TerminalCursor for visibility tests
- Fixed 2 test expectations (cursor position after print was off by 1 — tests had bugs, not implementation)

**Build/Test results:**
- SadConsole.csproj: Build succeeded, 0 errors
- All 990 terminal tests pass (net8.0, net9.0, net10.0)

**Key pattern:** Render step compatibility maintained — TerminalCursor.CursorRenderCellActiveState matches Components.Cursor's interface (Glyph, Foreground, Background properties). Gaff's host render steps can read either type without changes.
## 2026-03-09 — Terminal Cursor Phase 1 Complete

Delivered core implementation of TerminalCursor architecture per Thraka's final directives:

**Created:**
- SadConsole/Terminal/CursorShape.cs — 6-value enum mapping DECSCUSR parameters
- SadConsole/Terminal/TerminalCursor.cs — data class with Position, IsVisible, Shape, CursorRenderCellActiveState

**Modified:**
- SadConsole/Terminal/Writer.cs — Cursor type Components.Cursor → TerminalCursor? (nullable, injectable)
- SadConsole/Terminal/Writer.cs — Added null-safe SyncCursorPosition(), DECTCEM, DECSCUSR handlers
- SadConsole/Terminal/Writer.cs — Glyph mapping (Block→219, Underline→95, Bar→124)

**Results:**
- 2,178 tests pass (net8.0, net9.0, net10.0)
- 44 TerminalCursor-specific tests added (Roy + Rachael collaboration)
- Zero regressions

**Key design:** TerminalCursor replaces Components.Cursor to eliminate IComponent overhead. Writer needs only position/visibility/shape — enables headless ANSI rendering + cleaner terminal/UI separation.

**Dependency:** Gaff's host render steps depend on TerminalCursor interface. Rachael's tests validate both null-cursor (data-stream) and injected-cursor (interactive) modes.

**Next:** TerminalConsole Phase 2 — inherit ScreenSurface instead of Console (per 2026-03-09T00:14 directive).

## 2026-03-09 — TerminalConsole Phase 2 Complete

Delivered `SadConsole/TerminalConsole.cs` — the class that wires Writer + TerminalCursor onto a ScreenSurface.

**Created:** `SadConsole/TerminalConsole.cs`
- Inherits `ScreenSurface` (NOT Console — avoids Components.Cursor dead weight)
- Namespace: `SadConsole` (root, sibling of Console)
- Constructor `(int width, int height, IFont? font)` — creates Writer(Surface, Font), TerminalCursor, wires Cursor to Writer
- Render step: Uses `GameHost.Instance.GetRendererStep(Constants.RenderStepNames.TerminalCursor)` factory pattern (same as Components.Cursor uses for its render step). SetData(terminalCursor), add to Renderer.Steps, sort.
- `Feed(string)` and `Feed(ReadOnlySpan<byte>)` convenience methods forwarding to Writer
- Focus: OnFocused/OnFocusLost toggle TerminalCursor.IsVisible. ProcessKeyboard returns true when focused (captures input for Phase 3 KeyboardEncoder).
- UseKeyboard set to `Settings.DefaultConsoleUseKeyboard` (matches Console pattern)
- OnRendererChanged override re-adds cursor render step when renderer swaps
- Dispose cleans up render step

**Key pattern learned:** Host render steps are accessed via `GameHost.Instance.GetRendererStep(name)` factory — core library never references concrete host types. The RenderStepNames.TerminalCursor constant ("terminalcursor") was already registered in Phase 1 Constants.cs. Hosts register the concrete TerminalCursorRenderStep type at startup.

**Build:** 0 errors, 48 warnings (all pre-existing). **Tests:** 726/726 pass (net10.0), zero regressions.

**Next:** Phase 3 — KeyboardEncoder (convert keystrokes to terminal sequences for bidirectional communication).

## 2026-03-09 — Scribe Reconciliation

Orchestration logs written for Roy (Phase 2 impl) and Rachael (33 tests). Session log captures phase completion. No decision inbox items.

**Status:** All files staged and committed to experiments branch @ 9b94793c.

**Cross-team sync:** TerminalConsole Phase 2 complete (759/759 tests). Cursor integration ready for Phase 3 (KeyboardEncoder).

**Next phase:** Host integration testing (Gaff) and keyboard encoding layer (Phase 3).


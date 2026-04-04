# Decisions — SadConsole Squad

**Note:** Entries older than 30 days have been archived to `decisions-archive.md`.

---

## Architecture Decision: SadConsole Editor Addin System

**Author:** Deckard (Lead) | **Date:** 2026-07-10 | **Status:** Approved by Thraka

**Requested by:** Thraka

### Summary

Designed a complete addin system for the SadConsole Editor enabling third-party DLLs to be dropped into an `addins/` folder for auto-loading at startup. Addins can provide custom document types, file handlers, tools, ImGui panels, and menu items.

### Key Decisions

- **Entry Point:** `IEditorAddin` interface (Editor/Addins/IEditorAddin.cs)
- **Discovery:** `EditorAddinAttribute` assembly marker + `AddinLoader` (Editor/Addins/AddinLoader.cs)
- **Menu Integration:** `AddinMenuItem` records; `Core.State.AddinMenuItems` list; `GuiTopBar` renders them
- **Deployment:** Addins reference Editor.exe directly (no SDK NuGet; Editor is downloadable)
- **Trust Model:** Fully trusted — direct Core.State access
- **Sample Addin:** Editor.Addin becomes template/reference project

### Changes Required in Editor

- `Core.State.cs` — Add `AddinMenuItems` list; make `DocumentBuilders` mutable post-init
- `GuiTopBar.cs` — Render addin menu items
- `Program.cs` — Call `AddinLoader.LoadAndRegisterAddins()` in startup

**Full specification:** See orchestration log for complete architecture details.

---
# Decisions — SadConsole Squad

**Note:** Entries older than 30 days have been archived to `decisions-archive.md`.

---

## User Directive: Encoding-Aware Glyph Handling for Terminal.Writer

**Date:** 2026-03-04T18:35:00Z  
**By:** Thraka (via Copilot)  
**Category:** Implementation Directive  
**Status:** Implemented

The Writer needs encoding-aware glyph handling. SadConsole generally only supports CP437 (0-255 glyphs). Characters need conversion to CP437 when the font doesn't define the Unicode code point. Use `_font.GlyphRectangles.ContainsKey(charIndex)` — if the font defines it, print as-is; if not, try CP437 conversion; if that fails, print as-is (SadConsole shows invalid glyph). There should be a configurable encoding setting on the Writer.

**Rationale:** User request — SadConsole's font system is CP437-based but may support Unicode fonts in the future.

**Implementation:** Added `CharacterEncoding` enum (Codepage437/Unicode) and `Writer.Encoding` property. CP437 mode checks font first, then maps Unicode→CP437. Unicode mode passes through as-is.

---

## User Directive: Line Ending Mode for Terminal.Writer

**Date:** 2026-03-04T18:35:01Z  
**By:** Thraka (via Copilot)  
**Category:** Implementation Directive  
**Status:** Implemented

The Writer needs a configurable line ending mode (Linux vs Windows). In Linux mode, LF alone moves to the next line (implicit CR). In Windows mode, CR+LF is required — LF without preceding CR only moves down, not to column 0. Users need to know the type of data they're feeding the parser and configure accordingly.

**Rationale:** User request — different data sources use different line ending conventions.

**Implementation:** Added `LineFeedMode` enum (Strict/Implicit) and `Writer.LineFeeds` property. Default is Implicit (LF = CR+LF, Linux/BBS behavior). Users working with raw VT100 can switch to Strict.

---

## Terminal.Writer Integration Test Contract — Phase 1

**Author:** Rachael (Tester) | **Date:** 2026-03-04 | **Status:** Complete

Wrote 73 integration tests at `Tests/SadConsole.Tests/TerminalWriterTests.cs` defining the contract for `SadConsole.Terminal.Writer` rendering onto `ICellSurface`. Tests use real `CellSurface` instances (not mocks).

### Writer API Contract Assumed

```csharp
public class Writer : ITerminalHandler
{
    public Writer(ICellSurface surface);
    public void Feed(string text);
    public void Feed(ReadOnlySpan<byte> data);
    public TerminalState State { get; }  // .CursorX, .CursorY
    public Color[] Palette { get; }      // 256-color palette
    public CharacterEncoding Encoding { get; set; }  // CP437/Unicode
    public LineFeedMode LineFeeds { get; set; }      // Strict/Implicit
}
```

### Key Behavioral Decisions (team-relevant)

1. **Default colors:** fg=White, bg=Black — standard VT100 convention (not CellSurface's default Transparent bg)
2. **LF behavior:** LF alone moves cursor to column 0 and down one row (implicit CR+LF) — standard terminal behavior (can be switched to Strict mode)
3. **CUP parameters:** 1-based row;col, with 0 treated as 1 per ECMA-48
4. **Bold + standard color:** Produces bright variant (palette index + 8)
5. **Reverse video (SGR 7):** Display-level fg/bg swap — the cell's Foreground gets the bg color, Background gets the fg color
6. **Scroll:** Triggered when cursor moves past last row; top row scrolled out, new bottom row cleared to spaces
7. **RIS (ESC c):** Full reset — clears screen, cursor to home, resets all SGR attributes
8. **Erase:** Cleared cells get space (0x20) glyph with current default colors
9. **Tab stops:** Default every 8 columns
10. **Unrecognized CSI:** Silently ignored (no crash, no output)
11. **CharacterEncoding:** CP437 mode checks font first, then maps Unicode→CP437. Unicode mode passes through as-is for Unicode fonts.
12. **CP437 Glyph Handling:** Static lookup table avoids NuGet dependency. If font defines Unicode point, uses it directly; otherwise tries CP437 mapping.

### Team Impact

- **Roy:** These tests define what Writer must do. Build to make them pass. The `State` object needs `.CursorX` / `.CursorY` properties. `Palette` should be `Color[]` with ≥256 entries. Encoding and LineFeeds properties configurable.
- **Deckard:** LF-implies-CR decision is simplification. Real terminals differ. Mode flag allows per-data-source configuration.
- **Gaff:** No host impact — Writer is purely data-layer (CellSurface manipulation).

---

## Decision: Encoding-Aware Glyph Handling & LineFeed Mode for Terminal.Writer

**Author:** Roy (Core Dev) | **Date:** 2026-03-05 | **Status:** Implemented

### Summary

Added two configurable behaviors to `SadConsole.Terminal.Writer`:

1. **CharacterEncoding mode** (`Writer.Encoding` property) — Controls how incoming Unicode characters are mapped to font glyph indices. Default `Codepage437` checks the font's `GlyphRectangles` first, then falls back to a static CP437 lookup table. `Unicode` mode passes characters through as-is for Unicode-capable fonts.

2. **LineFeedMode** (`Writer.LineFeeds` property) — Controls how LF (0x0A) is handled. Default `Implicit` treats LF as CR+LF (move to column 0 and down), matching Linux terminal behavior and BBS ANSI art conventions. `Strict` mode treats LF as down-only (classic ANSI/VT behavior where CR must be explicit).

### Key Design Decisions

- **Static CP437 table instead of System.Text.Encoding.CodePages** — Avoids adding a NuGet dependency. The 256-character CP437 mapping is well-known and static. Reverse lookup table built once at class load.
- **Font-first glyph resolution** — In CP437 mode, if the font's `GlyphRectangles` already contains the Unicode code point, use it directly. Only fall back to CP437 mapping when the font doesn't know the character. This allows fonts with extended Unicode support to work correctly in CP437 mode.
- **`cell.Glyph` (int) instead of `cell.GlyphCharacter` (char)** — Resolved glyph values are set as integers since they may be CP437 byte indices (0–255), not Unicode code points.
- **Implicit LF as default** — Most ANSI art sources and Linux terminals treat LF as CR+LF. Users working with raw VT100 streams can switch to `Strict`.

### Files Modified

- `SadConsole/Terminal/Writer.cs` — Added enums, properties, `ResolveGlyph()`, CP437 table, LF mode logic
- `Tests/SadConsole.Tests/TerminalWriterTests.cs` — Updated LF test, added Strict mode test

### Team Implications

- **Gaff (Host Dev):** No impact — all changes are in core Writer, no host rendering changes needed.
- **Pris (Controls Dev):** No impact — Terminal.Writer is independent of controls.
- **Rachael (Tester):** New `Encoding` and `LineFeeds` properties available for test coverage. Consider adding tests for CP437 mapping edge cases (box-drawing chars, Greek letters) and Unicode pass-through mode.

---

## Decision: Parser Encoding Modes (CP437/UTF-8)

**Author:** Roy (Core Dev) | **Date:** 2026-03-05 | **Status:** Implemented

### Summary

Added `ParserEncoding` enum and `Parser.Encoding` property to control how high bytes (0x80–0xFF) are interpreted.

**Codepage437 mode (default):** Bytes > 0x7F are passed directly to `OnPrint()` as CP437 glyph indices. No UTF-8 decoding.

**UTF-8 mode:** Bytes > 0x7F trigger `DecodeUtf8()` to accumulate and decode multibyte sequences before dispatching to `OnPrint()`.

### Rationale

CP437 box-drawing characters (e.g., 0xC9 = ╔) were being mangled by unconditional UTF-8 decoding in the parser. Different data sources require different byte interpretations:
- ANSI art files (CP437 encoded) — use CP437 mode
- Modern UTF-8 streams — use UTF-8 mode

### Key Design Decisions

- **ParserEncoding enum** — Not the same as `System.Text.Encoding`. Used only for Parser high-byte handling.
- **Conditional decoder reset** — `_utf8Decoder.Reset()` only called when `Encoding == ParserEncoding.Utf8`.
- **HandleGround branching** — Bytes 0x80–0xFF dispatch to `OnPrint((char)b)` in CP437, `DecodeUtf8(b)` in UTF-8.

### Files Modified

- `SadConsole/Terminal/Parser.cs` — `ParserEncoding` enum, `Encoding` property, `HandleGround` branching
- `Tests/SadConsole.Tests/TerminalParserTests.cs` — 4 UTF-8 multibyte tests updated

### Test Status

All 87 Parser tests pass with dual-mode support. 4 UTF-8 multibyte tests set `_parser.Encoding = ParserEncoding.Utf8` explicitly.

---

## Decision: PendingWrap Clearing Must Be Opt-In, Not Opt-Out

**Author:** Deckard | **Date:** 2025-07-16 | **Status:** Implemented  
**Scope:** `SadConsole/Terminal/Writer.cs` — `OnCsiDispatch` method  
**Affects:** Roy (implementation), Rachael (tests)

### Problem

`OnCsiDispatch` (line 342) unconditionally clears `State.PendingWrap = false` as an epilogue after the dispatch switch. This means **every** CSI sequence — including SGR (`ESC[...m`) — resets pending-wrap state. Per ECMA-48 §7.1 and xterm behavior, SGR and other non-cursor-moving sequences must **not** clear pending-wrap. The result: ANSI art that sets colors at column-79 boundaries (common in BBS art) experiences progressive line drift.

The same pattern exists at line 224 for DEC private modes (`CSI ? ...`), where DECTCEM (cursor visibility) and DECAWM (auto-wrap toggle) should not clear pending-wrap either.

### Root Cause Analysis

Git history shows Writer.cs has a single commit — the blanket clear was baked into the original implementation. The design assumed "all CSI sequences affect cursor positioning" and applied a catch-all epilogue. This is an **opt-out** model: pending-wrap is cleared for everything, and any exception must be individually carved out.

The spec (ECMA-48) is actually quite clear: only operations that move the cursor should clear pending-wrap. The implementation got it wrong because of a structural choice: a blanket "clean up everything" epilogue in the dispatcher rather than in individual handler methods.

This is the same class of error as a database transaction that auto-commits after every query — the default behavior is too aggressive, and the correct behavior must be opted into.

### Design Decision: Invert to Opt-In Clearing

**Remove** the blanket `State.PendingWrap = false` from:
- Line 342 (end of `OnCsiDispatch`, after the switch)
- Line 224 (after `HandleDecPrivateMode` call)

**Instead**, each handler that genuinely moves the cursor clears `PendingWrap` itself.

#### Classification of CSI Sequences

**MUST clear PendingWrap** (cursor-moving):
| Sequence | Mnemonic | Reason |
|----------|----------|--------|
| `H` / `f` | CUP / HVP | Absolute cursor positioning |
| `A` | CUU | Cursor up |
| `B` | CUD | Cursor down |
| `C` | CUF | Cursor forward |
| `D` | CUB | Cursor backward |
| `E` | CNL | Cursor next line |
| `F` | CPL | Cursor previous line |
| `G` | CHA | Cursor horizontal absolute |
| `d` | VPA | Vertical position absolute |
| `I` | CHT | Cursor forward tab |
| `Z` | CBT | Cursor backward tab |
| `u` | Restore cursor | Restores position (already clears in `State.RestoreCursor()`) |
| `r` | DECSTBM | Homes cursor on set |

**MUST clear PendingWrap** (DEC private — cursor-moving subset):
| Mode | Mnemonic | Reason |
|------|----------|--------|
| 6 | DECOM | Origin mode homes cursor |

**MUST NOT clear PendingWrap** (non-cursor-moving):
| Sequence | Mnemonic | Reason |
|----------|----------|--------|
| `m` | SGR | Attribute change only — **this is the reported bug** |
| `J` | ED | Erase display — cursor stays put |
| `K` | EL | Erase in line — cursor stays put |
| `X` | ECH | Erase characters — cursor stays put |
| `n` | DSR | Query — no side effects |
| `s` | Save cursor | Saves state, no cursor change |
| `g` | TBC | Tab stop management — no cursor change |
| `S` | SU | Scroll up — region scrolls, cursor stays |
| `T` | SD | Scroll down — region scrolls, cursor stays |

**Needs careful consideration** (content-shifting, cursor may be implicitly affected):
| Sequence | Mnemonic | Current behavior | Recommendation |
|----------|----------|------------------|----------------|
| `@` | ICH | Insert chars at cursor | Clear — shifts content at cursor |
| `P` | DCH | Delete chars at cursor | Clear — shifts content at cursor |
| `L` | IL | Insert lines | Clear — xterm clears |
| `M` | DL | Delete lines | Clear — xterm clears |
| `b` | REP | Repeat last char | Manages wrap itself — do not clear in epilogue |

**DEC private modes that MUST NOT clear PendingWrap:**
- DECCKM (1), DECSCNM (5), DECAWM (7), DECTCEM (25)

### Implementation Approach for Roy

#### Step 1: Remove the blanket clears
Delete `State.PendingWrap = false;` from:
- Line 342 (after the main switch)
- Line 224 (after `HandleDecPrivateMode`)

#### Step 2: Add explicit clears to cursor-moving handlers
At the top of each cursor-moving case in the switch, or inside the called method, add `State.PendingWrap = false;`. The handler methods (`HandleCursorPosition`, `MoveCursorUp`, etc.) are the right place if they're only called from cursor-moving contexts.

#### Step 3: Handle DEC private DECOM specially
In `HandleDecPrivateMode`, add `State.PendingWrap = false;` only inside the `case 6` (DECOM) block, which homes the cursor.

#### Step 4: Document the pattern
Add a comment at the top of `OnCsiDispatch` stating the design rule:
```
// NOTE: PendingWrap is NOT cleared here. Each handler that moves the cursor
// is responsible for clearing State.PendingWrap. This is intentional per
// ECMA-48 — non-cursor-moving sequences (SGR, erase, etc.) must preserve
// pending-wrap state.
```

### Prevention: Making the Right Thing Easy

The opt-in model makes "not clearing" the default — which is the correct behavior for the majority of CSI sequences. A developer adding a new CSI handler must think: "Does this move the cursor? If yes, clear PendingWrap." This is a smaller, safer checklist than "Does this NOT move the cursor? If so, remember to skip the clear."

For future-proofing, Rachael should add a test case for every CSI sequence that verifies pending-wrap preservation/clearing:
- Set cursor to column (width-1), print a character (sets PendingWrap), then issue the CSI sequence, then verify PendingWrap is in the expected state.

### Implementation Outcome

Roy implemented the change on 2026-07-16. Removed 2 blanket clears and added explicit clears to 17 cursor-moving CSI handlers + DECOM. All 662 tests pass with zero regressions. Rachael wrote 8 regression tests (670 total pass) covering SGR-at-boundary preservation, cursor-move clearing, DEC private mode preservation, ECH preservation, multiple SGR chaining, and integration test with b5-ans01.ans.

---

## Decision: CUF Resolves PendingWrap at Right Margin

**Author:** Roy  
**Date:** 2026-03-06T06:43Z  
**Status:** Implemented  

### Context

When `CSI C` (CUF — Cursor Forward) arrives while `PendingWrap` is true at the right margin (col 79 on 80-wide surface), the Writer previously just cleared PendingWrap and attempted `Math.Min(79, 79 + n)` — a no-op. This broke ANSI art (specifically b5-ans01.ans) where `CSI 6C` after filling 80 columns was expected to position the cursor on the next line.

### Decision

CUF now **resolves** the pending wrap before applying forward movement: `col = 0`, `LineFeed()`, then `MoveCursorForward(n)`. Only CUF gets this behavior — other cursor movement commands (CUB, CUU, CUD, CUP, CHA, VPA) continue to just clear the flag.

### Rationale

- CUF from the right margin with PendingWrap is inherently a no-op without resolution (clamped at margin). Resolving first is strictly more useful.
- Matches ANSI.SYS immediate-wrap semantics used by BBS/ANSI art.
- 673 tests pass, zero regressions.

### Impact

- **Writer.cs**: CUF case modified (8 lines added)
- **TerminalWriterPhase3Tests.cs**: 3 regression tests added

---

## RowFontSurface — Multi-Font Row Surface Architecture

**Date:** 2026-03-02T21  
**Author:** Deckard (Lead)  
**Category:** Architecture — New Surface Type  
**Status:** Specification complete. Implementation by Roy (core) and Gaff (hosts) in progress.

### Summary

A new surface type where **each row can use a different font**. Extends `ScreenSurface` without modifying existing surfaces. Enables rich typography (mixed font sizes per row) and per-row font overlays.

### Key Design Decisions

**1. No Cached Rectangles**  
Variable row heights prevent pre-caching destination rects. Renderers compute `destRect` on the fly:
```csharp
XnaRectangle destRect = new XnaRectangle(
    x * rowFontSize.X,
    rowYOffset,
    rowFontSize.X,
    rowFontSize.Y);
```

**2. Sparse Row Font Storage**  
`Dictionary<int, IFont>` and `Dictionary<int, Point>` provide sparse storage. Rows not in dictionary fall back to default `Font` and `FontSize` properties. Efficient for typical use cases where only a few rows have custom fonts.

**3. Pre-Calculated RowYOffsets Array**  
Array indexed by row number caches Y pixel offsets. Recalculated via `RecalculateRowOffsets()` whenever fonts or sizes change. Avoids redundant calculation during rendering.

**4. Variable Height Calculation**  
`HeightPixels` is no longer `View.Height * FontSize.Y`. New formula:
```csharp
public override int HeightPixels => 
    RowYOffsets[RowYOffsets.Length - 1] + GetRowHeight(RowYOffsets.Length - 1);
```

**5. Uniform Width Calculation**  
Width uses default `FontSize.X` for column alignment. All cells in a column align horizontally regardless of row font. Per-column fonts out of scope.

**6. Mouse Input Coordinate Mapping**  
`PixelToCell(Point pixelPosition)` method uses linear search through `RowYOffsets` to find which row contains Y coordinate. Necessary for variable-height row mouse input.

**7. Resize Behavior**  
Override `Resize()` to call `RecalculateRowOffsets()`. User responsible for managing `RowFonts` and `RowFontSizes` dictionaries if rows are added/removed.

**8. Serialization via Font Names**  
`RowFonts` and `RowFontSizes` marked with `[DataMember]` and `[JsonConverter(typeof(RowFontDictionaryConverter))]`. Fonts serialized by name, resolved from `GameHost.Fonts` at load time.

### Core Implementation (Roy)

**File:** `SadConsole/RowFontSurface.cs`  
**Extends:** `ScreenSurface`

**Key Properties:**
- `RowFonts: Dictionary<int, IFont>` — row to font mapping
- `RowFontSizes: Dictionary<int, Point>` — row to font size mapping
- `RowYOffsets: int[]` — cached Y pixel offsets per row

**Key Methods:**
- `SetRowFont(int row, IFont font, Point? fontSize = null)` — assign font to row
- `GetRowFont(int row): IFont` — get font with fallback
- `GetRowFontSize(int row): Point` — get size with fallback
- `RecalculateRowOffsets()` — pre-calculate Y offsets
- `GetRowYOffset(int row): int` — O(1) lookup
- `GetRowHeight(int row): int` — row height in pixels
- `PixelToCell(Point pixelPosition): Point` — mouse coordinate mapping

**Constants:** `SadConsole/Renderers/Constants.cs`
- `RendererNames.RowFontSurface = "rowfontsurface"`
- `RenderStepNames.RowFontSurface = "rowfontsurface"`
- `RenderStepSortValues.RowFontSurface = 50`

### Host Implementations (Gaff)

**MonoGame Host:**
- `SadConsole.Host.MonoGame/Renderers/RowFontSurfaceRenderer.cs`
- `SadConsole.Host.MonoGame/Renderers/Steps/RowFontSurfaceRenderStep.cs`
- Registration in `Game.Mono.cs`

**SFML Host:**
- `SadConsole.Host.SFML/Renderers/RowFontSurfaceRenderer.cs`
- `SadConsole.Host.SFML/Renderers/Steps/RowFontSurfaceRenderStep.cs`
- Registration in `Game.cs`

**FNA Host:**
- Shares MonoGame via compile includes; no separate files needed

**Renderer Pattern:**
1. Clear base `ScreenSurfaceRenderer` steps
2. Add `RowFontSurfaceRenderStep` (draws surface)
3. Add `OutputSurfaceRenderStep` (blits to output)
4. Add `TintSurfaceRenderStep` (applies tint)

**Render Loop Structure:**
```csharp
for (int y = 0; y < surface.View.Height; y++)
{
    IFont rowFont = rowFontSurface.GetRowFont(y);
    Point rowFontSize = rowFontSurface.GetRowFontSize(y);
    int rowYOffset = rowFontSurface.GetRowYOffset(y);
    
    for (int x = 0; x < surface.View.Width; x++)
    {
        Rectangle destRect = new Rectangle(
            x * rowFontSize.X,
            rowYOffset,
            rowFontSize.X,
            rowFontSize.Y);
        // Draw background, glyph, decorators...
    }
}
```

### Performance Implications

- **No cached rects:** Small performance hit vs. `ScreenSurfaceRenderer` due to on-the-fly rect calculation
- **Per-row font lookups:** Dictionary lookups on every row — negligible for typical row counts (< 100)
- **Multiple texture switches:** If rows use different fonts, texture switching per row — acceptable for < 5 unique fonts

Future optimization: Batch rows by font to minimize texture switches.

### Testing Strategy (Rachael)

1. Basic multi-font rendering — 3 rows with different fonts
2. Height calculation — `HeightPixels` matches sum of row heights
3. Mouse input — `PixelToCell` returns correct coordinates
4. Viewport scrolling — rows render correctly when scrolled
5. Runtime font change — `SetRowFont` triggers re-render
6. Surface resize — `RowYOffsets` recalculates correctly
7. Serialization — custom row fonts persist/restore
8. Fallback behavior — unset rows use default font

### Related Documents

- Specification: `.squad/decisions/inbox/deckard-multifont-surface.md` (detailed)
- Orchestration logs: `.squad/orchestration-log/2026-03-02T21-{deckard,roy,gaff}.md`
- Session log: `.squad/log/2026-03-02T21-rowfontsurface.md`

### Sign-Off

**Deckard (Lead):** Specification ready for implementation.  
**Roy (Core Dev):** Implementation in progress. Builds clean.  
**Gaff (Host Dev):** Implementation in progress. All hosts build clean.

---

## PendingWrap Resolution Extended to Forward Tab (CHT / C0 HT)

**Date:** 2025-07-17 | **Updated:** 2026-03-06  
**Author:** Roy (Core Dev)  
**Category:** Terminal Cursor Movement  
**Status:** Implemented

### Summary

Comprehensive audit of all cursor-movement handlers in Writer.cs for the same "stuck at right margin" bug previously found in CUF. Found and fixed the same bug pattern in **CHT (CSI I — Forward Tab)** and **C0 HT (0x09 — Tab Character)**.

### Bug Pattern

When PendingWrap=true at the right margin (col = width-1), forward-moving handlers that simply clear the flag and apply clamped movement become no-ops: NextTabStop(79) = 79 on an 80-column surface. The cursor stays stuck at the margin instead of resolving the pending wrap and moving forward on the next line.

### Fix Applied

Same pattern as CUF: when PendingWrap && AutoWrap, resolve the wrap first (advance to next line col 0 via LineFeed()), then apply the forward tab movement.

### All Other Handlers Verified Safe

- **Absolute positioning** (CUP, CHA, VPA, DECSTBM, DECOM, CSI u, NEL, RI): set cursor to specific location, not affected by current position
- **Backward/vertical relative** (CUU, CUD, CUB, CNL, CPL, CBT): movement target differs from the margin, so the cursor does actually move
- **Edit-in-place** (ICH, DCH, IL, DL): operate at current position, no forward movement

### Team Impact

- **Rachael:** 6 new regression tests (679 total). Tests cover CHT, C0 HT, and multi-tab scenarios.
- **Gaff/Pris:** No rendering or control changes — purely core cursor-movement logic.
- **Specification compliance:** ECMA-48 §7.1 strict adherence via opt-in architecture.

---

## Decision: Bold Brightens Default Foreground (CGA Convention)

**Author:** Roy  
**Date:** 2026-03-06T07:30Z  
**Status:** Implemented  

### Context

`ResolveForeground()` in `Writer.cs` only applied bold brightening (palette shift 0-7 → 8-15) when `ForegroundMode == Palette`. When `ForegroundMode == Default` (after SGR 0 reset), bold was ignored—returning gray (170,170,170) instead of bright white (255,255,255).

This caused visible rendering errors in b5-ans01.ans and likely any ANSI art that uses `ESC[0m` (reset) followed by `ESC[1m` (bold) without an explicit foreground color.

### Decision

When `ForegroundMode == Default` and `Bold` is true, resolve foreground as `Palette[15]` (bright white) instead of `DefaultForeground`. This follows the CGA convention that the default foreground IS palette index 7, and bold shifts it to palette index 15.

### Impact

- **Writer.cs:** One-line change in `ResolveForeground()` default case
- **Tests:** 3 new tests in `TerminalWriterPhase3Tests.cs` (682 total, all pass)
- **ANSI art compatibility:** Fixes white-vs-gray rendering in any file that relies on bold + default foreground

---

## Architecture Decision: Terminal Writer Restructuring — Cursor Ownership & ITerminalOutput

**Author:** Deckard (Lead)  
**Date:** 2026-03-07T18:50:48Z  
**Status:** Proposed — Awaiting Thraka Design Decisions  
**Requested by:** Thraka

### Context

Three core architecture questions about evolving `SadConsole.Terminal.Writer` (1274 lines):

1. Should Writer's Cursor come from outside (injected), not created internally?
2. Should there be two different writers — data-stream writer (ANSI art browser) vs interactive terminal writer (BBS/telnet)?
3. Should `TerminalConsole` subclass `Console` for interactive operation?

### Summary of Recommendations

**Q1: Injectable Nullable Cursor.** ✅ **Recommended.**  
Make Cursor an injectable, nullable property. Writer creates Cursor internally currently (line 91–96); change to optional constructor parameter or property setter. State (State.CursorRow/State.CursorColumn) remains the terminal logic authority; Cursor.Position is a one-way display-only sync target.

**Rationale:** Surface is already external (passed to constructor). Cursor is the only internal dependency. Making it external enables:
- Headless mode (ANSI art rendering without visual cursor)
- Cursor sharing across Writers
- Custom Cursor subclasses
- Clean separation: data-stream mode (null cursor) vs interactive mode (provided cursor)

**Backwards Compatibility:** Minor breaking change — tests that assert `writer.Cursor != null` need updating. Add convenience constructor that accepts a Cursor for migration path.

---

**Q2: Two Writer Types vs Single Writer with Optional Output Channel.** ✅ **Recommendation: Single Writer with `ITerminalOutput?` response channel.**  
Do NOT split Writer into two classes. The 1200+ lines of ANSI sequence handling are identical for both use cases. Only differences:
- (a) Whether DA/DSR emit responses (add `ITerminalOutput?` property; null = silent)
- (b) Whether Cursor is present (make it optional)
- (c) Whether keyboard input is handled (separate class — `TerminalConsole` or `ProcessTerminal`)

**Design:**
```csharp
public interface ITerminalOutput
{
    void WriteResponse(ReadOnlySpan<byte> data);
}

public class Writer : ITerminalHandler
{
    public Components.Cursor? Cursor { get; set; }
    public ITerminalOutput? Output { get; set; }  // null for data-stream mode
}
```

**Rationale:** Configuration toggles are cleaner than code duplication. Existing TODO stubs for DA/DSR become live code when Output is set.

---

**Q3: TerminalConsole — Subclass or Composition.** ✅ **Recommendation: Subclass Console.**  
`TerminalConsole : Console` (not composition). Console already provides Surface + Cursor + focus + keyboard + rendering. TerminalConsole adds:
- Writer wired to Console.Surface and Console.Cursor
- Keyboard override to convert keystrokes to terminal sequences
- Response channel callback (DSR/query replies)

Follows existing pattern (ControlsConsole, Window both extend Console).

```csharp
public class TerminalConsole : Console
{
    public Writer Writer { get; }
    
    public TerminalConsole(int width, int height, IFont font)
        : base(width, height)
    {
        Writer = new Writer(Surface, font, Cursor);
    }
    
    public override bool ProcessKeyboard(Input.Keyboard keyboard)
    {
        // Encode keystrokes to terminal sequences
        return base.ProcessKeyboard(keyboard);
    }
}
```

---

### The Dual Cursor State Problem

**State.CursorRow/CursorColumn vs Cursor.Position** — two representations of cursor position.

**Resolution: State is authoritative; Cursor is display-only.**

Data flow during ANSI processing:
```
ANSI input → Parser → Writer.OnXxx() → State mutation → SyncCursorPosition() → Cursor.Position
```

In TerminalConsole, user input goes through the terminal protocol:
```
Keypress → KeyboardEncoder → ANSI bytes → Writer.Feed() → (same flow as above)
```

Never a reason to set `Cursor.Position` directly during terminal operation. TerminalConsole should set `Cursor.MouseClickReposition = false` by default.

### Files Affected

- `SadConsole/Terminal/Writer.cs` — Cursor constructor param, Output property, null checks in SyncCursorPosition()
- `SadConsole/Terminal/ITerminalOutput.cs` — NEW interface
- `SadConsole/Terminal/TerminalConsole.cs` — NEW class (subclass Console)
- `Tests/SadConsole.Tests/TerminalWriterTests.cs` — Update Cursor null-checks

### Backwards Compatibility

| Change | Impact | Migration |
|--------|--------|-----------|
| `Writer.Cursor` nullable | Tests accessing `.Cursor.Position` need null-aware access | Low — add convenience constructor |
| `Writer` constructor change | Tests injecting Cursor must use new signature | Low — most tests verify cells, not cursor |
| `ITerminalOutput` interface | Purely additive | None |
| `TerminalConsole` class | Purely additive | None |

---

### Open Questions for Thraka

1. **Breaking change tolerance:** Making `Writer.Cursor` nullable is a minor breaking change. Acceptable for next major version, or add parameterless constructor (mark `[Obsolete]`) for compatibility?

2. **KeyboardEncoder scope:** Should ANSI keyboard encoding (arrow keys, F-keys, Ctrl+key, application mode) be a static helper on Writer, or standalone class in `SadConsole.Terminal`? (Leaning toward standalone for testability and reuse.)

3. **TerminalConsole namespace:** `SadConsole.Terminal.TerminalConsole` or `SadConsole.TerminalConsole` (root namespace like Console/Window/ControlsConsole)?

### Related Documents

- Technical deep-dive: `.squad/decisions/inbox/roy-terminal-cursor-analysis.md`
- Orchestration logs: `.squad/orchestration-log/2026-03-07T18-50-48-{deckard,roy}.md`
- Session log: `.squad/log/2026-03-07T18-50-48-terminal-writer-architecture.md`

---

## Technical Deep-Dive: Terminal Cursor Architecture — Roy Analysis

**Author:** Roy (Core Dev)  
**Date:** 2026-03-07T18:50:48Z  
**Status:** Complete  

### Summary

Technical analysis supporting Deckard's architecture decision. Key findings:

**State vs Components.Cursor Duality:**
- **State.CursorRow/CursorColumn:** Pure integer tracking — the logical cursor position. Always accurate, updated by all cursor handlers. Zero dependency on Components.Cursor.
- **Components.Cursor.Position:** Visual echo. ONE-WAY sync from State → Cursor via `SyncCursorPosition()`. Writer never reads Cursor.Position.

**External Cursor Injection Risk Assessment: LOW-RISK**

What breaks:
- Null checks needed in `SyncCursorPosition()` and DECTCEM handler (2 places)
- Caller responsibility: ensure Cursor targets same surface, set `AutomaticallyShiftRowsUp = false`

What becomes possible:
- Headless mode (no visual cursor for ANSI art export)
- Cursor sharing (multiple Writers → single Cursor)
- Custom Cursor subclasses

**Reader Role:** Pure transport layer. Throttles bytes to Writer via `Writer.Feed()`. Decoupled from terminal logic.

**Two Writer Modes Justified:**
- **Data-stream:** Static ANSI art rendering, cursor irrelevant, instant or throttled playback
- **Interactive:** Bi-directional I/O, keyboard/mouse, cursor critical for feedback, DSR/query responses

But: Splitting would duplicate 1200+ lines. Better: single Writer with optional Cursor + ITerminalOutput, wrapped by TerminalConsole for interactive use.

**TerminalConsole Pattern:** Subclass Console (minimizes duplication, inherits rendering/focus/keyboard). Override ProcessKeyboard to encode keystrokes. Wire Writer to Console.Surface and Cursor.

### Phased Implementation Approach

**Phase 1 (Low-Risk):** Make Cursor external  
- Constructor: `Writer(ICellSurface surface, IFont font, Components.Cursor? cursor = null)`
- Null checks in sync code
- Result: data-stream mode doesn't need Cursor

**Phase 2 (Medium Effort):** Create TerminalConsole  
- Subclass Console
- Wire Writer to Surface + Cursor
- Override ProcessKeyboard for interactive input
- Result: interactive terminal emulator ready

**Phase 3 (Optional Future):** Split if complexity grows  
- TerminalRenderer (pure parser + surface writer)
- InteractiveTerminal orchestrates TerminalRenderer + I/O channels

### Related Documents

- Architecture decision: `.squad/decisions/inbox/deckard-terminal-writer-architecture.md`
- Orchestration logs: `.squad/orchestration-log/2026-03-07T18-50-48-{deckard,roy}.md`
- Session log: `.squad/log/2026-03-07T18-50-48-terminal-writer-architecture.md`

---

## Session Summary: Terminal Writer Architecture Analysis (2026-03-07)

**Agents:** Deckard (Lead), Roy (Core Dev)  
**Status:** Complete — Zero Disputes

Converged on identical recommendations across three architecture questions through parallel analysis. Deckard provided architecture-level design; Roy provided technical implementation-level validation. All findings merged to this decision document.

---

## Architecture Decision: TerminalMode Enum for Unified Terminal Behavior Control


**By:** Deckard (Lead)
**What:** Architectural design for `TerminalMode` — a single enum controlling behavior profiles across both the rendering (Writer) and input (KeyboardEncoder) subsystems of `SadConsole.Terminal`.

**Why:** Different terminal standards (CTerm, ANSI-BBS, VT102, xterm) define conflicting behaviors for C0 control bytes, screen-erase side effects, and keyboard escape sequences. A unified mode enum lets users switch behavior profiles with one property (`TerminalConsole.Mode`) rather than manually toggling a dozen boolean flags. Roy has implemented this design; this decision records the rationale and contracts.

**Details:**

- **TerminalMode enum values:** `CTerm` (default), `AnsiBbs`, `VT102`, `XTerm`
- **File location:** `SadConsole/Terminal/TerminalMode.cs` — standalone file in the Terminal namespace, co-located with Writer and KeyboardEncoder
- **Writer.Mode property:** `public TerminalMode Mode { get; set; } = TerminalMode.CTerm;` on `Writer`. Three behavioral branch points:
  1. `OnC0Control` — FF (0x0C) becomes clear-screen+home-cursor instead of ignored
  2. `OnC0Control` — C0 bytes 0x01–0x06, 0x0B, 0x0E–0x1A, 0x1C–0x1F render as CP437 glyphs via `OnPrint` instead of being silently dropped
  3. `OnCsiDispatch` case `'J'` — ED param 2 (erase entire display) also homes cursor to 0,0
- **KeyboardEncoder.Mode property:** `public TerminalMode Mode { get; set; } = TerminalMode.CTerm;` — reuses the same `TerminalMode` enum (no separate KeyboardEncoderMode). In AnsiBbs mode, overrides: Insert→`CSI @`, Delete→`DEL 0x7F`, End→`CSI K`, PageUp→`CSI V`, PageDown→`CSI U`, F5→`ESC O t`, Backspace→`BS 0x08` always
- **TerminalConsole convenience:** `public TerminalMode Mode { get => Writer.Mode; set { Writer.Mode = value; KeyboardEncoder.Mode = value; } }` — single setter prevents Writer/Encoder drift. Getter reads from Writer (authoritative source)
- **Default:** `CTerm` — preserves all existing behavior. Zero breaking changes for current users.
- **VT102 and XTerm:** Reserved enum values with no behavioral differences from CTerm today. Future work will add VT102 strict-mode restrictions (no CTerm font slots, no BBS extensions) and XTerm conventions (mouse reporting, bracketed paste, etc.)

**Design principles applied:**
- One enum, two consumers — avoids a parallel `KeyboardEncoderMode` that would inevitably drift
- Mode checked at dispatch points, not stored as dozens of booleans — adding a new mode means adding branches, not new state fields
- `IsAnsiBbsGlyphByte` extracted as static helper for auditability (pattern: make the predicate explicit and separately testable)
- FF checked before glyph-byte check in `OnC0Control` for unambiguous intent, even though `IsAnsiBbsGlyphByte` excludes 0x0C

**Implementation notes for Roy (already complete — this records the contracts):**
- Parser.cs requires no changes for any mode — ECMA-48 state machine is mode-agnostic
- State.cs requires no changes — mode is a dispatch-time concern, not session-state
- All mode-conditional branches use `== TerminalMode.AnsiBbs` (not `!= CTerm`) so VT102/XTerm don't accidentally inherit BBS behavior
- `BackspaceSendsDel` property on KeyboardEncoder is bypassed entirely in AnsiBbs mode (the AnsiBbs switch returns `\x08` before reaching the property check)


---

## Implementation Decision: Terminal Mode Implementation

 (roy-terminal-mode-impl)

**Date:** 2026-03-10  
**Author:** Roy  
**Status:** Implemented

## Summary

Added `TerminalMode` enum and mode-based behavior to `Writer` and `KeyboardEncoder`.

## Non-Obvious Choices

### 1. `IsAnsiBbsGlyphByte` as a separate static helper
Rather than embedding the glyph byte set inline in `OnC0Control`, I extracted it to a static `IsAnsiBbsGlyphByte(byte)` method using a switch expression. This keeps the C0 handler readable and makes the set of BBS glyph bytes explicit and easy to audit.

### 2. `OnC0Control` — FF (0x0C) is checked first, before the glyph check
`IsAnsiBbsGlyphByte` deliberately excludes 0x0C, so even if the order were reversed the glyph path would not trigger for FF. However, checking FF explicitly first before the glyph dispatch makes the intent unambiguous.

### 3. `OnC0Control` — early return after ANSI-BBS glyph dispatch
After calling `OnPrint((char)controlCode)` for a BBS glyph byte, we return immediately — skipping the `SyncCursorPosition()` call at the bottom of `OnC0Control`. This is correct: `OnPrint` handles cursor advancement internally, so calling `SyncCursorPosition()` again would be redundant.

### 4. CSI J ANSI-BBS cursor-home — preserves existing non-BBS behavior
The ED `case 'J':` block was refactored from a one-liner into a block with an extracted `edParam` local. The ANSI-BBS cursor-home only applies when `edParam == 2`. The comment "cursor stays put, does NOT clear PendingWrap" from the original is still accurate for CTerm/VT102/XTerm — the ANSI-BBS branch explicitly sets `PendingWrap = false` and calls `SyncCursorPosition()`.

### 5. `TerminalConsole.Mode` setter updates both Writer and KeyboardEncoder
The convenience property sets both at once to avoid a footgun where the Writer and KeyboardEncoder drift out of sync. Since the getter reads from `Writer.Mode`, the two are always in agreement after any set through the console-level property.

### 6. VT102 and XTerm modes — no behavioral changes
Per the task specification, VT102 and XTerm mode values are reserved for future use. All new conditional branches only activate on `TerminalMode.AnsiBbs`, so VT102 and XTerm exhibit identical behavior to CTerm.


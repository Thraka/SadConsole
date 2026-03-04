# Decisions — SadConsole Squad

## Doc Verification Workflow Directive

**Date:** 2026-02-25T14:08:44Z  
**By:** Thraka (via Copilot)  
**Category:** Process

When verifying an architecture doc, corrections should be applied directly to the source doc (e.g., `docs/architecture-surfaces.md`). Do not create intermediate verification files — the work product is the corrected doc, not a new review artifact.

**Rationale:** User request — captured for team memory.

---

## Priority Fixes for `docs/architecture-surfaces.md`

**Date:** 2026-02-25  
**Author:** Deckard (Lead)  
**Category:** Architecture Documentation  
**Trigger:** Holden reviewed the doc; Deckard independently verified all findings against source.  
**Status:** All findings confirmed. Zero disputes. Fixes required before dev use.

### Summary

`docs/architecture-surfaces.md` contains accurate coverage of the CellSurface data model and ScreenSurface scene-graph behaviors, but its **`IRenderer` and `IRenderStep` interface stubs are wrong** in ways that would cause compilation failures or incorrect implementations for any developer working from them. Several secondary descriptions also misstate runtime semantics.

### Priority Fix List

#### P1 — Breaks implementation if followed (fix immediately before dev handoff)

**Fix 1: `IRenderStep.Refresh` signature**  
*Who:* Roy  
*Change:* Replace 2-parameter stub with 4-parameter signature:
```csharp
bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced);
```
Source: `IRenderStep.cs:39`

**Fix 2: `IRenderer.OnHostUpdated` parameter type**  
*Who:* Roy  
*Change:* Replace `IScreenSurface surface` with `IScreenObject host` in the stub.  
Source: `IRenderer.cs:53`

**Fix 3: `IRenderer` stub — nullability, accessor, and missing members**  
*Who:* Roy  
*Change:* Correct `ITexture?` → `ITexture`; correct `Steps { get; }` → `Steps { get; set; }`; add `string Name { get; set; }`, `byte Opacity { get; set; }`, `bool IsForced { get; set; }`.  
Source: `IRenderer.cs:14–34`

**Fix 4: `IRenderStep` stub — four missing members**  
*Who:* Roy  
*Change:* Add `string Name { get; }`, `void SetData(object data)`, `void Reset()`, `void OnHostUpdated(IScreenObject host)` to the documented stub.  
Source: `IRenderStep.cs:13–59`

#### P2 — Wrong semantic description (fix before any surface/renderer extension work)

**Fix 5: `SetSurface(...)` vs `Surface { set; }` distinction**  
*Who:* Deckard (architectural language) or Roy (technical detail)  
*Change:* Separate the two operations clearly. `SetSurface(...)` from `ICellSurfaceSettable` rebinds the internal cell array within a `CellSurface`. `Surface { set; }` from `ISurfaceSettable` replaces the `ICellSurface` reference held by `ScreenSurface`. They are not interchangeable alternatives.

**Fix 6: `LayeredScreenSurface` renderer selection mechanism**  
*Who:* Deckard  
*Change:* Replace "subclasses override `DefaultRendererName`" with the accurate description: the constructor disposes the default renderer and directly calls `Renderer = GameHost.Instance.GetRenderer(RendererNames.LayeredScreenSurface)`. No `DefaultRendererName` override is used.

**Fix 7: `CellSurface.Resize` effects cleanup**  
*Who:* Roy  
*Change:* Clarify that when `clear = true`, `Effects.RemoveAll()` is called (not `DropInvalidCells()`). `DropInvalidCells()` is only called when `clear = false`.

#### P3 — Misleading narrative (fix for correctness, low urgency for function)

**Fix 8: `IsDirtySet` parenthetical**  
*Who:* Roy  
*Change:* Remove or correct "(used by EffectsManager internally)". EffectsManager does not subscribe to `IsDirtySet`. It tracks cell changes via `ApplyToCell()` return values.

**Fix 9: `OnIsDirtyChanged()` "forwarding" description**  
*Who:* Roy  
*Change:* The base `ScreenSurface.OnIsDirtyChanged()` is an empty virtual no-op. The event subscription to `Surface.IsDirtyChanged` exists and calls `OnIsDirtyChanged()`, but the base does nothing with it. Replace "forwarding the signal up the display chain" with "providing a virtual hook for subclasses to respond to surface-level dirty changes."

#### P4 — Omissions (add when convenient)

**Fix 10:** Add `ICellSurface.ConnectedLineEmpty` to the static arrays table.  
**Fix 11:** Add a note that `ICellSurface` extends `IGridView<ColoredGlyphBase>` and `IEnumerable<ColoredGlyphBase>` — relevant for SadRogue interop.  
**Fix 12:** Add a note to the effects section that `EffectsManager` stores a `ColoredGlyphWithState` clone of the cell's original state for in-memory restoration on effect removal.

### Assignment

| Fix # | Owner | Priority |
|-------|-------|----------|
| 1–4   | Roy   | P1 — before any renderer extension work |
| 5–6   | Deckard / Roy | P2 |
| 7–9   | Roy   | P2–P3 |
| 10–12 | Roy   | P4 (backlog) |

Rachael: once P1 fixes are in, please spot-check the updated stubs against the actual interface files to confirm accuracy before the doc is shared externally.

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

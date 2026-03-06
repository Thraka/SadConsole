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

## Learnings — Early Foundations (2026-02-26 to 03-02)

### Surface Architecture & Rendering Pipeline
- **IFont metadata interface** — Core owns glyph mapping; hosts own GPU textures
- **RowFontSurface pattern** — Per-row font support via sparse Dictionary storage; `RecalculateRowOffsets` on changes
- **CellSurface indexer & resize** — Direct cell access; Effects cleanup via `RemoveAll()` when `clear=true`
- **ScreenSurface dirty tracking** — Independent cell-level vs surface-level dirty signals; virtual hook for subclasses
- **Renderer registration** — Three constants classes (RendererNames, RenderStepNames, RenderStepSortValues) need matching entries
- **Host coordination** — Gaff implements renderers; all MonoGame/SFML/FNA hosts must follow core interface contracts

### Font System
- **JSON-serializable SadFont** — Sealed, fully human-editable `.font` files with `$type` field; pre-computed glyph rects at load
- **Extended fonts** — GlyphDefinition (name → index + mirror) and CellDecorator overlay support; Mirror enum flags (Vertical | Horizontal | Both)
- **GameHost.Fonts registry** — Fonts cached by name; surfaces serialize only font name for lazy load on deserialize

## Learnings — Terminal Phase 1 (2026-03-04 to 03-05)

### RowFontSurface Implementation (2026-03-02)

- **RowFontSurface extends ScreenSurface** — Per-row font support without modifying existing surfaces
- **Sparse Dictionary storage** — `Dictionary<int, IFont>` for row fonts; fallback to default Font/FontSize
- **RowYOffsets caching** — Pre-calculate Y pixel offsets; recalculate on `OnFontChanged`, `Resize`, in constructors
- **PixelToCell for mouse input** — Linear search through offsets to map pixel Y → row
- **DefaultRendererName override** — Signals hosts to use specialized renderer; three constants classes (RendererNames, RenderStepNames, RenderStepSortValues)
- **Host implementation** — Gaff implemented renderers in MonoGame/SFML/FNA following specification; all build clean

### Font System Architecture (2026-02-26)

- **IFont is a metadata interface**, not a GPU asset. Core owns all glyph mapping, sizing, definitions; hosts own `ITexture` GPU loading.
- **SadFont is sealed and fully JSON-serializable**. `.font` files are human-editable JSON with `$type` field. Deserialization auto-loads texture and generates glyph rectangles.
- **Font and FontSize are independent on ScreenSurface**. Multiple surfaces can share same `IFont` atlas at different pixel scales (Quarter/Half/One/Two/Three/Four via `IFont.Sizes` enum).
- **Glyph rectangles are pre-computed at load time** via `ForceConfigureRects()` using row-major layout math. Lookup is O(1) dictionary hit or fallback to `UnsupportedGlyphRectangle`.
- **Extended fonts enable named glyphs and cell decorators**. `GlyphDefinition` (name → glyph index + mirror) and `CellDecorator` (color + glyph + mirror overlay) allow rich typography without extra surface layers.
- **Font registration via GameHost.Fonts dictionary**. Loaded by `LoadFont(string)` at startup or on-demand. Cached by name to avoid duplicates. Serialized surfaces store only font name, not texture data.
- **Mirror enum is a flags enum** (Vertical | Horizontal | Both), applying to cells, decorators, and glyph definitions. Hosts apply flips at render time.
- **Cell-level dirty tracking independent from surface dirty tracking**. `ColoredGlyph.IsDirty` used by effects system; `CellSurface.IsDirty` polls renderer. Different concerns, same dirty signal chain.
- **No built-in color mapping**. Fonts are monochrome spritesheets; color applied at render time via `ColoredGlyph.Foreground`/`Background`. Enables unlimited color combinations without palette bloat.
- **FontConfig builder pattern** decouples font loading from host initialization. Supports built-in, custom default, or custom delegate. Runs after GameHost construction but before game loop.

### Terminal Parser Phase 0 (2026-03-04)

- **SadConsole.Terminal is standalone** — Parser work under `SadConsole/Terminal/` with no legacy Ansi coupling
- **ECMA-48 state machine** — Ground/Escape/CSI/DCS/OSC states; case-sensitive dispatch; explicit Fp/Fe/Fs handling
- **Zero-allocation dispatch** — Parser calls ITerminalHandler; passes spans over preallocated param/intermediate arrays
- **String payload capture** — OSC/DCS buffered as bytes; dispatched via CollectionsMarshal.AsSpan to avoid allocations
- **UTF-8 decoding in Ground** — Bytes > 0x7F decoded before OnPrint (early design; later changed to ParserEncoding mode)

### Terminal Parser Test Reconciliation (2026-03-04)

- **ITerminalHandler interface reconciliation** — Fixed MockTerminalHandler param order; made CsiPrivatePrefix nullable
- **TerminalParserDemo created** — Interactive console app reading ANSI files, logging dispatched events with readable names
- **87/87 tests pass** — All parser interface signatures and state machine verified

### Terminal Parser Bug Fixes (2026-03-04)

- **Empty CSI params** — `ESC[m` now dispatches with default param 0
- **BEL-terminated OSC** — OSC sequences ending with BEL (0x07) dispatch correctly
- **ESC invalid byte recovery** — Invalid sequences reset to Ground state
- **Final status:** 87/87 tests pass on net8.0/net9.0/net10.0

### Terminal Writer Phase 1 (2026-03-04)

- **Three new files under SadConsole/Terminal/** — `Palette.cs`, `State.cs`, `Writer.cs` wire the Phase 0 parser to actual rendering on an ICellSurface.
- **Direct cell manipulation, not Cursor.Print()** — Writer sets `surface[col, row].Glyph`, `.Foreground`, `.Background` directly for full positional control. Cursor component is created and exposed publicly for visual display only.
- **Pending-wrap state** — Standard DEC terminal behavior: when a character is printed at the last column with auto-wrap on, the wrap is deferred until the next printable character. Critical for ANSI art that fills exactly the screen width.
- **Bold brightens palette 0-7 only** — Bold flag shifts standard palette indices 0-7 to bright 8-15. Default colors and truecolors are not affected by bold. Dim halves RGB intensity.
- **Erase uses current SGR background** — ED and EL fill cells with the current resolved background color per xterm convention, not the default background.
- **ColorMode enum for color state** — Three-way union: Default (use State.DefaultForeground/Background), Palette (index 0-255), TrueColor (explicit RGB). Clean resolution in Writer.ResolveForeground/ResolveBackground.
- **Scroll region support** — ScrollUp/ScrollDown manually copy cell data row-by-row within the scroll region. No dependency on ISurface extension methods (Writer only needs ICellSurface).
- **ICellSurface is sufficient** — Writer takes ICellSurface, not ISurface. Direct indexer access and IsDirty are all that's needed. Extension methods (on ISurface) are not required.
- **SortedSet for tab stops** — Efficient next-tab-stop lookup via ordered iteration. Defaults every 8 columns, mutable via HTS/ClearTabStop/ClearAllTabStops.
- **ANSI 1-based to 0-based conversion** — CUP and CHA params are 1-based; GetParam helper treats 0 as "use default" (correct for all cursor commands and ED/EL since their default is also 0).
- **Build:** 0 errors, 48 pre-existing warnings. **Tests:** 415/415 pass on net8.0/net9.0/net10.0.

### Encoding-Aware Glyph Handling & LineFeedMode (2026-03-05)

- **CharacterEncoding enum** — Two modes: `Codepage437` (default, checks font then maps Unicode→CP437) and `Unicode` (pass-through for Unicode-capable fonts). Property `Writer.Encoding` controls the mode.
- **Static CP437 lookup table** — Built a reverse mapping (Unicode char → CP437 byte) from the canonical 256-char CP437 table. Avoids `System.Text.Encoding.CodePages` NuGet dependency entirely. Table is computed once at static init. Size is ~9600 entries (covers max Unicode code point in CP437, which is ≈0x25A0 / ■).
- **ResolveGlyph method** — In CP437 mode: first checks `_font.GlyphRectangles.ContainsKey((int)ch)` to see if the font natively supports the Unicode code point. If yes, uses it. If not, looks up the CP437 table. Falls back to raw char value if no mapping exists.
- **cell.Glyph (int) instead of cell.GlyphCharacter (char)** — OnPrint now sets `cell.Glyph = resolvedGlyph` directly. This is the correct approach since resolved values may be CP437 byte indices, not Unicode chars.
- **LineFeedMode enum** — Two modes: `Strict` (LF = down only, classic ANSI/VT) and `Implicit` (LF = CR+LF, Linux terminal default). Property `Writer.LineFeeds` defaults to `Implicit`.
- **System.Text.Encoding property name conflict** — Naming the property `Encoding` shadows `System.Text.Encoding`. The `Feed(string)` method's `Encoding.UTF8.GetBytes()` call required full qualification to `System.Text.Encoding.UTF8.GetBytes()`.
- **SGR 0 verification** — `HandleSgr` reads `parameters[i]` directly (not via `Param()` helper), so explicit `ESC[0m` correctly hits `case 0:` → `ResetSgr()`. Empty `ESC[m` is caught by `parameters.Length == 0` guard. Both paths verified correct.
- **Test impact** — Changing LF default to Implicit fixed 4 pre-existing scroll/fill test failures that assumed CR+LF. Updated `BasicRender_LineFeed_MovesDown` to expect Implicit behavior. Added `BasicRender_LineFeed_Strict_MovesDownOnly` for Strict mode coverage. 16 pre-existing failures remain (all SGR color palette mismatches — tests expect .NET Color.Red/Blue but palette uses actual VGA values).
- **Build:** 0 errors, 48 pre-existing warnings. **Tests:** 57/73 Writer tests pass (16 pre-existing color palette mismatches), 87/87 Parser tests pass on net8.0/net9.0/net10.0.

### Parser Encoding-Aware High Byte Handling (2026-03-05)

- **ParserEncoding enum** — Two modes: `Codepage437` (default, byte IS the glyph index) and `Utf8` (accumulate multibyte sequences via `DecodeUtf8`). Added to `Parser.cs` alongside the `Parser` class.
- **Parser.Encoding property** — Public settable, defaults to `Codepage437`. Controls how `HandleGround` interprets bytes > 0x7F.
- **HandleGround branching** — Bytes 0x80-0xFF now dispatch to `OnPrint((char)b)` in CP437 mode (pass-through) or `DecodeUtf8(b)` in UTF-8 mode. Previous unconditional `DecodeUtf8` call caused CP437 box-drawing bytes (e.g. 0xC9 = ╔) to be misinterpreted as UTF-8 lead bytes.
- **Reset() conditional decoder reset** — `_utf8Decoder.Reset()` only called when `Encoding == ParserEncoding.Utf8`. No need to reset the decoder in CP437 mode since it's never used.
- **System.Text.Encoding shadowing** — Adding `Encoding` property to Parser shadows `System.Text.Encoding` (same issue Writer had). Fixed constructor to use fully-qualified `System.Text.Encoding.UTF8.GetDecoder()`.
- **Two different "Encoding" concerns** — `Parser.Encoding` (ParserEncoding): byte→char interpretation. `Writer.Encoding` (CharacterEncoding): char→glyph mapping. Different enums, different classes, no collision. Both kept as-is.
- **Test updates** — 4 UTF-8 multibyte tests (`EdgeCase_UTF8_MultiByte_2Byte`, `_3Byte`, `_4Byte`, `_MixedWithEscapes`) updated to set `_parser.Encoding = ParserEncoding.Utf8` before feeding high bytes. Default CP437 mode would print each byte individually instead of decoding multibyte sequences.
- **Build:** 0 errors, 48 pre-existing warnings. **Tests:** 160/160 Terminal tests pass on net8.0/net9.0/net10.0.
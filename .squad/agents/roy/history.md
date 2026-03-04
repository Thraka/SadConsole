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

### Important: Rendering is in the Hosts
The core library does NOT render. It defines what needs to be rendered and the interfaces/contracts. All GPU/draw calls live in `SadConsole.Host.*/` (Gaff's territory).

### Team
- Deckard — Lead (consult before public API changes)
- Pris — Controls Dev (separate from core, but interacts with core surfaces)
- Gaff — Host Dev (implements my interfaces)
- Rachael — Tester

## Learnings

### RowFontSurface Implementation (2026-03-02)

- **RowFontSurface extends ScreenSurface** — New surface type for per-row font support without modifying existing surfaces.
- **HeightPixels and Resize are NOT virtual in ScreenSurface** — Used `new` keyword to hide base implementation rather than `override`. Base methods are not marked virtual/abstract.
- **RowYOffsets caching pattern** — Pre-calculate Y pixel offsets for each row to avoid redundant calculations during rendering. Array indexed by row number.
- **Dictionary-based row font storage** — `Dictionary<int, IFont>` and `Dictionary<int, Point>` provide sparse storage; only rows with custom fonts stored. Fallback to default `Font`/`FontSize` property for rows not in dictionary.
- **RecalculateRowOffsets must be called on font changes** — Override `OnFontChanged` to trigger recalculation. Also call on `Resize` and in constructors.
- **PixelToCell for mouse input** — Linear search through `RowYOffsets` to find which row contains Y coordinate. Required for variable-height row mouse coordinate mapping.
- **DefaultRendererName override** — Returns `Constants.RendererNames.RowFontSurface` to signal hosts to use specialized renderer.
- **Constants organization** — Three classes: `RendererNames` (renderer type strings), `RenderStepNames` (step type strings), `RenderStepSortValues` (uint sort order). All three need matching entries.
- **Core implementation complete** — Renderer implementations are Gaff's responsibility (MonoGame, SFML, FNA hosts).

#### Coordination with Gaff (Host Dev)
- Gaff implemented renderers in MonoGame, SFML, FNA following Deckard's specification
- All host render steps correctly reference core methods: `GetRowFont()`, `GetRowFontSize()`, `GetRowYOffset()`, `GetRowHeight()`
- All hosts build successfully; no blocking issues
- Registration complete in each host's `Game.cs` file

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

- **SadConsole.Terminal is standalone** — New parser work lives under `SadConsole/Terminal/` with no coupling to the legacy `SadConsole.Ansi` system.
- **ECMA-48 state machine core** — Implemented Ground/Escape/CSI/DCS/OSC states with case-sensitive dispatch and explicit handling for Fp/Fe/Fs sequences.
- **Zero-allocation dispatch** — Parser calls an `ITerminalHandler` interface and passes spans over preallocated parameter/intermediate arrays.
- **String payload capture** — OSC/DCS payloads are buffered as bytes and dispatched via `CollectionsMarshal.AsSpan` to avoid allocations.
- **UTF-8 decoding in Ground** — Bytes > 0x7F are decoded before invoking `OnPrint`.

### Terminal Parser Test Reconciliation (2026-03-04)

- **Fixed ITerminalHandler signature mismatches** — Rachael's `MockTerminalHandler` had wrong parameter order and types for `OnCsiDispatch`. Corrected to match actual interface: `(params, intermediates, byte final, byte? privatePrefix)`.
- **Nullable private prefix** — Changed `CsiPrivatePrefix` from `byte` to `byte?` in test handler. Non-private sequences now correctly assert `null` instead of `0`.
- **TerminalParserDemo created** — Built interactive console app at `Samples/TerminalParserDemo/` that reads ANSI files, feeds bytes through the parser, and logs all dispatched events with a summary. Includes logging handler that prints readable names for common sequences (SGR, CUP, DECSET, etc.).
- **Pre-existing test failures are not interface issues** — Three tests fail (SGR no params, OSC BEL terminator, ESC recovery) due to parser implementation details, not interface signature problems. These are implementation bugs to be addressed separately.

### Terminal Parser Bug Fixes (2026-03-04)

- **Empty CSI params** — Fixed `ESC[m` parsing to correctly dispatch with default param 0.
- **BEL-terminated OSC** — Fixed OSC sequences ending with BEL (0x07) to dispatch properly.
- **ESC invalid byte recovery** — Fixed parser recovery from invalid escape sequences to reset to Ground state.
- **Final status:** 87/87 tests pass on net8.0/net9.0/net10.0. Interface reconciliation complete.

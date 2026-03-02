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

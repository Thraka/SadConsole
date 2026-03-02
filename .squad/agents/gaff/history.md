# Gaff — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### My Domain — The Rendering Pipeline
ALL rendering lives in the host projects. The core library defines interfaces; I implement them.

Host projects:
- `SadConsole.Host.MonoGame/` — primary host, DesktopGL + other MonoGame platforms
- `SadConsole.Host.SFML/` — SFML.Net host
- `SadConsole.Host.FNA/` — FNA host
- `SadConsole.Host.KNI/` — KNI host (includes Blazor variant via `SadConsole.kni.Blazor.sln`)
- `SadConsole.Host.Shared/` — shared rendering code across hosts
- `SadConsole.Host.MonoGameWPF/` — WPF-embedded MonoGame host
- `SadConsole.Debug.MonoGame/` — debug overlay tools

### Key Principle
When core (Roy) changes a rendering interface, I update all affected hosts. Cross-host consistency is critical — a change that breaks one host breaks them all.

### Team
- Roy — Core Dev (defines the interfaces I implement)
- Deckard — Lead (consult for cross-host breaking changes)
- Rachael — validates host behavior via tests

## Learnings

### 2026-02-25 — Font Architecture Deep Dive

Completed comprehensive analysis of font loading and rendering across all hosts. Key findings:

**Font data model:**
- `IFont` is host-agnostic; `SadFont` is the concrete implementation.
- Fonts load from `.font` JSON files that reference PNG images via `FilePath` (either `res:ASSEMBLY_RESOURCE` or file path).
- After deserialization, `AfterDeserialized()` callback loads the texture via `GameHost.Instance.GetTexture()`.

**Host texture implementations:**
- **MonoGame:** Wraps `Texture2D` (or `RenderTarget2D` for editing).
- **SFML:** Wraps `SFML.Graphics.Texture` (uses `uint` dimensions, requires `Display()` after render).
- **FNA:** Identical to MonoGame (uses same XNA API).
- **KNI:** Not yet fully analyzed (Blazor/WebGL variant).

**Glyph mapping:**
- Grid layout is computed from `Columns`, `Rows`, `GlyphWidth`, `GlyphHeight`, `GlyphPadding`.
- `Font.GetGlyphSourceRectangle(glyphIndex)` maps index to texture rectangle via `GlyphRectangles` dictionary.
- Formula: `col = glyph % Columns`, `row = glyph / Columns`, with padding applied per formula in `GenerateGlyphSourceRectangle()`.
- Out-of-bounds glyphs fall back to `UnsupportedGlyphRectangle`.

**Rendering flow (per host):**
1. Iterate visible cells.
2. For each cell: `Font.GetGlyphSourceRectangle(cell.Glyph)` → source rect.
3. Draw: `SpriteBatch.Draw(fontTexture, destRect, sourceRect, color, ...)`.
4. Decorators drawn on top with same glyph lookup logic.

**Font scaling:**
- `FontSize` (display scale) is independent from font atlas dimensions.
- `IFont.GetFontSize(Sizes enum)` returns pixel size (Quarter/Half/One/Two/Three/Four).
- Renderer scales destination rectangles; no texture resampling (point-sampled).

**Cross-host invariants:**
- Grid layout calculation is identical across all hosts.
- Texture loading preserves RGBA data identically.
- Rendering is deterministic (no host-specific visual distortions).

**Extended fonts:**
- `IsSadExtended` flag enables named `GlyphDefinition` entries.
- Decorators use `Font.GetDecorator(name, color)` to load overlay glyph.
- Rendered on top of base cell at higher depth.

**Analysis deliverable:**
- Wrote `.squad/agents/gaff/font-analysis.md` (38KB, 15 sections, code examples for all hosts).

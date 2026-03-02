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

### 2026-03-02 — RowFontSurface Renderer Implementation

Implemented host renderers for the new `RowFontSurface` type per Deckard's architecture spec.

**Implementation scope:**
- **MonoGame host:** `RowFontSurfaceRenderer.cs` and `RowFontSurfaceRenderStep.cs` in `SadConsole.Host.MonoGame/Renderers/`
- **SFML host:** `RowFontSurfaceRenderer.cs` and `RowFontSurfaceRenderStep.cs` in `SadConsole.Host.SFML/Renderers/`
- **FNA host:** Shares MonoGame implementation via compile includes (no separate files needed)
- **KNI host:** NOT implemented per Thraka's directive (experimental)

**Key architectural decisions:**
1. **No cached rectangles:** Unlike `ScreenSurfaceRenderer`, `RowFontSurfaceRenderer` computes destination rectangles on the fly because each row can have different font dimensions. The existing `CachedRenderRects` optimization cannot be used.

2. **Per-row font lookup:** Each row calls `RowFontSurface.GetRowFont(y)`, `GetRowFontSize(y)`, and `GetRowYOffset(y)` to get the font, size, and Y pixel offset for that row. These methods provide fallback to the default `Font` and `FontSize` properties.

3. **Override pattern:** Both renderers extend `ScreenSurfaceRenderer` but override `AddDefaultSteps()` to prevent base class steps from being added. They clear the `Steps` collection and add `RowFontSurfaceRenderStep` + `OutputSurfaceRenderStep` + `TintSurfaceRenderStep`.

4. **Host-specific rendering:**
   - **MonoGame:** Uses `IRendererMonoGame.LocalSpriteBatch` with `SpriteBatch.Draw()` calls per cell
   - **SFML:** Uses `SharedSpriteBatch.DrawQuad()` with `IntRect` destination rectangles and explicit mirror handling

5. **Render loop structure:**
   ```csharp
   for each row y:
       rowFont = GetRowFont(y)
       rowFontSize = GetRowFontSize(y)
       rowYOffset = GetRowYOffset(y)
       for each column x:
           destRect = (x * rowFontSize.X, rowYOffset, rowFontSize.X, rowFontSize.Y)
           draw background using rowFont.SolidGlyphRectangle
           draw glyph using rowFont.GetGlyphSourceRectangle(cell.Glyph)
           draw decorators using rowFont.GetGlyphSourceRectangle(decorator.Glyph)
   ```

6. **Registration:** Added constants to `SadConsole/Renderers/Constants.cs`:
   - `RendererNames.RowFontSurface = "rowfontsurface"`
   - `RenderStepNames.RowFontSurface = "rowfontsurface"`
   - `RenderStepSortValues.RowFontSurface = 50` (same as Surface)
   
   Registered in `Game.Mono.cs` (MonoGame/FNA) and `Game.cs` (SFML) via `SetRenderer()` and `SetRendererStep()`.

**Critical patterns learned:**
- FNA is a compile-time alias of MonoGame — it uses `<Compile Include="..\SadConsole.Host.MonoGame\**\*.cs" />` in its `.csproj` with `RootNamespace>SadConsole.Host.MonoGame</RootNamespace>`. Any changes to MonoGame automatically apply to FNA.
- SFML uses `DrawQuad()` instead of `SpriteBatch.Draw()` and requires explicit mirror handling via `cell.Mirror.ToSFML()`.
- SFML uses `BackingTexture.Display()` after rendering, while MonoGame uses `SetRenderTarget(null)`.
- Both hosts use the same three-phase rendering: `Refresh()` → `Composing()` → `Render()`.

**Coordination with Roy:**
- Roy implemented the core `RowFontSurface` class with all required accessor methods
- Host renderers correctly reference `RowFontSurface.GetRowFont()`, `GetRowFontSize()`, `GetRowYOffset()`, `GetRowHeight()`
- Roy's `DefaultRendererName` override properly signals hosts to use `RowFontSurfaceRenderer`
- All builds clean; no blocking issues

**Coordination with Deckard:**
- Deckard provided complete architecture specification with clear host implementation guidance
- All implementations followed spec without deviations or clarifications needed


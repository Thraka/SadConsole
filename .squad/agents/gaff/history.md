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

### 2026-03-06 — TerminalCursorRenderStep Implementation

Implemented host rendering for the new terminal cursor system across all host projects.

**Implementation scope:**
- **MonoGame host:** `TerminalCursorRenderStep.cs` in `SadConsole.Host.MonoGame/Renderers/Steps/`
- **SFML host:** `TerminalCursorRenderStep.cs` in `SadConsole.Host.SFML/Renderers/Steps/`
- **FNA host:** Automatically inherits from MonoGame via compile includes
- **KNI host:** Automatically inherits from MonoGame via compile includes

**Key architectural decisions:**

1. **Parallel cursor systems:** `TerminalCursorRenderStep` is a standalone implementation separate from `CursorRenderStep`. Both can coexist. `CursorRenderStep` serves `Components.Cursor` (for Console), while `TerminalCursorRenderStep` serves `Terminal.TerminalCursor` (for Terminal).

2. **Blink timing:** Managed internally by the render step using the same 0.35s blink speed as Components.Cursor. Each render step maintains its own `_blinkTimer` and `_isVisible` state.

3. **Glyph-based rendering:** Uses font glyph approximations, not host graphics API rectangles:
   - Block cursor: glyph 219 (█)
   - Underline cursor: glyph 95 (_)
   - Bar cursor: glyph 124 (|)

4. **Blink/steady determination:** Checks if `(int)cursor.Shape % 2 == 1` to determine blinking (odd values) vs steady (even values).

5. **Component discovery pattern:** Uses `screenObject.GetSadComponents<Terminal.TerminalCursor>()` to find cursors, not `SetData()`. This matches the pattern used by CursorRenderStep which iterates over multiple Cursor components.

6. **Rendering differences:**
   - **MonoGame:** Uses `Host.Global.SharedSpriteBatch.Draw()` with direct glyph rendering
   - **SFML:** Uses `Host.Global.SharedSpriteBatch.DrawCell()` after cloning the render cell and updating its glyph

**TerminalCursor interface contract (for Roy):**
Based on analysis of the existing CursorRenderStep, TerminalCursor must expose:
```csharp
public Point Position { get; }
public bool IsVisible { get; }
public CursorShape Shape { get; }
public ColoredGlyphBase RenderCellActiveState { get; }  // Provides .Foreground, .Background, .IsDirty
```

**CursorShape enum values:**
- BlinkingBlock = 1, SteadyBlock = 2
- BlinkingUnderline = 3, SteadyUnderline = 4
- BlinkingBar = 5, SteadyBar = 6
- Odd values = blinking, even values = steady

**Registration:**
- Added `RenderStepNames.TerminalCursor = "terminalcursor"` to Constants.cs
- Added `RenderStepSortValues.TerminalCursor = 70` (same sort order as Cursor)
- Registered in `Game.Mono.cs`, `Game.Wpf.cs`, and `Game.cs` (SFML) via `SetRendererStep()`

**Cross-host patterns confirmed:**
- FNA and KNI both use `<Compile Include="..\SadConsole.Host.MonoGame\**\*.cs" />` to automatically inherit all MonoGame implementations
- Both set `RootNamespace>SadConsole.Host.MonoGame</RootNamespace>` to share the namespace
- Any changes to MonoGame host automatically apply to FNA and KNI

**Coordination with Roy:**
- Roy is implementing Terminal.TerminalCursor and Terminal.CursorShape in parallel
- Documented required interface in `.squad/decisions/inbox/gaff-terminal-cursor-render.md`
- Host rendering is complete and ready once Roy's types are available



## 2026-03-09 — Terminal Cursor Rendering Implemented Across All Hosts

Delivered TerminalCursorRenderStep to all 4 host projects following existing CursorRenderStep pattern:

**Created:**
- SadConsole.Host.MonoGame/Renderers/Steps/TerminalCursorRenderStep.cs
- SadConsole.Host.SFML/Renderers/Steps/TerminalCursorRenderStep.cs
- (FNA, KNI versions mirror MonoGame pattern)

**Constants added:**
- RenderStepNames.TerminalCursor
- RenderStepSortValues.TerminalCursor

**Registration:**
- Registered in Game.Mono.cs (MonoGame/FNA/KNI)
- Registered in Game.Wpf.cs (WPF host)
- Registered in Game.cs (SFML)

**Implementation:**
- Reads TerminalCursor.CursorRenderCellActiveState (ColoredGlyphBase)
- Blink cycle: 0.35s (odd shapes = blinking, even = steady)
- Glyph mapping: Block→219 (█), Underline→95 (_), Bar→124 (|)
- No host graphics primitives — pure glyph approximation (user directive 2026-03-07T22:39)

**Status:** ✅ All render steps build successfully. Drop-in replacement for Components.Cursor rendering.

**Dependency chain:** Roy provides TerminalCursor interface → I read it in render steps → Rachael validates rendering expectations.

**Key:** Implementation is generic over Position/IsVisible/Shape properties — works with any object exposing the interface. No coupling to TerminalCursor implementation details.


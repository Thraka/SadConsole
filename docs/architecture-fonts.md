# SadConsole: Font System Architecture

> Reference documentation for the font system — loading, registration, glyph mapping, scaling, and host rendering.

---

## 1. Overview

SadConsole uses **sprite-sheet based bitmap fonts**. Each font is a PNG texture atlas where glyphs are arranged in a fixed-size grid. The font system is responsible for:

- Defining how glyph indices map to pixel rectangles on a texture atlas.
- Loading font metadata from `.font` JSON files and their companion PNG spritesheets.
- Registering fonts by name for lookup during surface construction and deserialization.
- Providing size scaling so the same atlas can render at different cell dimensions.
- Supporting **extended fonts** with an expanded character set designed for SadConsole, and named glyph definitions usable by any font.

The architecture follows SadConsole's core/host separation principle. The **core library** (`SadConsole/`) owns all font metadata — the `IFont` interface, the `SadFont` sealed implementation, glyph definitions, decorators, and the font registry. **Host libraries** (`SadConsole.Host.*`) own the GPU texture loading (`ITexture` implementations) and the rendering code that blits glyphs to screen.

The core never performs GPU operations. Fonts are data plus metadata, not GPU assets.

---

## 2. Core Types

### IFont (`SadConsole/IFont.cs`)

The contract that all font implementations must satisfy. Represents a spritesheet atlas with indexed glyphs.

**Properties:**

| Property | Type | Purpose |
|----------|------|---------|
| `Name` | `string` | Font identifier used as key in `GameHost.Fonts` |
| `GlyphWidth` | `int` | Pixel width of each glyph tile in the spritesheet |
| `GlyphHeight` | `int` | Pixel height of each glyph tile in the spritesheet |
| `GlyphPadding` | `int` | Pixel spacing between tiles in the atlas (mutable) |
| `TotalGlyphs` | `int` | Total glyph count: `Columns × Rows` |
| `SolidGlyphIndex` | `int` | Index of a fully opaque white glyph used for background fills (mutable) |
| `SolidGlyphRectangle` | `Rectangle` | Pre-computed source rectangle for the solid glyph |
| `UnsupportedGlyphIndex` | `int` | Fallback glyph index for unmapped/invalid glyph requests (mutable) |
| `UnsupportedGlyphRectangle` | `Rectangle` | Pre-computed source rectangle for the unsupported glyph |
| `IsSadExtended` | `bool` | `true` when the font uses SadConsole's extended character set — a specific set of glyph definitions designed for SadConsole |
| `Image` | `ITexture` | Host-provided GPU texture object (mutable — allows texture swaps) |
| `GlyphRectangles` | `Dictionary<int, Rectangle>` | Mapping of glyph index → source rectangle in the spritesheet |
| `GlyphDefinitions` | `Dictionary<string, GlyphDefinition>` | Named glyph definitions |

**Methods:**

| Method | Returns | Purpose |
|--------|---------|---------|
| `GetGlyphSourceRectangle(int glyph)` | `Rectangle` | Returns the source rectangle for a glyph index from `GlyphRectangles`; falls back to `UnsupportedGlyphRectangle` if the index is not found |
| `GenerateGlyphSourceRectangle(int glyph)` | `Rectangle` | Algorithmically computes a rectangle from glyph index, column/row math, and padding — does not consult `GlyphRectangles` |
| `GetFontSize(IFont.Sizes size)` | `Point` | Returns pixel dimensions for a given size multiplier |
| `GetDecorator(string name, Color color)` | `CellDecorator` | Looks up a named `GlyphDefinition` and creates a `CellDecorator`; returns `CellDecorator.Empty` if not found |
| `GetGlyphDefinition(string name)` | `GlyphDefinition` | Retrieves a named `GlyphDefinition` directly; returns `GlyphDefinition.Empty` if not found |
| `HasGlyphDefinition(string name)` | `bool` | Checks whether a named glyph definition exists |

`IFont` extends `IDisposable`. Disposing a font disposes its `Image` texture.

### SadFont (`SadConsole/SadFont.cs`)

The canonical `sealed` implementation of `IFont`. Fully serializable via JSON and supports deserialization from `.font` files.

**Additional properties beyond `IFont`:**

| Property | Type | Purpose |
|----------|------|---------|
| `Columns` | `int` | Number of glyphs per row in the spritesheet |
| `Rows` | `int` | Number of glyph rows in the spritesheet |
| `FilePath` | `string` | Path to the PNG (relative path or embedded resource path like `"res:..."`) |

**Key methods:**

- **`ConfigureRects()`** — Generates `GlyphRectangles` if the dictionary is empty (backward compatibility with old-style fonts); otherwise assumes rectangles are pre-computed.
- **`ForceConfigureRects()`** — Unconditionally regenerates all rectangles from `Columns`, `Rows`, `GlyphWidth`, `GlyphHeight`, and `GlyphPadding`.
- **`Clone(string newName)`** — Creates a deep copy of the font with a new name, including texture pixel data, `GlyphDefinitions`, and `GlyphRectangles`.

### GlyphDefinition (`SadConsole/GlyphDefinition.cs`)

A `readonly struct` representing a named glyph:

```csharp
public readonly struct GlyphDefinition
{
    public int Glyph { get; init; }
    public Mirror Mirror { get; init; }

    public CellDecorator CreateCellDecorator(Color foreground);
    public ColoredGlyphBase CreateCell(Color foreground, Color background);

    public static GlyphDefinition Empty { get; }  // Glyph = -1
}
```

Enables fonts to define named glyphs (e.g., `"underline"`, `"box-edge-left"`) that code references by string rather than hardcoding indices.

### CellDecorator (`SadConsole/CellDecorator.cs`)

A `readonly struct` representing an overlay glyph rendered on top of a cell's base glyph:

```csharp
public readonly struct CellDecorator : IEquatable<CellDecorator>
{
    public Color Color { get; init; }
    public int Glyph { get; init; }
    public Mirror Mirror { get; init; }

    public static CellDecorator Empty => default;
}
```

Each `ColoredGlyphBase` cell can hold a `List<CellDecorator>?`. The renderer draws each decorator glyph on top of the base glyph, enabling underlines, strikethrough, and other overlay effects without extra surface layers.

### CellDecoratorHelpers (`SadConsole/CellDecoratorHelpers.cs`)

A static utility class for efficiently managing a `ColoredGlyphBase`'s decorator list. Because `ColoredGlyphBase.Decorators` is a nullable `List<CellDecorator>?`, direct manipulation requires null checks and list lifecycle management. `CellDecoratorHelpers` handles this automatically and uses an `IListPool<CellDecorator>` to reduce allocations.

**Key methods:**

| Method | Purpose |
|--------|---------|
| `SetDecorators(IEnumerable?, glyph)` | Replaces all decorators on a glyph; passing `null` clears them |
| `SetDecorator(decorator, glyph)` | Replaces all decorators with a single decorator |
| `AddDecorators(IEnumerable?, glyph)` | Adds decorators, skipping duplicates |
| `AddDecorator(decorator, glyph)` | Adds a single decorator if not already present |
| `RemoveDecorators(IEnumerable, glyph)` | Removes specific decorators; nulls the list if empty |
| `RemoveDecorator(decorator, glyph)` | Removes a single decorator; nulls the list if empty |
| `RemoveAllDecorators(glyph)` | Clears all decorators and returns the list to the pool |
| `CloneDecorators(glyph)` | Returns a pooled copy of the glyph's decorator list |
| `ItemsMatch(list, list)` | Compares two decorator lists for element-wise equality |

All methods that leave an empty list automatically return it to the pool and set `Decorators` to `null`.

### Mirror (`SadConsole/GlyphMirror.cs`)

A `[Flags]` enum controlling glyph flip transformations during rendering:

```csharp
[Flags]
public enum Mirror
{
    None = 0,
    Vertical = 1,      // Flip upside-down
    Horizontal = 2,     // Flip left-right
}
```

`Vertical | Horizontal` produces a 180° rotation. Used on `ColoredGlyphBase.Mirror`, `GlyphDefinition.Mirror`, and `CellDecorator.Mirror`.

### FontExtensions (`SadConsole/Font.Extensions.cs`)

Static extension methods for font-related calculations:

- `GetRenderRect(this IFont, int x, int y, Point fontSize)` — converts a cell position to a pixel-space destination rectangle.
- `GetWorldPosition(this IFont, Point position, Point fontSize)` — converts a cell position to pixel coordinates.
- `GetGlyphRatio(this IFont, Point fontSize)` — returns the X/Y aspect ratio of a glyph at the given font size.

---

## 3. Font File Format

A `.font` file is a JSON file containing a serialized `SadFont` object. It is paired with a PNG spritesheet that contains the glyph atlas.

### Minimal Example

```json
{
    "$type": "SadConsole.SadFont, SadConsole",
    "Name": "IBM_8x16",
    "FilePath": "IBM8x16.png",
    "GlyphHeight": 16,
    "GlyphPadding": 1,
    "GlyphWidth": 8,
    "SolidGlyphIndex": 219,
    "Columns": 16
}
```

### Extended Example (with glyph definitions)

```json
{
    "$type": "SadConsole.SadFont, SadConsole",
    "Name": "IBM_8x16_ext",
    "FilePath": "IBM8x16_NoPadding_extended.png",
    "GlyphHeight": 16,
    "GlyphWidth": 8,
    "GlyphPadding": 0,
    "SolidGlyphIndex": 219,
    "Columns": 16,
    "IsSadExtended": true,
    "GlyphDefinitions": {
        "underline": { "Glyph": 95, "Mirror": 0 },
        "strikethrough": { "Glyph": 196, "Mirror": 0 },
        "box-edge-left": { "Glyph": 256, "Mirror": 0 }
    }
}
```

### Field Reference

| JSON Field | SadFont Property | Required | Notes |
|------------|------------------|----------|-------|
| `$type` | *(type metadata)* | Yes | Must be `"SadConsole.SadFont, SadConsole"` |
| `Name` | `Name` | Yes | Font identifier string |
| `FilePath` | `FilePath` | Yes | Path to PNG spritesheet — relative path or `"res:..."` for embedded resources |
| `GlyphWidth` | `GlyphWidth` | Yes | Pixel width per glyph |
| `GlyphHeight` | `GlyphHeight` | Yes | Pixel height per glyph |
| `GlyphPadding` | `GlyphPadding` | No | Pixel spacing between tiles (default: 0) |
| `SolidGlyphIndex` | `SolidGlyphIndex` | No | Index of the solid white glyph for background fills |
| `UnsupportedGlyphIndex` | `UnsupportedGlyphIndex` | No | Index for missing-glyph fallback (default: 0) |
| `Columns` | `Columns` | No | Glyphs per row; auto-calculated from image width if 0 |
| `Rows` | `Rows` | No | Glyph rows; auto-calculated from image height if 0 |
| `IsSadExtended` | `IsSadExtended` | No | Marks font as using SadConsole's extended character set (default: false) |
| `GlyphDefinitions` | `GlyphDefinitions` | No | Name → `{ Glyph, Mirror }` mapping |

### PNG Spritesheet Layout

The companion PNG is a 2D grid of glyph tiles:

```
┌───┬───┬───┬───┬───┐
│ 0 │ 1 │ 2 │ 3 │...│  ← Row 0
├───┼───┼───┼───┼───┤
│ C │C+1│C+2│C+3│...│  ← Row 1  (C = Columns)
├───┼───┼───┼───┼───┤
│...│   │   │   │...│
└───┴───┴───┴───┴───┘
```

- Glyphs are in **row-major order**: glyph index `i` is at column `i % Columns`, row `i / Columns`.
- Each tile is `GlyphWidth × GlyphHeight` pixels.
- If `GlyphPadding > 0`, tiles are separated by that many pixels. The padding formula for glyph at column `cx`, row `cy` is: `x = (cx × GlyphWidth) + ((cx + 1) × GlyphPadding)`, `y = (cy × GlyphHeight) + ((cy + 1) × GlyphPadding)`.
- If `GlyphPadding = 0`, tiles are adjacent: `x = cx × GlyphWidth`, `y = cy × GlyphHeight`.

Glyphs are **monochrome** (typically white on transparent). Colors are applied at render time via `ColoredGlyph.Foreground` and `ColoredGlyph.Background`.

---

## 4. Glyph System

### Glyph Index

A glyph index is an integer (0 to `TotalGlyphs - 1`) that maps to a tile position in the spritesheet. Each `ColoredGlyphBase` cell carries a single glyph index in its `Glyph` property.

### Glyph Rectangle Lookup

The `GlyphRectangles` dictionary maps each glyph index to its source `Rectangle` in the atlas. It is pre-computed at font load time by `ConfigureRects()` / `ForceConfigureRects()`.

Runtime lookup:

```csharp
Rectangle rect = font.GetGlyphSourceRectangle(cell.Glyph);
// Returns GlyphRectangles[glyph] if found, else UnsupportedGlyphRectangle
```

The fallback to `UnsupportedGlyphRectangle` prevents crashes from invalid glyph references.

### Solid Glyph

`SolidGlyphIndex` points to a fully opaque white glyph used by the renderer for:
- Background color fills (the solid glyph is drawn at `cell.Background` color behind the character glyph).
- Debug visualization and solid-colored blocks.

When `SolidGlyphIndex` is set, `SolidGlyphRectangle` updates automatically.

### GlyphDefinition

Any font can have a `GlyphDefinitions` dictionary that maps string names to `GlyphDefinition` structs. This is a general-purpose helper that enables code to reference glyphs by semantic name rather than hardcoding indices:

```csharp
GlyphDefinition def = font.GetGlyphDefinition("underline");
CellDecorator deco = def.CreateCellDecorator(Color.Yellow);
```

`GlyphDefinitions` is independent of `IsSadExtended` — any font can define named glyphs regardless of whether it uses the extended character set.

### IsSadExtended

`IsSadExtended` marks a font as using SadConsole's extended character set. Extended fonts typically include glyph indices beyond the standard 256 (e.g., indices 256–275 for box edges and decorators in `IBM_ext.font`). This flag indicates the font provides a specific set of glyphs designed for use with SadConsole, but it does not gate or enable `GlyphDefinitions` support.

### CellDecorator Rendering

Each `ColoredGlyphBase` can hold a `List<CellDecorator>?`. During rendering:

1. The renderer draws the cell's background using the solid glyph at `cell.Background` color.
2. The renderer draws the base glyph at `cell.Foreground` color.
3. For each decorator in `cell.Decorators`, the renderer draws the decorator's glyph at the decorator's color on top.

This layering enables underlines, strikethrough, box edges, and other overlays without requiring additional surface layers.

### Glyph Index Remapping (Legacy)

`SadFont` supports a serialized `_remapper` field (`IndexMapping[]?`) for legacy fonts with irregular layouts. After deserialization, each mapping overrides the auto-generated rectangle for an index by computing the rectangle for a different source index. The remapper array is cleared after application and is not used during normal operation.

---

## 5. Font Sizes and Scaling

### IFont.Sizes Enum

```csharp
public enum Sizes
{
    Quarter = 0,  // 0.25× glyph dimensions
    Half = 1,     // 0.50× glyph dimensions
    One = 2,      // 1.00× glyph dimensions (standard)
    Two = 3,      // 2.00× glyph dimensions
    Three = 4,    // 3.00× glyph dimensions
    Four = 5      // 4.00× glyph dimensions
}
```

### GetFontSize

`IFont.GetFontSize(Sizes)` computes pixel dimensions from a size enum:

```csharp
public Point GetFontSize(Sizes size) => size switch
{
    Sizes.Quarter => new Point((int)(GlyphWidth * 0.25), (int)(GlyphHeight * 0.25)),
    Sizes.Half    => new Point((int)(GlyphWidth * 0.5),  (int)(GlyphHeight * 0.5)),
    Sizes.Two     => new Point(GlyphWidth * 2, GlyphHeight * 2),
    Sizes.Three   => new Point(GlyphWidth * 3, GlyphHeight * 3),
    Sizes.Four    => new Point(GlyphWidth * 4, GlyphHeight * 4),
    _             => new Point(GlyphWidth, GlyphHeight),  // Sizes.One
};
```

### Scaling Model

Size scaling is **logical**, not physical. The same spritesheet texture is used at all sizes. The renderer draws from the original glyph source rectangle to a larger or smaller destination rectangle, and the graphics API (e.g., MonoGame `SpriteBatch`) handles the scaling. `SamplerState.PointClamp` is used for pixel-perfect rendering without interpolation.

Quarter and Half scales may produce rounding artifacts due to integer truncation in the `Point` calculation.

### Default Font Size

`GameHost.DefaultFontSize` is an `IFont.Sizes` value (default: `Sizes.One`) that applies to new `ScreenSurface` instances that do not explicitly specify a font size. It can be set during startup via `ConfigureFonts().SetDefaultFontSize(size)`.

---

## 6. Font Registration and Loading

### GameHost.Fonts

**File:** `SadConsole/GameHost.cs`

The central font registry is a `Dictionary<string, IFont>` on the `GameHost` singleton:

```csharp
public Dictionary<string, IFont> Fonts { get; }
```

All loaded fonts are registered here by name. The registry is used during deserialization to resolve font references.

### Embedded Fonts

Two fonts are embedded as assembly resources and loaded at startup:

| Property | Resource | Font Name |
|----------|----------|-----------|
| `GameHost.EmbeddedFont` | `SadConsole.Resources.IBM.font` | `IBM_8x16` |
| `GameHost.EmbeddedFontExtended` | `SadConsole.Resources.IBM_ext.font` | `IBM_8x16_ext` |

### LoadDefaultFonts(string? defaultFont)

Called by each host during initialization:

1. Loads `EmbeddedFont` and `EmbeddedFontExtended` from assembly resources.
2. Selects the default font:
   - If `defaultFont` is provided → loads it via `LoadFont(defaultFont)`.
   - If `Settings.UseDefaultExtendedFont` is `true` → uses `EmbeddedFontExtended`.
   - Otherwise → uses `EmbeddedFont`.

### LoadFont(string fontFilePath)

Loads a font from a `.font` JSON file and registers it:

```csharp
public IFont LoadFont(string fontFilePath)
{
    IFont font = Serializer.Load<IFont>(fontFilePath, false, jsonSettings);
    if (Fonts.TryGetValue(font.Name, out IFont? existing))
        return existing;  // Return cached copy if already loaded
    Fonts.Add(font.Name, font);
    return font;
}
```

Uses `TypeNameHandling.All` to preserve the `$type` field. Fonts are cached by name — duplicate names return the existing instance.

### Deserialization Pipeline (SadFont.AfterDeserialized)

When a `SadFont` is deserialized from JSON, the `[OnDeserialized]` callback fires:

1. **Resolve `FilePath`** — if the path starts with `"res:"`, load from the assembly's embedded resources via `GetManifestResourceStream`. Otherwise, resolve relative to `GameHost.SerializerPathHint` (the directory of the `.font` file) and load from disk.
2. **Create `ITexture Image`** — calls `GameHost.Instance.GetTexture(path)` or `GetTexture(stream)`, which is host-specific.
3. **Compute grid dimensions** — if `Columns` or `Rows` is 0, calculate from the image dimensions: `Columns = Image.Width / (GlyphWidth + GlyphPadding)`.
4. **Generate glyph rectangles** — calls `ConfigureRects()` unless `_skipAutomaticGlyphGeneration` is set.
5. **Apply remapping** — if the legacy `_remapper` array is present, override specific glyph rectangles, then clear the array.

### FontConfig (`SadConsole/Configuration/FontConfig.cs`)

A configurator that runs during `GameHost` initialization via the fluent builder API:

- `ConfigureFonts(bool useExtendedDefault)` — selects the built-in font.
- `ConfigureFonts(string customDefaultFont, string[]? extraFonts)` — loads a custom default font plus additional fonts.
- `ConfigureFonts(Action<FontConfig, GameHost> fontLoader)` — custom delegate for complex loading scenarios.
- `SetDefaultFontSize(IFont.Sizes size)` — sets the default font size for new surfaces.

---

## 7. Surface-Font Integration

### ScreenSurface Font Properties

Each `ScreenSurface` holds two independent font-related properties:

```csharp
public IFont Font { get; set; }       // The sprite-sheet atlas
public Point FontSize { get; set; }   // Pixel dimensions used for rendering cells
```

`Font` is the atlas. `FontSize` is the display scale. They are **independent** — multiple surfaces can share the same `Font` but render at different sizes.

```csharp
var surface1 = new ScreenSurface(80, 25);
surface1.Font = gameHost.DefaultFont;    // IBM 8×16 atlas
surface1.FontSize = new Point(8, 16);   // 1× size

var surface2 = new ScreenSurface(80, 25);
surface2.Font = gameHost.DefaultFont;    // Same atlas
surface2.FontSize = new Point(16, 32);  // 2× size
```

### Default Assignment

During `ScreenSurface` construction:

```csharp
Font = font ?? GameHost.Instance.DefaultFont;
FontSize = Font.GetFontSize(GameHost.Instance.DefaultFontSize);
```

### Font Change Notification

When either `Font` or `FontSize` changes:

1. `OnFontChanged(IFont oldFont, Point oldFontSize)` fires (virtual; overridable by subclasses).
2. `IsDirty = true` — triggers a renderer refresh on the next frame.
3. The renderer detects the change passively during its `Refresh` phase by checking whether `AbsoluteArea` dimensions have changed, and reallocates its backing texture automatically if needed.

`UpdateAbsolutePosition()` is **not** called automatically. The caller is responsible for re-layout if font changes affect positioning.

### Coordinate System Impact

In **cell mode** (default, `UsePixelPositioning = false`):

```csharp
AbsolutePosition = Position * FontSize + Parent.AbsolutePosition
```

A surface at cell position `(5, 3)` with `FontSize = (8, 16)` is at pixel `(40, 48)` plus parent offset.

In **pixel mode** (`UsePixelPositioning = true`):

```csharp
AbsolutePosition = Position + Parent.AbsolutePosition
```

The surface's pixel-space bounding rectangle is:

```csharp
WidthPixels  = Surface.View.Width  * FontSize.X
HeightPixels = Surface.View.Height * FontSize.Y
```

These dimensions use the **viewport** size, not the full buffer size. A scrollable surface with a 80×25 view but 80×500 buffer presents as 80×25 cells on screen.

---

## 8. Host Rendering Pipeline

### Shared Pattern

All hosts follow the same rendering pattern for font glyphs. The `SurfaceRenderStep` (in each host's `Renderers/Steps/` folder) executes during the renderer's `Refresh` phase:

1. **Allocate backing texture** — create or resize a `RenderTarget2D` (MonoGame/FNA) or `RenderTexture` (SFML) to match `screenObject.AbsoluteArea` pixel dimensions.
2. **Begin batch** — start a sprite batch targeting the backing texture.
3. **Draw default background** — if `Surface.DefaultBackground` has non-zero alpha, draw a full-surface rectangle using `Font.SolidGlyphRectangle` as the source and the default background as the color.
4. **Iterate visible cells** — loop through the viewport (`Surface.View`), reading cells from the flat array using viewport-adjusted indices:
   ```csharp
   int i = ((y + ViewPosition.Y) * Surface.Width) + ViewPosition.X;
   ```
5. **Per cell:**
   - **Background:** Draw `Font.SolidGlyphRectangle` at `cell.Background` color if non-transparent and different from the default background.
   - **Glyph:** Draw `Font.GetGlyphSourceRectangle(cell.Glyph)` at `cell.Foreground` color (skip if glyph is 0 or foreground is transparent).
   - **Decorators:** For each `CellDecorator` in `cell.Decorators`, draw the decorator's glyph rectangle at the decorator's color.
6. **End batch** — finalize the sprite batch.

Destination rectangles are pre-computed in `CachedRenderRects` (on the renderer), sized according to `FontSize`, not the native glyph dimensions. This is how scaling works — the source rectangle is always at native glyph size; the destination rectangle is at the surface's `FontSize`.

### MonoGame Host

**File:** `SadConsole.Host.MonoGame/Renderers/Steps/SurfaceRenderStep.cs`

- Backing texture: `RenderTarget2D` (set via `GraphicsDevice.SetRenderTarget`).
- Draw calls: `LocalSpriteBatch.Draw(fontTexture, destRect, srcRect, color, ...)`.
- Font texture access: `((Host.GameTexture)font.Image).Texture` → `Texture2D`.
- Sampler: `SamplerState.PointClamp` for pixel-perfect rendering.
- Blend state: Configurable via `IRendererMonoGame.MonoGameBlendState`.

**Font editing extension** (`SadConsole.Host.MonoGame/ExtensionsFont.cs`): `Edit_EnableEditing(this IFont)` converts the font's `Texture2D` to a `RenderTarget2D` for GPU-side glyph manipulation.

### SFML Host

**File:** `SadConsole.Host.SFML/Renderers/Steps/SurfaceRenderStep.cs`

- Backing texture: `RenderTexture` (SFML).
- Draw calls: `SharedSpriteBatch.DrawCell(cell, destRect, drawBackground, font)` — a helper that wraps the quad-level draw logic.
- Texture dimensions use `uint` (SFML convention).
- Requires an explicit `BackingTexture.Display()` call after rendering to finalize the texture.
- Uses `IntRect` (int coordinates) rather than float-based rectangles.

### FNA Host

FNA uses the same XNA-compatible API as MonoGame. Its `GameTexture` implementation wraps `Microsoft.Xna.Framework.Graphics.Texture2D` (FNA fork) and follows identical code paths for font texture loading and glyph rendering.

### Texture Loading (GameHost.GetTexture)

Each host implements `GetTexture` on `GameHost`:

| Host | Underlying Texture Type | Source |
|------|------------------------|--------|
| MonoGame | `Texture2D` | `Texture2D.FromStream(GraphicsDevice, stream)` |
| SFML | `SFML.Graphics.Texture` | `new Texture(stream)` |
| FNA | `Texture2D` (FNA) | Same as MonoGame |

All hosts wrap their native texture in a `GameTexture` class that implements `ITexture`, providing a uniform API for pixel access (`GetPixels`, `SetPixels`, `GetPixel`, `SetPixel`) and lifecycle management.

### Font Texture Lifecycle

```
.font JSON
  ↓ [JSON deserialization]
SadFont instance
  ↓ [AfterDeserialized → FilePath resolution]
GameHost.GetTexture(path or stream)
  ↓ [Host-specific texture creation]
GameTexture wrapper (ITexture)
  ↓ [Stores native GPU texture: Texture2D / SFML.Texture / etc.]
  ↓ [During rendering]
SpriteBatch.Draw(fontTexture, destRect, srcRect, color)
  ↓ [GPU rasterization]
Backing texture → draw call queue → window
```

---

## 9. Events and Serialization

### Font-Related Events

`GameHost` exposes two events for global font changes:

```csharp
public event EventHandler<FontChangedEventArgs>? DefaultFontChanged;
public event EventHandler<FontSizeChangedEventArgs>? DefaultFontSizeChanged;
```

**`FontChangedEventArgs`** (`SadConsole/FontChangedEventArgs.cs`): carries `OldFont` and `NewFont` (`IFont`).

**`FontSizeChangedEventArgs`** (`SadConsole/FontSizeChangedEventArgs.cs`): carries `OldSize` and `NewSize` (`IFont.Sizes`).

These fire when `GameHost.DefaultFont` or `GameHost.DefaultFontSize` change at runtime.

At the surface level, `ScreenSurface.OnFontChanged(IFont oldFont, Point oldFontSize)` is a virtual method invoked when either `Font` or `FontSize` changes on that surface.

### Font Serialization Strategy

When a `ScreenSurface` is serialized (e.g., to save game state), the font is **not** embedded. Instead, a custom JSON converter stores only the font's name:

**`FontJsonConverter`** (`SadConsole/SerializedTypes/Font.cs`):

- **Write:** Serializes `IFont` as `FontSerialized { Name = font.Name }`.
- **Read:** Deserializes `FontSerialized`, then looks up `Name` in `GameHost.Instance.Fonts`. If the name is not found, falls back to `GameHost.Instance.DefaultFont`.

This strategy avoids duplicating spritesheet texture data in save files. It requires that all fonts referenced by serialized surfaces are registered in `GameHost.Fonts` before deserialization occurs.

Font files themselves (`.font` JSON) are serialized with `TypeNameHandling.All` to preserve the `$type` discriminator for polymorphic deserialization.

---

## 10. Design Principles

### Core/Host Separation

The font system follows SadConsole's primary architectural principle:

- **Core** owns metadata: `IFont` interface, `SadFont` implementation, `GlyphDefinition`, `CellDecorator`, `Mirror`, font registration, `.font` file parsing.
- **Hosts** own GPU resources: `ITexture` implementation, texture loading from disk/stream, sprite batch rendering of glyphs.

The core never touches GPU state. `IFont.Image` is typed as `ITexture` (the core interface), and all texture creation goes through `GameHost.GetTexture()`.

### Immutability Decisions

**Immutable (value types):**
- `GlyphDefinition` — `readonly struct` with `init` properties.
- `CellDecorator` — `readonly struct` with `init` properties.
- `Mirror` — enum.

**Mutable (by design):**
- `SadFont` — class; supports post-construction modification of `GlyphPadding`, `SolidGlyphIndex`, `UnsupportedGlyphIndex`, and `Image`.
- `GlyphRectangles` and `GlyphDefinitions` dictionaries are mutable for runtime modification.

### Extension Points

**Custom `IFont` implementations:** Host or library code can implement `IFont` directly for procedurally generated fonts, runtime atlasing, multi-layer glyphs, or alternative serialization formats.

**Custom `IRenderStep` for font features:** Hosts can add render steps to implement font drop shadows, outline effects, glyph animation, or colored glyph replacement.

**`GlyphDefinition` libraries:** Font definitions can be expanded with domain-specific named glyphs (e.g., UI icon sets, game-specific tile names).

**Font editing at runtime:** The MonoGame host provides `Edit_EnableEditing(this IFont)` to convert a font's texture to a `RenderTarget2D` for GPU-side modification. Other hosts can implement equivalent functionality.

---

## Key File Reference

### Core Font Types

| File | Contents |
|------|----------|
| `SadConsole/IFont.cs` | `IFont` interface and `IFont.Sizes` enum |
| `SadConsole/SadFont.cs` | Sealed `SadFont` implementation |
| `SadConsole/GlyphDefinition.cs` | Named glyph struct to identify glyphs by name |
| `SadConsole/CellDecorator.cs` | Overlay glyph struct |
| `SadConsole/CellDecoratorHelpers.cs` | Static helpers for managing decorator lists on cells (pooling, add/remove/clone) |
| `SadConsole/GlyphMirror.cs` | `Mirror` flags enum |
| `SadConsole/Font.Extensions.cs` | Font utility extension methods |

### Font Loading and Registration

| File | Contents |
|------|----------|
| `SadConsole/GameHost.cs` | `Fonts` dictionary, `LoadFont()`, `LoadDefaultFonts()` |
| `SadConsole/Configuration/FontConfig.cs` | Font configuration builder |

### Font Events

| File | Contents |
|------|----------|
| `SadConsole/FontChangedEventArgs.cs` | `FontChangedEventArgs` |
| `SadConsole/FontSizeChangedEventArgs.cs` | `FontSizeChangedEventArgs` |

### Serialization

| File | Contents |
|------|----------|
| `SadConsole/SerializedTypes/Font.cs` | `FontJsonConverter` and `FontSerialized` |

### Embedded Resources

| File | Contents |
|------|----------|
| `SadConsole/Resources/IBM.font` | Standard IBM 8×16 font metadata |
| `SadConsole/Resources/IBM_ext.font` | Extended IBM font with glyph definitions |

### Host Rendering

| File | Contents |
|------|----------|
| `SadConsole.Host.MonoGame/GameTexture.cs` | MonoGame `ITexture` implementation |
| `SadConsole.Host.MonoGame/Renderers/Steps/SurfaceRenderStep.cs` | MonoGame glyph rendering |
| `SadConsole.Host.MonoGame/ExtensionsFont.cs` | Font editing extensions |
| `SadConsole.Host.SFML/GameTexture.cs` | SFML `ITexture` implementation |
| `SadConsole.Host.SFML/Renderers/Steps/SurfaceRenderStep.cs` | SFML glyph rendering |
| `SadConsole.Host.Shared/GameTextureHelpers.cs` | Shared texture utility methods |

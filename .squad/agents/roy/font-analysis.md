# SadConsole Font System Architecture Analysis

**Date:** 2026-02-26  
**Analyst:** Roy (Core Dev)  
**Purpose:** Deep-dive technical analysis of the font system for Deckard's architecture document

---

## Executive Summary

SadConsole's font system separates **font metadata** (glyphs per spritesheet, sizing, definitions) from **font rendering** (GPU texture management, draw calls). The core library owns `IFont` and `SadFont`; hosts own GPU texture loading via `ITexture`. This analysis covers all font types, loading/registration, size scaling, glyph definitions, and decorator support.

---

## 1. IFont Interface — Specification

**File:** `SadConsole/IFont.cs`

The `IFont` interface is the contract that all font implementations must satisfy. It represents a **spritesheet atlas** with named glyphs.

### Properties

| Property | Type | Purpose |
|----------|------|---------|
| `Name` | `string` | Font identifier (used when registered in `GameHost.Fonts`) |
| `GlyphWidth` | `int` | Pixel width of each glyph tile in the spritesheet |
| `GlyphHeight` | `int` | Pixel height of each glyph tile in the spritesheet |
| `GlyphPadding` | `int` | Pixel spacing between tiles in the atlas (set/get, mutable) |
| `TotalGlyphs` | `int` | Total count of glyphs = `Columns × Rows` |
| `SolidGlyphIndex` | `int` | Glyph index of a solid white box (used for background fills) |
| `SolidGlyphRectangle` | `Rectangle` | Pre-computed rectangle for the solid glyph (read-only) |
| `UnsupportedGlyphIndex` | `int` | Glyph index to use when a requested glyph is not found (fallback) |
| `UnsupportedGlyphRectangle` | `Rectangle` | Pre-computed rectangle for the unsupported glyph (read-only) |
| `IsSadExtended` | `bool` | `true` when font supports SadConsole extended decorators; otherwise `false` |
| `Image` | `ITexture` | Host-provided texture object (GPU texture; mutable) |
| `GlyphRectangles` | `Dictionary<int, Rectangle>` | Mapping of glyph index → source rectangle in spritesheet |
| `GlyphDefinitions` | `Dictionary<string, GlyphDefinition>` | Named glyph definitions (for extended fonts) |

### Methods

| Method | Signature | Purpose |
|--------|-----------|---------|
| `GetGlyphSourceRectangle(int glyph)` | `Rectangle GetGlyphSourceRectangle(int glyph)` | Returns the source rectangle for a glyph index from `GlyphRectangles`; falls back to `UnsupportedGlyphRectangle` if not found. |
| `GenerateGlyphSourceRectangle(int glyph)` | `Rectangle GenerateGlyphSourceRectangle(int glyph)` | Generates a rectangle based on glyph index, row/column math, and padding. Does **not** consult `GlyphRectangles` — purely algorithmic. Used to auto-generate rects on load. |
| `GetFontSize(IFont.Sizes size)` | `Point GetFontSize(Sizes size)` | Returns pixel dimensions for a given size multiplier (Quarter, Half, One, Two, Three, Four). |
| `GetDecorator(string name, Color color)` | `CellDecorator GetDecorator(string name, Color color)` | Looks up a named `GlyphDefinition`, creates a `CellDecorator` with the specified color, and returns it. Returns `CellDecorator.Empty` if not found. |
| `GetGlyphDefinition(string name)` | `GlyphDefinition GetGlyphDefinition(string name)` | Retrieves a named `GlyphDefinition` directly. Returns `GlyphDefinition.Empty` if not found. |
| `HasGlyphDefinition(string name)` | `bool HasGlyphDefinition(string name)` | Checks if a named glyph definition exists. |

### Nested Enum: IFont.Sizes

```csharp
public enum Sizes
{
    Quarter = 0,  // 0.25× (GlyphWidth * 0.25, GlyphHeight * 0.25)
    Half = 1,     // 0.50× (GlyphWidth * 0.5, GlyphHeight * 0.5)
    One = 2,      // 1.00× (GlyphWidth, GlyphHeight) — standard size
    Two = 3,      // 2.00× (GlyphWidth * 2, GlyphHeight * 2)
    Three = 4,    // 3.00× (GlyphWidth * 3, GlyphHeight * 3)
    Four = 5      // 4.00× (GlyphWidth * 4, GlyphHeight * 4)
}
```

**Note:** Sizes are **scaling factors applied to rendering**, not separate font files. The same spritesheet is displayed at different pixel sizes. The renderer uses `FontSize` (a `Point`) to scale cells during blit operations.

---

## 2. SadFont — Concrete Implementation

**File:** `SadConsole/SadFont.cs`

`SadFont` is the canonical (sealed) implementation of `IFont`. It is fully serializable via JSON and supports deserialization from `.font` files.

### Additional Properties (Beyond IFont)

| Property | Type | Purpose |
|----------|------|---------|
| `Columns` | `int` | Number of glyphs per row in the spritesheet |
| `Rows` | `int` | Number of glyph rows in the spritesheet |
| `FilePath` | `string` | Path to the PNG (relative or embedded resource path like `"res:..."`) |
| `_remapper` | `IndexMapping[]?` | [DataMember] Legacy field for serialized glyph remapping (deserialized only) |
| `_skipAutomaticGlyphGeneration` | `bool` | [DataMember] Flag to prevent auto-generation of `GlyphRectangles` on deserialization |

### Key Methods

#### ConfigureRects() / ForceConfigureRects()

- **`ConfigureRects()`** — Generates `GlyphRectangles` if the dictionary is empty (old-style font); otherwise assumes rectangles are pre-computed.
- **`ForceConfigureRects()`** — Unconditionally regenerates all rectangles based on `Columns`, `Rows`, `GlyphWidth`, `GlyphHeight`, and `GlyphPadding`.

**Algorithm:**
```csharp
for (int i = 0; i < Rows * Columns; i++)
{
    int cx = i % Columns;
    int cy = i / Columns;
    
    if (GlyphPadding != 0)
        GlyphRectangles[i] = new Rectangle(
            (cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
            (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
            GlyphWidth, GlyphHeight);
    else
        GlyphRectangles[i] = new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
}
```

#### Clone(string newName)

Creates a deep copy of the font with a new name. Copies the texture pixels and all `GlyphDefinitions` and `GlyphRectangles`.

#### AfterDeserialized() (OnDeserialized callback)

Executes after JSON deserialization:
1. Loads `ITexture Image` from file path (either embedded resource or disk).
2. Auto-calculates `Columns` / `Rows` from image dimensions if they are 0.
3. Calls `ConfigureRects()` to generate rectangles if `_skipAutomaticGlyphGeneration` is false.
4. Applies `_remapper` index mappings if present, then clears it.

---

## 3. Font Registration and Loading

### GameHost.Fonts Dictionary

**File:** `SadConsole/GameHost.cs` (lines 114)

- **Type:** `Dictionary<string, IFont>`
- **Purpose:** Central registry of all available fonts by name.
- **Scope:** Singleton on `GameHost.Instance`.
- **Access:** Used during deserialization to resolve font references by name.

### Font Loading Flow

#### LoadDefaultFonts(string? defaultFont)

Called during host initialization (in derived host classes).

1. **Load embedded fonts:**
   ```csharp
   EmbeddedFont = LoadResourceFont("SadConsole.Resources.IBM.font");
   EmbeddedFontExtended = LoadResourceFont("SadConsole.Resources.IBM_ext.font");
   ```

2. **Set default font:**
   - If `defaultFont == null` and `Settings.UseDefaultExtendedFont == true` → use `EmbeddedFontExtended`
   - Else → use `EmbeddedFont`
   - Or if `defaultFont` is provided → load and use custom font via `LoadFont(defaultFont)`

#### LoadFont(string font)

**File:** `SadConsole/GameHost.cs` (lines 347–379)

Loads a font from a `.font` JSON file and registers it.

```csharp
public IFont LoadFont(string font)
{
    var settings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    IFont masterFont = Serializer.Load<IFont>(font, false, settings);
    
    if (Fonts.TryGetValue(masterFont.Name, out IFont? existing))
        return existing;  // Return cached copy if already loaded
    
    Fonts.Add(masterFont.Name, masterFont);
    return masterFont;
}
```

**Key points:**
- Uses `TypeNameHandling.All` to preserve the `$type` field in JSON.
- Caches by name — duplicate names return the existing instance.
- Throws `JsonSerializationException` if the file is malformed or missing `$type`.

#### FontConfig Configuration System

**File:** `SadConsole/Configuration/FontConfig.cs`

A configurator that runs during `GameHost` initialization:

- **`ConfigureFonts(bool useExtendedDefault)`** — Selects built-in font.
- **`ConfigureFonts(string customDefaultFont, string[]? extraFonts)`** — Loads custom default + extra fonts.
- **`ConfigureFonts(Action<FontConfig, GameHost> fontLoader)`** — Custom delegate for complex loading.
- **`SetDefaultFontSize(IFont.Sizes size)`** — Sets the default font size.

```csharp
public class FontConfig : IConfigurator
{
    public Action<FontConfig, GameHost>? FontLoader { get; set; }
    public string[] CustomFonts = Array.Empty<string>();
    public string? AlternativeDefaultFont = null;
    public bool UseExtendedFont = false;
    public IFont.Sizes DefaultFontSize = IFont.Sizes.One;
    
    public void Run(BuilderBase config, GameHost game)
    {
        FontLoader?.Invoke(this, game);
        Settings.UseDefaultExtendedFont = UseExtendedFont;
    }
}
```

---

## 4. Font Sizes and Scaling Model

### Size Independence

**Key Design:** `Font` and `FontSize` are **independent** on `ScreenSurface`.

```csharp
public IFont Font { get; set; }           // The atlas (spritesheet)
public Point FontSize { get; set; }       // Pixel dimensions for rendering
```

**Consequence:** Multiple `ScreenSurface` instances can share the same `Font` but render at different sizes.

Example:
```csharp
var surface1 = new ScreenSurface(80, 25);
surface1.Font = gameHost.DefaultFont;     // IBM 8×16 atlas
surface1.FontSize = new Point(8, 16);     // Render at 1× size

var surface2 = new ScreenSurface(80, 25);
surface2.Font = gameHost.DefaultFont;     // Same IBM atlas
surface2.FontSize = new Point(16, 32);    // Render at 2× size
```

Both surfaces share the same spritesheet but are displayed at different scales.

### GetFontSize(IFont.Sizes size)

This method (defined on `IFont` interface; implemented on `SadFont`) computes pixel dimensions from a size enum:

```csharp
public Point GetFontSize(IFont.Sizes size)
{
    return size switch
    {
        Sizes.Quarter => new Point((int)(GlyphWidth * 0.25), (int)(GlyphHeight * 0.25)),
        Sizes.Half => new Point((int)(GlyphWidth * 0.5), (int)(GlyphHeight * 0.5)),
        Sizes.Two => new Point(GlyphWidth * 2, GlyphHeight * 2),
        Sizes.Three => new Point(GlyphWidth * 3, GlyphHeight * 3),
        Sizes.Four => new Point(GlyphWidth * 4, GlyphHeight * 4),
        _ => new Point(GlyphWidth, GlyphHeight),  // Sizes.One
    };
}
```

### Default Font Size

- **GameHost.DefaultFontSize** — An `IFont.Sizes` enum value (default: `One`).
- Applies to new `ScreenSurface` instances that don't explicitly specify a font size.
- Can be set via `ConfigureFonts().SetDefaultFontSize(size)`.

---

## 5. Glyph System

### Glyph Index

A **glyph index** is an integer (0 to `TotalGlyphs - 1`) that maps to a position in the spritesheet atlas.

- Glyphs are laid out in **row-major order** in the spritesheet.
- Index calculation: `glyph = row * Columns + col`
- Inverse: `row = glyph / Columns`, `col = glyph % Columns`

Each `ColoredGlyphBase` cell carries a single glyph index, which the renderer uses to fetch the source rectangle and blit.

### GlyphRectangles Dictionary

**Type:** `Dictionary<int, Rectangle>`

Maps glyph indices to their source rectangles in the spritesheet. Pre-computed at font load time.

- **Modern fonts:** Rectangles are generated by `ForceConfigureRects()` or provided at load time.
- **Legacy fonts:** If empty at deserialization, `ConfigureRects()` generates them.

Access via `IFont.GetGlyphSourceRectangle(int glyph)`:
```csharp
public Rectangle GetGlyphSourceRectangle(int glyph)
{
    if (GlyphRectangles.TryGetValue(glyph, out Rectangle value))
        return value;
    return UnsupportedGlyphRectangle;  // Fallback
}
```

### GlyphDefinition Struct

**File:** `SadConsole/GlyphDefinition.cs`

A lightweight value type representing a **named glyph** for extended fonts.

```csharp
public readonly struct GlyphDefinition
{
    public int Glyph { get; init; }
    public Mirror Mirror { get; init; }
    
    public CellDecorator CreateCellDecorator(Color foreground) 
        => new(foreground, Glyph, Mirror);
    
    public ColoredGlyphBase CreateCell(Color foreground, Color background)
        => new ColoredGlyph(foreground, background, Glyph, Mirror);
    
    public static GlyphDefinition Empty { get; } = new GlyphDefinition(-1, 0);
}
```

**Purpose:** Enables fonts to define named glyphs (e.g., "underline", "box-corner-tl") that can be referenced by string in code, rather than hardcoding indices.

### Solid Glyph

**Property:** `IFont.SolidGlyphIndex` (set/get)

An index pointing to a fully opaque white glyph, used by the renderer for:
- Background fills
- Solid-colored blocks
- Debug visualization

When `SolidGlyphIndex` changes, `SolidGlyphRectangle` is automatically updated via the setter.

### Unsupported Glyph

**Property:** `IFont.UnsupportedGlyphIndex` (set/get)

When rendering code requests a glyph index that doesn't exist in `GlyphRectangles`, the renderer falls back to this index (typically a placeholder "?" character).

---

## 6. Extended Font Support (IsSadExtended)

**Property:** `IFont.IsSadExtended` (bool)

When `true`, the font declares support for **SadConsole extended glyph definitions** and **cell decorators**.

### Use Cases

Extended fonts enable:
- **Named glyphs:** Defined in `GlyphDefinitions` (e.g., "underline", "box-edge-left")
- **Cell decorators:** Overlay glyphs rendered on top of base glyphs without a second surface layer
- **Rich typography:** Underlines, strikethrough, box edges, multi-character effects

### Example: IBM_ext.font

**File:** `Fonts/IBM_ext.font`

```json
{
    "$type": "SadConsole.SadFont, SadConsole",
    "Name": "IBM_8x16_ext",
    "FilePath": "IBM8x16_NoPadding_extended.png",
    "GlyphHeight": 16,
    "GlyphWidth": 8,
    "SolidGlyphIndex": 219,
    "IsSadExtended": true,
    "GlyphDefinitions": {
        "underline": { "Glyph": 95, "Mirror": 0 },
        "strikethrough": { "Glyph": 196, "Mirror": 0 },
        "box-edge-left": { "Glyph": 256, "Mirror": 0 },
        ...
    }
}
```

The spritesheet contains glyphs up to index 256–275 (for the box edges and other extended characters).

---

## 7. CellDecorator — Overlay Glyphs

**File:** `SadConsole/CellDecorator.cs`

A lightweight struct that represents an **overlay glyph** rendered on top of a cell's base glyph.

```csharp
public readonly struct CellDecorator : IEquatable<CellDecorator>
{
    public readonly Color Color { get; init; }
    public readonly int Glyph { get; init; }
    public readonly Mirror Mirror { get; init; }
    
    public CellDecorator(Color color, int glyph, Mirror mirror) { ... }
    
    public static CellDecorator Empty => default;
}
```

**Purpose:** Attach secondary glyphs to cells without requiring extra surface layers. Example: adding an underline decorator to a character.

### Cell Decorator Usage

Each `ColoredGlyphBase` can hold a `List<CellDecorator>?`:

```csharp
public abstract partial class ColoredGlyphBase
{
    public List<CellDecorator>? Decorators { get; set; }
    ...
}
```

The renderer:
1. Draws the base glyph at the cell's foreground color.
2. For each decorator in `Decorators`, draws the decorator glyph at the decorator's color on top.

### Creating Decorators from Glyph Definitions

```csharp
CellDecorator decorator = font.GetDecorator("underline", Color.Yellow);
// Internally: GlyphDefinition def = GlyphDefinitions["underline"];
//             return new CellDecorator(Color.Yellow, def.Glyph, def.Mirror);
```

---

## 8. Mirror Enum

**File:** `SadConsole/GlyphMirror.cs`

A flags enum that controls glyph flip/mirror transformations during rendering.

```csharp
[Flags]
public enum Mirror
{
    None = 0,              // No flipping
    Vertical = 1,          // Flip vertically (upside-down)
    Horizontal = 2,        // Flip horizontally (left-right)
}
```

**Combinable:** `Mirror.Vertical | Mirror.Horizontal` flips both axes (180° rotation).

Used on:
- `ColoredGlyphBase.Mirror` (per-cell)
- `GlyphDefinition.Mirror` (in extended fonts)
- `CellDecorator.Mirror` (on overlay glyphs)

---

## 9. Surfaces Reference Fonts

### ScreenSurface Font Model

**File:** `SadConsole/ScreenSurface.cs`

Each `ScreenSurface` holds:

```csharp
public IFont Font { get; set; }       // The atlas
public Point FontSize { get; set; }   // Pixel cell dimensions
```

#### Font Changes

When either property changes:
1. `OnFontChanged(IFont oldFont, Point oldFontSize)` is called (virtual; overridable).
2. `IsDirty = true` (triggers renderer refresh).
3. `CallOnHostUpdated()` → renderer's `OnHostUpdated(IScreenObject host)` (for texture reallocation if needed).

**Note:** `UpdateAbsolutePosition()` is **not** called automatically. Caller is responsible for re-layout.

#### Coordinate System Impact

In **cell-mode** (default):
```csharp
AbsolutePosition = Position * FontSize + ParentAbsolutePosition
```

A `ScreenSurface` at cell position `(5, 3)` with `FontSize = (8, 16)` is at pixel `(40, 48)` plus parent offset.

Switching to **pixel-mode** (`UsePixelPositioning = true`):
```csharp
AbsolutePosition = Position  // Position is already in pixels
```

---

## 10. The .font File Format

### Structure

A `.font` file is a **JSON file** containing a serialized `SadFont` object.

#### Minimal Example (IBM.font)

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

#### Full Example with Extended Definitions (IBM_ext.font)

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
        "box-edge-left": { "Glyph": 256, "Mirror": 0 },
        ...
    }
}
```

### Field Mappings

| JSON Field | SadFont Property | Purpose |
|------------|------------------|---------|
| `$type` | (type metadata) | Required; must be `"SadConsole.SadFont, SadConsole"` |
| `Name` | `Name` | Font identifier string |
| `FilePath` | `FilePath` | Path to PNG spritesheet (relative or `res:...` for embedded) |
| `GlyphWidth` | `GlyphWidth` | Pixel width per glyph |
| `GlyphHeight` | `GlyphHeight` | Pixel height per glyph |
| `GlyphPadding` | `GlyphPadding` | Pixel spacing between tiles |
| `SolidGlyphIndex` | `SolidGlyphIndex` | Index of solid white glyph |
| `UnsupportedGlyphIndex` | `UnsupportedGlyphIndex` | Index for missing glyphs (optional, defaults to 0) |
| `Columns` | `Columns` | Glyphs per row in spritesheet (optional; auto-calculated on load) |
| `Rows` | `Rows` | Glyphs per column in spritesheet (optional; auto-calculated on load) |
| `IsSadExtended` | `IsSadExtended` | Boolean; enables glyph definitions (optional, defaults to false) |
| `GlyphDefinitions` | `GlyphDefinitions` | Object mapping names to `{ Glyph, Mirror }` (optional) |

### Deserialization Process

1. **JSON parse** → `SadFont` object via `Newtonsoft.Json.JsonConvert.DeserializeObject`.
2. **OnDeserialized callback** fires:
   - Load `ITexture Image` from `FilePath` (host-provided).
   - Auto-calculate `Columns` / `Rows` if missing (using image dimensions).
   - Call `ConfigureRects()` to generate rectangles.
3. **Result:** Fully initialized `SadFont` ready to use.

### Embedded Resource Fonts

Fonts can reference embedded resources in the SadConsole assembly:

```json
"FilePath": "res:SadConsole.Resources.IBM8x16.png"
```

During deserialization, the `res:` prefix is stripped, and the assembly resource stream is loaded:

```csharp
if (FilePath.StartsWith("res:"))
{
    using (Stream fontStream = typeof(SadFont).Assembly.GetManifestResourceStream(FilePath.Substring(4))!)
        Image = GameHost.Instance.GetTexture(fontStream);
}
```

---

## 11. Font-Related Events and Event Args

### FontChangedEventArgs

**File:** `SadConsole/FontChangedEventArgs.cs`

```csharp
public class FontChangedEventArgs : EventArgs
{
    public IFont OldFont { get; }
    public IFont NewFont { get; }
}
```

Raised by `GameHost.DefaultFont` property when changed.

### FontSizeChangedEventArgs

**File:** `SadConsole/FontSizeChangedEventArgs.cs`

```csharp
public class FontSizeChangedEventArgs : EventArgs
{
    public IFont.Sizes OldSize { get; }
    public IFont.Sizes NewSize { get; }
}
```

Raised by `GameHost.DefaultFontSize` property when changed.

### GameHost Font Events

```csharp
public event EventHandler<FontChangedEventArgs>? DefaultFontChanged;
public event EventHandler<FontSizeChangedEventArgs>? DefaultFontSizeChanged;
```

Fired when the global default font/size changes (e.g., during host initialization or runtime switches).

---

## 12. Font Extensions

**File:** `SadConsole/Font.Extensions.cs`

Utility methods for font-related calculations:

```csharp
public static class FontExtensions
{
    // Convert cell position to pixel position
    public static Rectangle GetRenderRect(this IFont font, int x, int y, Point fontSize)
        => new(x * fontSize.X, y * fontSize.Y, fontSize.X, fontSize.Y);
    
    // Get pixel position of a cell
    public static Point GetWorldPosition(this IFont font, Point position, Point fontSize)
        => new(position.X * fontSize.X, position.Y * fontSize.Y);
    
    // Get aspect ratio
    public static (float X, float Y) GetGlyphRatio(this IFont font, Point fontSize)
        => ((float)fontSize.X / fontSize.Y, (float)fontSize.Y / fontSize.X);
}
```

---

## 13. Font Serialization (for Surfaces)

**File:** `SadConsole/SerializedTypes/Font.cs`

When a `ScreenSurface` is serialized (e.g., to save game state), its font reference is handled by a custom JSON converter:

```csharp
public class FontJsonConverter : JsonConverter<IFont>
{
    public override void WriteJson(JsonWriter writer, IFont value, JsonSerializer serializer)
        => serializer.Serialize(writer, FontSerialized.FromFont(value));
    
    public override IFont ReadJson(JsonReader reader, Type objectType, IFont existingValue, ...)
        => FontSerialized.ToFont(serializer.Deserialize<FontSerialized>(reader));
}

public class FontSerialized
{
    public string Name;
    
    public static FontSerialized FromFont(IFont font) 
        => new() { Name = font.Name };
    
    public static IFont ToFont(FontSerialized font)
    {
        if (font == null) return null;
        return GameHost.Instance.Fonts.ContainsKey(font.Name) 
            ? GameHost.Instance.Fonts[font.Name] 
            : GameHost.Instance.DefaultFont;
    }
}
```

**Strategy:** Store only the **font name**, and look it up at deserialization time from `GameHost.Fonts`. This avoids serializing the entire spritesheet texture.

---

## 14. Key File Reference

### Core Font Types

| File | Contents |
|------|----------|
| `SadConsole/IFont.cs` | `IFont` interface contract (8 KiB) |
| `SadConsole/SadFont.cs` | Concrete sealed `SadFont` implementation (12 KiB) |
| `SadConsole/GlyphDefinition.cs` | Named glyph struct for extended fonts (1 KiB) |
| `SadConsole/CellDecorator.cs` | Overlay glyph struct (2 KiB) |
| `SadConsole/GlyphMirror.cs` | Mirror flags enum (1 KiB) |
| `SadConsole/Font.Extensions.cs` | Utility extensions (1 KiB) |

### Font Loading & Registration

| File | Contents |
|------|----------|
| `SadConsole/GameHost.cs` | `LoadFont()`, `LoadDefaultFonts()`, `Fonts` dict (partial, ~200 lines) |
| `SadConsole/Configuration/FontConfig.cs` | Font configuration builder (6 KiB) |

### Font Change Notifications

| File | Contents |
|------|----------|
| `SadConsole/FontChangedEventArgs.cs` | Font change event args (1 KiB) |
| `SadConsole/FontSizeChangedEventArgs.cs` | Font size change event args (1 KiB) |

### Surface Integration

| File | Contents |
|------|----------|
| `SadConsole/ScreenSurface.cs` | `Font` and `FontSize` properties, `OnFontChanged()` hook (partial, ~150 lines) |

### Serialization

| File | Contents |
|------|----------|
| `SadConsole/SerializedTypes/Font.cs` | `FontJsonConverter` for surface deserialization (1 KiB) |

### Resources

| File | Contents |
|------|----------|
| `SadConsole/Resources/IBM.font` | Embedded standard font metadata |
| `SadConsole/Resources/IBM_ext.font` | Embedded extended font metadata + glyph definitions |

---

## 15. Design Principles and Patterns

### Core-Host Separation in Font System

**Core (`SadConsole/`):**
- `IFont` interface (contract)
- `SadFont` sealed implementation (metadata: glyph layout, definitions, rectangles)
- `GlyphDefinition`, `CellDecorator`, `Mirror` (value types)
- Font registration in `GameHost.Fonts`
- `.font` file parsing (JSON structure)

**Host (`SadConsole.Host.*/`):**
- `ITexture` implementation (GPU texture loading, management)
- Asset loading from disk or embedded resources
- Rendering: maps glyphs to texture atlas and blit to screen

**Implication:** The core never performs GPU operations. Fonts are **data + metadata**, not GPU assets.

### Immutability and Mutability

**Immutable (read-only):**
- `GlyphDefinition` (struct, init properties)
- `CellDecorator` (struct, init properties)
- `Mirror` (enum)

**Mutable:**
- `SadFont` (class; supports post-construction modification)
- `IFont.GlyphPadding` (settable)
- `IFont.SolidGlyphIndex`, `IFont.UnsupportedGlyphIndex` (settable)
- `IFont.Image` (settable; allows texture swaps)

### Serialization Strategy

**Font files (`.font`):** JSON with `$type` field. Full deserialization of metadata + texture loading on demand.

**Surface serialization:** Store only font **name**, resolve to instance from `GameHost.Fonts` at load time. Avoids duplicating spritesheet data.

### Glyph Lookup Strategy

**Standard approach:**
```csharp
Rectangle rect = font.GetGlyphSourceRectangle(glyphIndex);
// Returns GlyphRectangles[glyphIndex] or UnsupportedGlyphRectangle if not found
```

**For extended fonts:**
```csharp
GlyphDefinition def = font.GetGlyphDefinition("underline");
if (def != GlyphDefinition.Empty)
{
    CellDecorator deco = def.CreateCellDecorator(Color.Yellow);
    // Use deco.Glyph and deco.Mirror when rendering
}
```

---

## 16. Known Limitations and Notes

### 1. Font Size Scaling

- Size scaling is **logical** (applies multipliers during rendering), not **physical** (separate bitmap files).
- Quarter and Half scales may show artifacts due to integer division and pixel-level rounding.
- No built-in anti-aliasing for scaled fonts.

### 2. Glyph Remapping

- The `_remapper` field (legacy serialized data) supports remapping individual glyphs to different indices.
- Applies **after** deserialization, overriding auto-generated rectangles.
- Intended for fonts that have irregular layouts (e.g., old TheDraw fonts).

### 3. Extended Font Scope

- `IsSadExtended` is a **flag**, not a guarantee of feature support on the host side.
- Hosts must verify the flag and handle `GlyphDefinition` lookups and `CellDecorator` rendering.
- The core provides the data structure; hosts implement the behavior.

### 4. No Built-in Color Mapping

- Fonts are **monochrome spritesheets** (grayscale/white glyphs).
- Colors are applied at **render time** via `ColoredGlyph.Foreground` / `ColoredGlyph.Background`.
- No per-glyph color palettes or font-level color translation.

### 5. Image Disposal

- `SadFont.Dispose()` disposes the `Image` texture.
- It is the caller's responsibility to ensure fonts are not disposed while still in use.
- No reference counting or lifetime management on the core side.

---

## 17. Potential Extension Points

### Custom IFont Implementations

Host or library code can implement `IFont` directly to provide:
- Procedurally generated fonts
- Runtime font atlasing
- Multi-layer glyphs (baked into spritesheet)
- Alternative serialization formats (binary, YAML, etc.)

### Custom Render Steps for Font Features

Hosts can add `IRenderStep` instances to enable:
- Font drop shadows
- Outline effects
- Animation of individual glyphs
- Colored glyph replacement (e.g., tinting specific characters)

### GlyphDefinition Extensions

Extend `GlyphDefinition` usage beyond the core by:
- Adding custom properties (e.g., animation data)
- Defining libraries of predefined decorators for common themes

---

## Summary for Deckard

The **SadConsole font system** is a clean separation of concerns:

1. **IFont + SadFont** — Metadata layer (glyph mapping, definitions, sizing).
2. **GlyphDefinition + CellDecorator** — Extended feature types (named glyphs, overlays).
3. **GameHost.Fonts** — Central registry; loaded and resolved during deserialization.
4. **.font JSON format** — Human-editable, version-agnostic asset definition.
5. **ScreenSurface font/fontSize** — Independent, scalable rendering properties.

**Strengths:**
- Clean core/host boundary (hosts handle GPU textures).
- No circular dependencies or tight coupling.
- Easy to extend (custom IFont, custom render steps).
- Serialization avoids embedding textures (name-based resolution).

**Weaknesses (if any):**
- No built-in color palettes or dynamic color mapping.
- Size scaling is logical (rounding artifacts possible).
- Legacy remapping code path (rarely used, but still present).

This architecture scales well for both simple 2D games and complex UI systems.


# Font Loading and Rendering Architecture — Deep Analysis

**Author:** Gaff (Host Dev)  
**Date:** 2026-02-25  
**Scope:** All host implementations (MonoGame, SFML, FNA, KNI) + shared code  
**Status:** Complete analysis of font pipeline across all hosts

---

## Overview

Fonts in SadConsole are sprite-sheet based texture atlases, where each glyph is a fixed-size tile in a 2D grid. The rendering pipeline maps Unicode character indices to pixel rectangles on the font texture and draws them during cell rendering. The architecture is **host-specific** for texture loading and rendering but uses a **host-agnostic** interface and metadata (`IFont`, `SadFont`) for registration and access.

---

## 1. Font Data Model

### 1.1 IFont Interface (Core)

**File:** `SadConsole/IFont.cs`

The host-independent font contract:

```csharp
public interface IFont: IDisposable
{
    string Name { get; }                               // Registration ID
    int GlyphWidth { get; }                            // Pixel width of each tile
    int GlyphHeight { get; }                           // Pixel height of each tile
    int GlyphPadding { get; set; }                     // Spacing between tiles in sheet
    int TotalGlyphs { get; }                           // Columns × Rows
    int SolidGlyphIndex { get; set; }                  // All-white fill glyph (for backgrounds)
    Rectangle SolidGlyphRectangle { get; }             // Precomputed rect for solid glyph
    int UnsupportedGlyphIndex { get; set; }            // Fallback for unmapped glyphs
    Rectangle UnsupportedGlyphRectangle { get; }       // Precomputed rect
    bool IsSadExtended { get; set; }                   // True if font supports CellDecorator names
    ITexture Image { get; set; }                       // The sprite sheet (host-specific)
    Rectangle GetGlyphSourceRectangle(int glyph);      // Maps glyph index → sheet rectangle
    Dictionary<int, Rectangle> GlyphRectangles { get; }// Precomputed glyph→rect mapping
    Dictionary<string, GlyphDefinition> GlyphDefinitions { get; }  // Named glyphs for decorators
    Point GetFontSize(Sizes size);                     // Quarter/Half/One/Two/Three/Four
}
```

**Key insight:** The interface is decoupled from texture type; `ITexture Image` is host-specific but accessed through a uniform API.

### 1.2 SadFont Implementation (Core)

**File:** `SadConsole/SadFont.cs`

The concrete implementation:

```csharp
public sealed class SadFont : IFont
{
    public int Columns { get; set; }                   // Tiles wide
    public int Rows { get; set; }                      // Tiles tall
    public string FilePath { get; set; }               // Load source (res: or file path)
    public Dictionary<int, Rectangle> GlyphRectangles { get; set; }  // Sparse or dense mapping
    public Dictionary<string, GlyphDefinition> GlyphDefinitions { get; set; }
    
    public Rectangle GetGlyphSourceRectangle(int glyph)
    {
        if (GlyphRectangles.TryGetValue(glyph, out var rect))
            return rect;
        return UnsupportedGlyphRectangle;
    }
    
    public Rectangle GenerateGlyphSourceRectangle(int glyph)
    {
        // Compute rect from grid index: glyph % Columns, glyph / Columns
        int cx = glyph % Columns;
        int cy = glyph / Columns;
        
        if (GlyphPadding != 0)
            return new Rectangle(
                (cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
                (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
                GlyphWidth, GlyphHeight);
        else
            return new Rectangle(cx * GlyphWidth, cy * GlyphHeight, GlyphWidth, GlyphHeight);
    }
    
    public void ConfigureRects()
    {
        // Populate GlyphRectangles dictionary if empty
        if (GlyphRectangles.Count == 0)
            ForceConfigureRects();
    }
    
    public void ForceConfigureRects()
    {
        // Rebuild all glyph rects from Columns, Rows, GlyphWidth, GlyphHeight, GlyphPadding
        for (int i = 0; i < Rows * Columns; i++)
            GlyphRectangles[i] = GenerateGlyphSourceRectangle(i);
    }
}
```

### 1.3 Font File Format

**Example:** `SadConsole/Resources/IBM.font`

```json
{
    "Name": "IBM_8x16",
    "FilePath": "res:SadConsole.Resources.IBM8x16.png",
    "GlyphHeight": 16,
    "GlyphPadding": 1,
    "GlyphWidth": 8,
    "SolidGlyphIndex": 219,
    "Columns": 16
}
```

**Key fields:**
- `FilePath`: Either `res:ASSEMBLY_RESOURCE_NAME` or relative path. Resolved during deserialization.
- `Columns`: Required. If `Rows` is missing, computed from texture height: `Image.Height / (GlyphHeight + GlyphPadding)`.
- `GlyphPadding`: Spacing in pixels between tiles. Affects rectangle calculation.
- `SolidGlyphIndex`: Must point to a fully white (or near-white) glyph for background fills.

---

## 2. Font Loading Pipeline

### 2.1 GameHost Font Registration

**File:** `SadConsole/GameHost.cs`

Central host interface for font access:

```csharp
public abstract partial class GameHost : IDisposable
{
    public Dictionary<string, IFont> Fonts { get; } = new();
    public SadFont EmbeddedFont { get; internal set; }          // IBM_8x16
    public SadFont EmbeddedFontExtended { get; internal set; }  // IBM_ext (with decorators)
    public IFont DefaultFont { get; set; }                      // Active font for new surfaces
    public IFont.Sizes DefaultFontSize { get; set; }
    
    protected void LoadDefaultFonts(string? defaultFont)
    {
        // Load embedded fonts from assembly
        EmbeddedFont = LoadResourceFont("SadConsole.Resources.IBM.font");
        EmbeddedFontExtended = LoadResourceFont("SadConsole.Resources.IBM_ext.font");
        
        // Set default based on settings or explicit parameter
        DefaultFont = (defaultFont != null) 
            ? LoadFont(defaultFont) 
            : Settings.UseDefaultExtendedFont 
                ? EmbeddedFontExtended 
                : EmbeddedFont;
    }
    
    public IFont LoadFont(string fontFilePath)
    {
        var font = Serializer.Load<IFont>(fontFilePath, false, jsonSettings);
        if (Fonts.TryGetValue(font.Name, out var existing))
            return existing;
        Fonts.Add(font.Name, font);
        return font;
    }
    
    private SadFont LoadResourceFont(string fontName)
    {
        // Load from assembly manifest (embedded resources)
        Assembly assembly = typeof(SadFont).Assembly;
        using var stream = assembly.GetManifestResourceStream(fontName);
        var font = JsonConvert.DeserializeObject<SadFont>(new StreamReader(stream).ReadToEnd(), jsonSettings);
        Fonts.Add(font.Name, font);
        return font;
    }
}
```

### 2.2 Font Deserialization & Texture Loading

**File:** `SadConsole/SadFont.cs` — `AfterDeserialized` callback

```csharp
[OnDeserialized]
private void AfterDeserialized(StreamingContext context)
{
    if (FilePath.StartsWith("res:"))
    {
        // Load from embedded assembly resource
        using var fontStream = typeof(SadFont).Assembly
            .GetManifestResourceStream(FilePath.Substring(4));
        Image = GameHost.Instance.GetTexture(fontStream);
    }
    else
    {
        // Load from file system, relative to SerializerPathHint (directory of .font file)
        string fullPath = Path.Combine(GameHost.SerializerPathHint, FilePath);
        Image = GameHost.Instance.GetTexture(fullPath);
    }
    
    // Compute grid dimensions if missing
    if (Columns == 0)
        Columns = (int)Math.Floor((double)Image.Width / (GlyphWidth + GlyphPadding));
    if (Rows == 0)
        Rows = (int)Math.Floor((double)Image.Height / (GlyphHeight + GlyphPadding));
    
    // Populate GlyphRectangles if empty (auto-generate grid layout)
    if (!_skipAutomaticGlyphGeneration)
        ConfigureRects();
    
    // Apply glyph index remapping if present
    if (_remapper != null)
    {
        foreach (var mapping in _remapper)
            GlyphRectangles[mapping.From] = GenerateGlyphSourceRectangle(mapping.To);
        _remapper = null;
    }
}
```

**Critical flow:**
1. `.font` JSON is deserialized into `SadFont`.
2. `AfterDeserialized` fires automatically during JSON parsing.
3. `FilePath` is resolved to load the actual PNG/image file.
4. `GameHost.Instance.GetTexture(path)` is called — **host-specific implementation**.
5. `Image` property set to host-specific `ITexture` wrapper.
6. Grid layout is computed and `GlyphRectangles` is built.

---

## 3. Host-Specific Texture Loading

Each host implements `GameHost.GetTexture()` and wraps the graphics API texture in `ITexture`:

### 3.1 MonoGame Host

**Files:**
- `SadConsole.Host.MonoGame/Game.Mono.cs`
- `SadConsole.Host.MonoGame/GameTexture.cs`

```csharp
// SadConsole.Host.MonoGame/Game.Mono.cs
public override ITexture GetTexture(string resourcePath) =>
    new Host.GameTexture(resourcePath);

public override ITexture GetTexture(Stream textureStream) =>
    new Host.GameTexture(textureStream);

public override ITexture CreateTexture(int width, int height) =>
    new Host.GameTexture(width, height);
```

**GameTexture implementation:**

```csharp
public partial class GameTexture : ITexture
{
    public Microsoft.Xna.Framework.Graphics.Texture2D Texture { get; private set; }
    
    // Constructor from file path
    public GameTexture(string path)
    {
        using Stream fontStream = SadConsole.Game.Instance.OpenStream(path);
        _texture = Texture2D.FromStream(Global.GraphicsDevice, fontStream);
        _resourcePath = path;
        Size = _texture.Width * _texture.Height;
    }
    
    // Constructor from stream
    public GameTexture(Stream stream)
    {
        _texture = Texture2D.FromStream(Global.GraphicsDevice, stream);
        Size = _texture.Width * _texture.Height;
    }
    
    // Constructor with size (creates empty texture)
    public GameTexture(int width, int height)
    {
        _texture = new Texture2D(Global.GraphicsDevice, width, height);
        Size = width * height;
    }
    
    // Constructor wrapping existing Texture2D
    public GameTexture(Texture2D texture, bool handleDispose = false)
    {
        _skipDispose = !handleDispose;
        _texture = texture;
        Size = _texture.Width * _texture.Height;
    }
}
```

**Features:**
- Transparent creation from file or stream via `Texture2D.FromStream()`.
- Wraps MonoGame `Texture2D` object.
- Pixel access methods: `GetPixels()`, `SetPixels()`, `GetPixel(Point)`, `SetPixel(Point, Color)`.
- Can convert to `RenderTarget2D` for editing (via `Edit_EnableEditing()` extension).

### 3.2 SFML Host

**Files:**
- `SadConsole.Host.SFML/Game.cs`
- `SadConsole.Host.SFML/GameTexture.cs`

```csharp
// SadConsole.Host.SFML/Game.cs
public override ITexture GetTexture(string resourcePath) =>
    new SadConsole.Host.GameTexture(resourcePath);

public override ITexture GetTexture(Stream textureStream) =>
    new SadConsole.Host.GameTexture(textureStream);

public override ITexture CreateTexture(int width, int height) =>
    new Host.GameTexture((uint)width, (uint)height);
```

**GameTexture implementation:**

```csharp
public partial class GameTexture : ITexture
{
    public SFML.Graphics.Texture Texture { get; private set; }
    
    // From file path
    public GameTexture(string path)
    {
        using Stream fontStream = new FileStream(path, FileMode.Open);
        _texture = new Texture(fontStream);
        _resourcePath = path;
        Size = Width * Height;
    }
    
    // From stream
    public GameTexture(Stream stream)
    {
        _texture = new Texture(stream);
        Size = Width * Height;
    }
    
    // Create empty with dimensions
    public GameTexture(uint width, uint height)
    {
        _texture = new Texture(width, height);
        Size = (int)(width * height);
    }
}
```

**Differences from MonoGame:**
- Wraps SFML `Texture` instead of `Texture2D`.
- Constructor takes `uint` for dimensions (SFML convention).
- Direct stream loading: `new Texture(stream)`.

### 3.3 FNA & KNI Hosts

**Status:** FNA and KNI share code structure with MonoGame (both use XNA-compatible APIs).

**FNA:** Uses `Microsoft.Xna.Framework` (FNA fork). GameTexture implementation is nearly identical to MonoGame but compiled against FNA libraries.

**KNI:** Supports multiple backends (including Blazor WebGL). The core game host uses similar patterns with platform-specific texture loading.

---

## 4. Glyph Index to Rectangle Mapping

### 4.1 Grid Layout Calculation

During font initialization, glyph indices are mapped to texture rectangles using a **row-major grid layout**:

```csharp
public Rectangle GenerateGlyphSourceRectangle(int glyph)
{
    int cx = glyph % Columns;        // Column in grid
    int cy = glyph / Columns;        // Row in grid
    
    if (GlyphPadding != 0)
    {
        // Account for padding between tiles
        return new Rectangle(
            (cx * GlyphWidth) + ((cx + 1) * GlyphPadding),
            (cy * GlyphHeight) + ((cy + 1) * GlyphPadding),
            GlyphWidth, GlyphHeight);
    }
    else
    {
        // No padding: tiles are adjacent
        return new Rectangle(
            cx * GlyphWidth, 
            cy * GlyphHeight, 
            GlyphWidth, GlyphHeight);
    }
}
```

**Example (IBM font: 8×16, 16 columns, 1px padding):**
- Glyph 0: `(1×(0+1), 1×(0+1), 8, 16)` = `(1, 1, 8, 16)`
- Glyph 1: `(1×(8)+1×(1+1), 1×(0+1), 8, 16)` = `(10, 1, 8, 16)`
- Glyph 16 (row 1, col 0): `(1×(0+1), 16×(1+1), 8, 16)` = `(1, 33, 8, 16)`

### 4.2 Glyph Rectangles Dictionary

**File:** `SadConsole/SadFont.cs`

```csharp
public Dictionary<int, Rectangle> GlyphRectangles { get; set; }
```

- **Purpose:** Maps glyph index → source rectangle on the font texture.
- **Sparseness:** Can be sparse (only mapped glyphs) or dense (all glyphs 0–TotalGlyphs).
- **Generation:** Populated by `ConfigureRects()` at font load time.
- **Lookup:**
  ```csharp
  public Rectangle GetGlyphSourceRectangle(int glyph)
  {
      if (GlyphRectangles.TryGetValue(glyph, out var rect))
          return rect;
      return UnsupportedGlyphRectangle;  // Fallback for unmapped/invalid indices
  }
  ```

**Extended fonts:** May include named `GlyphDefinition` entries for decorator glyphs used by `CellDecorator`.

---

## 5. Font Rendering at the Host Level

### 5.1 Rendering Pipeline Overview

Each frame per cell:
1. **Cell data:** `cell.Glyph` (int index), `cell.Foreground` (color), `cell.Background` (color), `cell.Decorators` (optional overlay glyphs).
2. **Font lookup:** `font.GetGlyphSourceRectangle(cell.Glyph)` → `Rectangle` in font texture.
3. **Renderer draw:** Blit font texture rect to output texture, colored by foreground.
4. **Decorators:** Repeat for each decorator glyph on top.

### 5.2 MonoGame Render Step

**File:** `SadConsole.Host.MonoGame/Renderers/Steps/SurfaceRenderStep.cs`

```csharp
public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
{
    var monoRenderer = (IRendererMonoGame)renderer;
    
    // Create or resize backing texture if needed
    if (backingTextureChanged || BackingTexture == null || 
        screenObject.AbsoluteArea.Width != BackingTexture.Width || 
        screenObject.AbsoluteArea.Height != BackingTexture.Height)
    {
        BackingTexture = new RenderTarget2D(
            Host.Global.GraphicsDevice, 
            screenObject.AbsoluteArea.Width, 
            screenObject.AbsoluteArea.Height, ...);
    }
    
    if (result || screenObject.IsDirty || isForced)
    {
        Host.Global.GraphicsDevice.SetRenderTarget(BackingTexture);
        monoRenderer.LocalSpriteBatch.Begin(SpriteSortMode.Deferred, 
            monoRenderer.MonoGameBlendState, 
            SamplerState.PointClamp, ...);
        
        IFont font = screenObject.Font;
        Texture2D fontImage = ((Host.GameTexture)font.Image).Texture;
        
        // Render default background if present
        if (screenObject.Surface.DefaultBackground.A != 0)
            monoRenderer.LocalSpriteBatch.Draw(fontImage, 
                new XnaRectangle(0, 0, BackingTexture.Width, BackingTexture.Height), 
                font.SolidGlyphRectangle.ToMonoRectangle(),  // Source: solid white glyph
                screenObject.Surface.DefaultBackground.ToMonoColor(), ...);
        
        int rectIndex = 0;
        
        // Iterate over visible cells
        for (int y = 0; y < screenObject.Surface.View.Height; y++)
        {
            int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) 
                    + screenObject.Surface.ViewPosition.X;
            
            for (int x = 0; x < screenObject.Surface.View.Width; x++)
            {
                ColoredGlyphBase cell = screenObject.Surface[i];
                
                if (cell.IsVisible)
                {
                    // Draw background (if non-transparent and different from default)
                    if (cell.Background != Transparent && cell.Background != DefaultBackground)
                        monoRenderer.LocalSpriteBatch.Draw(fontImage, 
                            monoRenderer.CachedRenderRects[rectIndex],     // Destination: cell position
                            font.SolidGlyphRectangle.ToMonoRectangle(),    // Source: solid glyph
                            cell.Background.ToMonoColor(), ...);
                    
                    // Draw glyph (if non-zero index)
                    if (cell.Glyph != 0 && cell.Foreground != Transparent && ...)
                        monoRenderer.LocalSpriteBatch.Draw(fontImage, 
                            monoRenderer.CachedRenderRects[rectIndex],           // Destination
                            font.GetGlyphSourceRectangle(cell.Glyph).ToMonoRectangle(),  // Source ← GLYPH MAPPING
                            cell.Foreground.ToMonoColor(), ...);
                    
                    // Draw decorators (overlay glyphs)
                    if (cell.Decorators != null)
                        for (int d = 0; d < cell.Decorators.Count; d++)
                            if (cell.Decorators[d].Color != Transparent)
                                monoRenderer.LocalSpriteBatch.Draw(fontImage, 
                                    monoRenderer.CachedRenderRects[rectIndex], 
                                    font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(),  // Decorator glyph
                                    cell.Decorators[d].Color.ToMonoColor(), ...);
                }
                
                i++;
                rectIndex++;
            }
        }
        
        monoRenderer.LocalSpriteBatch.End();
        Host.Global.GraphicsDevice.SetRenderTarget(null);
    }
    
    return result;
}
```

**Key steps:**
1. **Font texture access:** `((Host.GameTexture)font.Image).Texture` → raw graphics API texture.
2. **Glyph rect lookup:** `font.GetGlyphSourceRectangle(cell.Glyph)` → source `Rectangle` on atlas.
3. **Draw call:** `SpriteBatch.Draw(fontTexture, destRect, sourceRect, color, ...)`.
4. **Destination rect:** From cached array `CachedRenderRects[rectIndex]` (precomputed cell positions).
5. **Decorators:** Same process, drawn on top with higher depth value.

### 5.3 SFML Render Step

**File:** `SadConsole.Host.SFML/Renderers/Steps/SurfaceRenderStep.cs`

```csharp
public bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)
{
    if (backingTextureChanged || BackingTexture == null || ...)
    {
        BackingTexture = new RenderTexture((uint)screenObject.AbsoluteArea.Width, 
                                           (uint)screenObject.AbsoluteArea.Height);
    }
    
    if (result || screenObject.IsDirty || isForced)
    {
        BackingTexture.Clear(Color.Transparent);
        Host.Global.SharedSpriteBatch.Reset(BackingTexture, sfmlRenderer.SFMLBlendState, Transform.Identity);
        
        IFont font = screenObject.Font;
        
        // Background fill
        if (screenObject.Surface.DefaultBackground.A != 0)
            Host.Global.SharedSpriteBatch.DrawQuad(
                new IntRect(0, 0, (int)BackingTexture.Size.X, (int)BackingTexture.Size.Y), 
                font.SolidGlyphRectangle.ToIntRect(),  // Source: solid glyph
                screenObject.Surface.DefaultBackground.ToSFMLColor(), 
                ((SadConsole.Host.GameTexture)font.Image).Texture);
        
        int rectIndex = 0;
        
        for (int y = 0; y < screenObject.Surface.View.Height; y++)
        {
            int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) 
                    + screenObject.Surface.ViewPosition.X;
            
            for (int x = 0; x < screenObject.Surface.View.Width; x++)
            {
                ColoredGlyphBase cell = screenObject.Surface[i];
                
                if (cell.IsVisible)
                    Host.Global.SharedSpriteBatch.DrawCell(cell,  // ← Helper method
                        sfmlRenderer.CachedRenderRects[rectIndex], 
                        cell.Background != Transparent && cell.Background != DefaultBackground, 
                        font);
                
                i++;
                rectIndex++;
            }
        }
        
        Host.Global.SharedSpriteBatch.End();
        BackingTexture.Display();  // Finalize render to texture
    }
    
    return result;
}
```

**SFML differences:**
- Uses `RenderTexture` instead of `RenderTarget2D`.
- Calls `BackingTexture.Display()` to finalize (SFML requirement).
- Uses helper `SharedSpriteBatch.DrawCell()` (wraps draw logic).

---

## 6. Font Scaling & Size Management

### 6.1 FontSize Enumeration

**File:** `SadConsole/IFont.cs`

```csharp
public enum Sizes
{
    Quarter = 0,   // 0.25× glyph dimensions
    Half = 1,      // 0.5× glyph dimensions
    One = 2,       // 1.0× glyph dimensions (default)
    Two = 3,       // 2.0× glyph dimensions
    Three = 4,     // 3.0× glyph dimensions
    Four = 5       // 4.0× glyph dimensions
}

public Point GetFontSize(Sizes size)
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

### 6.2 ScreenSurface Font Management

**File:** `SadConsole/ScreenSurface.cs` (partial)

```csharp
public class ScreenSurface : ScreenObject
{
    public IFont Font { get; set; }
    public Point FontSize { get; set; }  // Pixel size per cell, set via GetFontSize()
    
    // When font/size changes:
    protected virtual void OnFontChanged(IFont oldFont, Point oldFontSize)
    {
        IsDirty = true;
        Renderer?.OnHostUpdated(this);  // Notify renderer to realloc if needed
    }
    
    public override void UpdateAbsolutePosition()
    {
        if (UsePixelPositioning)
            AbsolutePosition = Position;
        else
            AbsolutePosition = (FontSize * Position);  // Cell pos → pixel pos
        
        // ... propagate to children
    }
    
    public int WidthPixels => Surface.View.Width * FontSize.X;
    public int HeightPixels => Surface.View.Height * FontSize.Y;
}
```

**Key pattern:**
- `Font` is the atlas (shared across surfaces).
- `FontSize` is the **display scale** (can differ from `Font.GlyphWidth/Height`).
- Multiple surfaces can share one font at different scales.
- `AbsoluteArea` is computed as `WidthPixels` × `HeightPixels`.

### 6.3 Rendering at Non-Native Scale

When `FontSize != (Font.GlyphWidth, Font.GlyphHeight)`, the renderer scales the draw destination:

```csharp
// Example: 8×16 font, displayed at 2× scale
IFont font = screenObject.Font;  // 8×16
Point fontSize = screenObject.FontSize;  // 16×32

// For cell at (0, 0):
Rectangle destRect = new Rectangle(0, 0, fontSize.X, fontSize.Y);  // 16×32

// Still draws from font texture at original glyph rect:
Rectangle srcRect = font.GetGlyphSourceRectangle(glyphIndex);  // 8×16 on atlas

// MonoGame.SpriteBatch handles scaling from src to dest
spriteBatch.Draw(fontTexture, destRect, srcRect, color, ...);
```

**No texture resampling:** Scaling is done at draw time by the graphics API (point sampling for pixel-perfect look).

---

## 7. Host-Specific Features & Limitations

### 7.1 MonoGame

**Strengths:**
- Mature XNA ecosystem. Well-tested texture loading.
- `RenderTarget2D` for off-screen rendering, font editing.
- `SpriteBatch` with fine-grained control (blend states, sort modes, depth).
- Supports Windows, macOS, Linux via MonoGame frameworks.

**Limitations:**
- Texture2D cannot be modified directly after creation (requires pixel array copy).
- No direct GPU texture memory reading (must use `GetData()` which is slow).

**Font editing (MonoGame):**
```csharp
// SadConsole.Host.MonoGame/ExtensionsFont.cs
public static void Edit_EnableEditing(this IFont font)
{
    GameTexture oldTexture = (GameTexture)font.Image;
    
    // Convert to RenderTarget2D for GPU editing
    if (oldTexture.Texture is not RenderTarget2D)
    {
        RenderTarget2D newTexture = new(Global.GraphicsDevice, 
            font.Image.Width, font.Image.Height, 
            false, Global.GraphicsDevice.DisplayMode.Format, 
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        
        newTexture.SetData(oldTexture.GetPixelsMonoColor());
        font.Image = new GameTexture(newTexture, true);
        oldTexture.Dispose();
    }
}
```

### 7.2 SFML

**Strengths:**
- Clean C++ binding. Modern graphics API support.
- Native texture loading from streams/files.
- `RenderTexture` for off-screen rendering.

**Differences:**
- Uses `IntRect` (int coordinates) instead of `Rectangle` (float).
- Texture dimensions are `uint` (unsigned).
- Requires explicit `Display()` call after rendering to texture.

**Note:** SFML host is functionally equivalent to MonoGame for font rendering; differences are API-level, not conceptual.

### 7.3 FNA

**Status:** Identical to MonoGame in terms of font rendering (same XNA API subset).
- Wraps SFML internally on non-Windows platforms.
- MonoGame GameTexture code path is used.

### 7.4 KNI (including Blazor)

**Status:** Partially complete.
- KNI is a lightweight WebGL runtime for Blazor.
- Font rendering is texture-based like other hosts.
- WebGL limitations: texture must be power-of-two on older contexts; some sampling modes unavailable.

---

## 8. Shared Host Infrastructure

### 8.1 Texture Helper Utilities

**File:** `SadConsole.Host.Shared/GameTextureHelpers.cs`

Utility methods for texture-to-surface conversion:

```csharp
public static (Color color, float brightness, bool hasContent) CalculateCellColor(
    Color[] pixels, int startX, int startY, int fontSizeX, int fontSizeY, 
    int textureWidth, Color colorKey)
{
    // Average color of a rectangular region, weighted by alpha
    // Used to convert arbitrary textures to cell-based representations
}

public static void ProcessForegroundCell(
    ICellSurface surface, int w, int h, Color color, float brightness,
    TextureConvertForegroundStyle style, ...)
{
    // Map cell color/brightness to glyph index (block chars, ASCII symbols, etc.)
}
```

Not directly used in font loading, but part of the texture infrastructure.

### 8.2 Renderer Base Classes

**File:** `SadConsole.Host.Shared/Renderers/`

- `LayeredRenderer.cs` — Shared implementation for `LayeredScreenSurface`.
- `WindowRenderer.cs` — Shared final-output renderer.

These are **shared by design** to avoid duplication across MonoGame/SFML/FNA.

---

## 9. Extended Fonts & Cell Decorators

### 9.1 Extended Font Support

**File:** `SadConsole/IFont.cs` → `IsSadExtended` property

```csharp
public bool IsSadExtended { get; set; }
public Dictionary<string, GlyphDefinition> GlyphDefinitions { get; }

public CellDecorator GetDecorator(string name, Color color)
{
    if (GlyphDefinitions.ContainsKey(name))
        return GlyphDefinitions[name].CreateCellDecorator(color);
    return CellDecorator.Empty;
}
```

**GlyphDefinition:**
- Binds a string name (e.g., `"underline"`, `"strikethrough"`) to a glyph index.
- Created by font tools during font design.
- Used by `CellDecorator` (overlay glyphs on cell).

**Example:**
```csharp
cell.Decorators = new List<CellDecorator> { font.GetDecorator("underline", Color.Blue) };
```

### 9.2 Rendering with Decorators

See **Section 5.2** (MonoGame Render Step):
```csharp
if (cell.Decorators != null)
    for (int d = 0; d < cell.Decorators.Count; d++)
        monoRenderer.LocalSpriteBatch.Draw(fontImage, 
            monoRenderer.CachedRenderRects[rectIndex], 
            font.GetGlyphSourceRectangle(cell.Decorators[d].Glyph).ToMonoRectangle(),  
            cell.Decorators[d].Color.ToMonoColor(), ...);
```

Each decorator glyph is drawn on top of the base cell, with its own color and glyph index lookup.

---

## 10. Font Registration & Default Font

### 10.1 GameHost Font Management

```csharp
public class GameHost
{
    public Dictionary<string, IFont> Fonts { get; }  // Name → Font lookup
    public IFont DefaultFont { get; set; }            // Active font for new surfaces
    public IFont.Sizes DefaultFontSize { get; set; }  // Active font size
    
    public SadFont EmbeddedFont { get; internal set; }          // IBM_8x16
    public SadFont EmbeddedFontExtended { get; internal set; }  // IBM_ext
}

// On host startup (called by host implementation):
protected void LoadDefaultFonts(string? defaultFont)
{
    EmbeddedFont = LoadResourceFont("SadConsole.Resources.IBM.font");
    EmbeddedFontExtended = LoadResourceFont("SadConsole.Resources.IBM_ext.font");
    
    DefaultFont = (defaultFont != null) 
        ? LoadFont(defaultFont) 
        : Settings.UseDefaultExtendedFont 
            ? EmbeddedFontExtended 
            : EmbeddedFont;
}
```

### 10.2 ScreenSurface Font Assignment

```csharp
public class ScreenSurface : ScreenObject
{
    private IFont _font;
    
    public IFont Font
    {
        get => _font;
        set
        {
            if (_font == value) return;
            IFont oldFont = _font;
            _font = value;
            OnFontChanged(oldFont, FontSize);
        }
    }
}

// During construction:
public ScreenSurface(int width, int height, IFont? font = null)
{
    Font = font ?? GameHost.Instance.DefaultFont;
    FontSize = Font.GetFontSize(GameHost.Instance.DefaultFontSize);
    Renderer = GameHost.Instance.GetRenderer(DefaultRendererName);
}
```

**Pattern:**
- Surfaces default to `GameHost.DefaultFont` if not specified.
- Font can be changed at runtime; triggers `IsDirty` and renderer reallocation.

---

## 11. GameTexture Type Relationships

### 11.1 ITexture Interface

**File:** `SadConsole/ITexture.cs`

```csharp
public interface ITexture : IDisposable
{
    int Width { get; }
    int Height { get; }
    int Size { get; }  // Width × Height
    string ResourcePath { get; }
    
    Rectangle GetGlyphSourceRectangle(int glyph);
    
    Color[] GetPixels();
    void SetPixels(Color[] pixels);
    void SetPixel(Point position, Color color);
    Color GetPixel(Point position);
    
    ICellSurface ToSurface(...);  // Texture → cell surface conversion
}
```

### 11.2 Host Texture Implementations

| Host | Class | Underlying Type | Notes |
|------|-------|-----------------|-------|
| MonoGame | `SadConsole.Host.GameTexture` | `Microsoft.Xna.Framework.Graphics.Texture2D` | Or `RenderTarget2D` for editing |
| SFML | `SadConsole.Host.GameTexture` | `SFML.Graphics.Texture` | Same class name, different namespace |
| FNA | `SadConsole.Host.GameTexture` | `Microsoft.Xna.Framework.Graphics.Texture2D` (FNA) | Code-compatible with MonoGame |
| KNI | TBD | WebGL texture | Platform-specific |

### 11.3 Font Texture Lifecycle

```
.font (JSON)
    ↓ [Deserialization]
SadFont instance (IFont)
    ↓ [AfterDeserialized → FilePath resolution]
GameHost.GetTexture(path/stream)
    ↓ [Host-specific]
GameTexture wrapper (ITexture)
    ↓ [Stores underlying graphics API texture]
Texture2D (MonoGame) / SFML.Texture / etc.
    ↓ [During rendering]
SpriteBatch.Draw(fontTexture, destRect, srcRect, color, ...)
    ↓ [GPU rasterization]
Output framebuffer
```

---

## 12. Key Behaviors & Constraints

### 12.1 Glyph Index Out-of-Bounds

```csharp
public Rectangle GetGlyphSourceRectangle(int glyph)
{
    if (GlyphRectangles.TryGetValue(glyph, out var rect))
        return rect;
    return UnsupportedGlyphRectangle;  // Fallback
}
```

- Any glyph index not in `GlyphRectangles` → rendered as `UnsupportedGlyphIndex` glyph.
- Prevents crashes from invalid glyph references.
- Default `UnsupportedGlyphIndex` is typically a placeholder glyph (e.g., '?').

### 12.2 Texture Ownership & Disposal

**MonoGame:**
```csharp
public GameTexture(Texture2D texture, bool handleDispose = false)
{
    _skipDispose = !handleDispose;
    _texture = texture;
}

public void Dispose()
{
    if (!_skipDispose)
        _texture?.Dispose();
}
```

- If `handleDispose = false`, wrapping object does NOT dispose underlying texture.
- Fonts loaded from disk are wrapped with `handleDispose = true` (owned).
- Embedded resources or shared textures may be wrapped with `handleDispose = false`.

### 12.3 Viewport Scrolling & Glyph Rendering

```csharp
for (int y = 0; y < screenObject.Surface.View.Height; y++)
{
    int i = ((y + screenObject.Surface.ViewPosition.Y) * screenObject.Surface.Width) 
            + screenObject.Surface.ViewPosition.X;
    
    for (int x = 0; x < screenObject.Surface.View.Width; x++)
    {
        // Render cell at index i
        // Destination rect is computed relative to view (0, 0), not absolute surface pos
        i++;
    }
}
```

- Renderer reads only the visible viewport.
- Glyph rects remain the same (always from font atlas).
- Destination rects adjust for viewport position.

---

## 13. Cross-Host Consistency

### 13.1 Invariants All Hosts Must Maintain

1. **Font grid layout must be identical:**
   - `Rectangle GenerateGlyphSourceRectangle(int)` logic is **identical across all hosts**.
   - Glyph 0 always maps to grid position (0, 0).
   - Grid is row-major: glyph index = `row * Columns + column`.

2. **Texture loading must preserve image data:**
   - Any PNG/image format loadable by one host must load identically in others.
   - Color values must be preserved (RGBA).
   - Padding layout in texture must match font metadata.

3. **Rendering must be deterministic:**
   - Same cell + glyph + color on all hosts → visually identical output (modulo anti-aliasing).
   - No host-specific visual distortions in glyph rendering.

### 13.2 Known Differences

| Aspect | MonoGame | SFML | FNA | KNI |
|--------|----------|------|-----|-----|
| Texture type | `Texture2D` | `SFML.Texture` | `Texture2D` (FNA) | WebGL texture |
| Rectangle type | `float`-based | `int`-based | `float`-based | Host-specific |
| Font editing | `RenderTarget2D` | `RenderTexture` | `RenderTarget2D` | Not yet implemented |
| Sampler | `PointClamp` | Auto | `PointClamp` | WebGL default |
| Blend mode | `BlendState` enum | `BlendMode` enum | `BlendState` enum | WebGL blend |

None of these affect the core font glyph-to-rect mapping or rendering logic.

---

## 14. Summary: Font Architecture

| Component | Location | Role |
|-----------|----------|------|
| **IFont** | `SadConsole/IFont.cs` | Host-independent font contract |
| **SadFont** | `SadConsole/SadFont.cs` | Concrete font class; JSON-deserializable |
| **GameTexture** | `SadConsole.Host.*/GameTexture.cs` | Host-specific texture wrapper |
| **GameHost.LoadFont()** | `SadConsole/GameHost.cs` | Font registration and loading |
| **SadFont.AfterDeserialized()** | `SadConsole/SadFont.cs` | Texture loading hook |
| **SurfaceRenderStep** | `SadConsole.Host.*/Renderers/Steps/SurfaceRenderStep.cs` | Per-cell glyph rendering |
| **CachedRenderRects** | Renderer property | Precomputed destination rectangles |

**Data flow:**
```
.font JSON → SadFont deserialization → FilePath resolution → GameHost.GetTexture() → GameTexture wrapper → Font.GetGlyphSourceRectangle() → SpriteBatch.Draw() → Screen
```

---

## 15. Testing & Validation Notes

For Deckard's architecture document and Rachael's test suite:

### 15.1 Coverage Gaps
- No automated tests for font loading across hosts.
- No regression tests for glyph rect calculation.
- Extended font decorator rendering is not tested.
- Font scaling (non-native sizes) is untested.

### 15.2 Edge Cases
1. **Zero-padding fonts:** GlyphPadding = 0; rectangles must be adjacent without gaps.
2. **Sparse glyph mappings:** Only some indices mapped; others fall back to unsupported glyph.
3. **Large glyphs:** 32×32 or larger; must not overflow texture dimensions.
4. **Unicode edge glyphs:** Glyph indices near or above TotalGlyphs.
5. **Shared font at multiple scales:** Font used by multiple surfaces at different FontSize values.

### 15.3 Host-Specific Validation
- MonoGame: Ensure `RenderTarget2D` disposal is correct in Edit mode.
- SFML: Verify `RenderTexture.Display()` is called (common omission).
- FNA: Ensure code compiles against both MonoGame and FNA frameworks.
- KNI: WebGL power-of-two texture constraint must be handled.

---

**End of Analysis**

Author: Gaff | Date: 2026-02-25 | Status: Complete and ready for review by Deckard

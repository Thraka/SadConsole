# SadConsole: CellSurface and ScreenSurface Architecture

## Overview

SadConsole splits its surface concept across two distinct layers:

- **`CellSurface`** — a pure data container. A 2D grid of `ColoredGlyphBase` objects with viewport, dirty-tracking, and all editing/drawing logic. No rendering. No scene graph. Just data.
- **`ScreenSurface`** — a displayable scene-graph node. It owns a `CellSurface` (or any `ICellSurface`), a font, a renderer, and a position within the parent/child hierarchy. It bridges the data model to the rendering pipeline.

This separation lets you manipulate cell data without touching the display system (useful for off-screen buffers, procedural generation, serialization, etc.) and lets you swap surfaces under a displayed object without reconstructing it.

---

## CellSurface

### Data Model

```csharp
public class CellSurface : ICellSurface, ICellSurfaceResize, ICellSurfaceSettable
```

`CellSurface` is the canonical implementation of `ICellSurface`. It holds:

| Member | Purpose |
|--------|---------|
| `ColoredGlyphBase[] Cells` | Flat row-major array of all cells in the buffer |
| `BoundedRectangle _viewArea` | Tracks both the total buffer area and the visible viewport within it |
| `Color DefaultForeground/Background` | Colors used when clearing or creating new cells |
| `int DefaultGlyph` | Glyph index used when clearing cells |
| `bool IsDirty` | Dirty flag; set to `true` by nearly every mutating operation |
| `Effects.EffectsManager Effects` | Per-cell animated effect manager |
| `bool UsePrintProcessor` | When `true`, `Print` calls route through `ColoredString.Parser` |
| `int TimesShiftedUp/Down/Left/Right` | Cumulative shift counters |

### Cell Storage and Indexing

Cells are stored in a single flat `ColoredGlyphBase[]` array using **row-major order**:

```
index = y * Width + x
      = Point.ToIndex(x, y, Width)
```

The array size is always `totalWidth * totalHeight` regardless of the viewport. The constructor enforces `viewWidth <= totalWidth` and `viewHeight <= totalHeight`.

Three indexer overloads are provided for convenience:

```csharp
ColoredGlyphBase this[int x, int y]       // → Cells[Point.ToIndex(x, y, Width)]
ColoredGlyphBase this[int index]           // → Cells[index]
ColoredGlyphBase this[Point position]      // → Cells[position.ToIndex(Width)]
Span<ColoredGlyphBase> this[Range range]   // → Cells.AsSpan(range)
```

Indexer setters are `protected`; mutation through public API goes through the extension methods in `CellSurfaceEditor`.

#### ColoredGlyphBase

Each cell is a `ColoredGlyphBase` (abstract base) or the concrete `ColoredGlyph`. A cell carries:

```csharp
Color Foreground     // glyph color
Color Background     // background fill color
int   Glyph          // index into the font sprite sheet
Mirror Mirror        // None / FlipHorizontally / FlipVertically / Both
bool  IsVisible      // if false, cell is skipped during rendering
List<CellDecorator>? Decorators  // optional overlay glyphs (e.g., underline from extended font)
bool  IsDirty        // per-cell dirty flag (used by some effects)
```

`CellDecorator` adds a second glyph and color rendered on top of the base glyph, enabling rich extended-font features without a second cell.

### Dirty Tracking

`IsDirty` is a **surface-level** flag — it signals that the renderer should re-bake its backing texture. It operates at two granularities:

1. **Surface-level (`CellSurface.IsDirty`)** — the boolean that the renderer polls. Set `true` by virtually every mutating method on the surface. When set, raises `IsDirtyChanged`. Reset to `false` by the renderer after a successful `Refresh`.

2. **Cell-level (`ColoredGlyphBase.IsDirty`)** — set `true` when any property of a cell changes. Raises `IsDirtySet`. This is used by `EffectsManager` to know which cells to process, not by the renderer directly.

The `SetIsDirtySafe()` helper sets the backing field directly without firing the event — used in bulk resize operations to avoid cascading event noise.

```csharp
public bool IsDirty
{
    get => _isDirty;
    set
    {
        if (_isDirty == value) return;
        _isDirty = value;
        OnIsDirtyChanged();  // → fires IsDirtyChanged event
    }
}
```

`ScreenSurface` subscribes to `Surface.IsDirtyChanged` and calls its own virtual `OnIsDirtyChanged()`, forwarding the signal up the display chain.

### Viewport System

`CellSurface` supports a **scrollable viewport** via `BoundedRectangle _viewArea`:

- `Area` — the full buffer bounds (`0, 0, totalWidth, totalHeight`)
- `View` — the visible rectangle (a sub-region of `Area`, constrained by `BoundedRectangle`)
- `ViewPosition` — where within the buffer the viewport is anchored
- `ViewWidth` / `ViewHeight` — visible dimensions
- `IsScrollable` — `true` when `View` dimensions differ from `Area` dimensions

The viewport determines what the renderer draws. The renderer reads `Surface.View` to know which rows and columns to blit. Changing `ViewPosition` is how scroll behavior is implemented.

### Editor Operations

All editing operations are defined as **extension methods** on `ISurface` in the static class `CellSurfaceEditor` (`ICellSurface.Editor.cs`). This is the primary API surface for manipulating cells. Both `CellSurface` and `ScreenSurface` implement `ISurface` (via `IScreenSurfaceEditable`), so all methods work on either.

Key operation categories:

**Cell access / mutation:**
```csharp
void SetGlyph(this ISurface, int x, int y, int glyph, Color fg, Color bg, ...)
void SetForeground(this ISurface, int x, int y, Color color)
void SetBackground(this ISurface, int x, int y, Color color)
void SetMirror(this ISurface, int x, int y, Mirror mirror)
void SetCellAppearance(this ISurface, int x, int y, ColoredGlyphBase appearance)
```

**Print:**
```csharp
void Print(this ISurface, int x, int y, string text)
void Print(this ISurface, int x, int y, string text, Color foreground, Color background, ...)
void Print(this ISurface, int x, int y, ColoredString text)
void Print(this ISurface, int x, int y, ColoredGlyphBase[] glyphs)
```

When `UsePrintProcessor` is `true`, string `Print` calls route through `ColoredString.Parser`, which processes embedded markup commands (color changes, blink, etc.).

**Fill and clear:**
```csharp
ColoredGlyphBase[] Fill(this ISurface, Color? fg, Color? bg, int? glyph, Mirror? mirror)
ColoredGlyphBase[] Fill(this ISurface, Rectangle area, ...)
void Clear(this ISurface)
void Clear(this ISurface, Rectangle area)
void Erase(this ISurface, int x, int y)
```

**Copy:**
```csharp
void Copy(this ISurface, ICellSurface destination)
void Copy(this ISurface, Rectangle area, ICellSurface destination, int destX, int destY)
ICellSurface GetSubSurface(this ISurface, Rectangle view)
```

**Scroll/shift:**
```csharp
void ShiftUp(this ISurface, int amount, bool wrap = false)
void ShiftDown(this ISurface, int amount, bool wrap = false)
void ShiftLeft(this ISurface, int amount, bool wrap = false)
void ShiftRight(this ISurface, int amount, bool wrap = false)
void ShiftRow(this ISurface, int row, int amount, bool wrap)
void ShiftColumn(this ISurface, int col, int amount, bool wrap)
```

**Effects:**
```csharp
void SetEffect(this ISurface, int x, int y, ICellEffect? effect)
void SetEffect(this ISurface, IEnumerable<Point> cells, ICellEffect? effect)
```

### Drawing Operations

Drawing helpers are also extension methods on `ISurface` in `CellSurfaceEditor`:

```csharp
IEnumerable<ColoredGlyphBase> DrawLine(this ISurface, Point start, Point end, int? glyph, Color? fg, Color? bg, ...)
void DrawBox(this ISurface, Rectangle area, ShapeParameters parameters)
void DrawCircle(this ISurface, Rectangle area, ShapeParameters parameters)
void ConnectLines(this ISurface)
void ConnectLines(this ISurface, int[] lineStyle)
void ConnectLines(this ISurface, int[] lineStyle, Rectangle area)
```

`ConnectLines` inspects every cell in the surface (or a region), detects which adjacent cells are also line-drawing characters, and replaces each with the correct junction glyph. Predefined line-style arrays live on `ICellSurface` as static members:

```csharp
ICellSurface.ConnectedLineThin        // ┌─┐│┼│└─┘ etc.
ICellSurface.ConnectedLineThick       // ╔═╗║╬║╚═╝ etc.
ICellSurface.Connected3dBox
ICellSurface.ConnectedLineThinExtended // extended font variants
```

`ShapeParameters` (`ShapeParameters.cs`) captures all glyph, color, fill, and border configuration for shapes.

### Resize

`ICellSurfaceResize` exposes:

```csharp
void Resize(int viewWidth, int viewHeight, int totalWidth, int totalHeight, bool clear)
void Resize(int width, int height, bool clear)
```

The resize algorithm preserves existing cells by copying the overlapping region into a new array. When `clear = true`, the preserved cells are also wiped. After resize, `Effects.DropInvalidCells()` prunes effect bindings referencing cells that no longer exist, and `OnCellsReset()` fires — a virtual hook for subclasses to respond (e.g., the renderer invalidates its backing texture).

### Effects Manager

`EffectsManager` (owned by `CellSurface`) maintains two dictionaries:

- `Dictionary<ICellEffect, ColoredGlyphEffectData>` — effect → affected cells
- `Dictionary<ColoredGlyphBase, ColoredGlyphEffectData>` — cell → active effect

`UpdateEffects(TimeSpan delta)` is called every frame by `ScreenSurface.Update`. Each active effect ticks, potentially modifying cell appearance (color, glyph, visibility) and setting the surface `IsDirty = true` when a change occurs. Effects are transient: they do not persist the cell's original state to disk.

---

## ScreenSurface

### Inheritance Chain

```
object
 └─ ScreenObject        (IScreenObject)
     └─ ScreenSurface   (IScreenSurfaceEditable, ISurfaceSettable, ICellSurfaceResize, IDisposable)
         ├─ Console      (adds Cursor component)
         ├─ LayeredScreenSurface  (adds LayeredSurface component, ILayeredData)
         └─ UI.ControlsConsole    (adds ControlHost component)
             └─ UI.WindowConsole
```

`ScreenObject` is the base scene-graph node — it contributes `Children`, `Parent`, `Position`, `AbsolutePosition`, `IsVisible`, `IsEnabled`, focus, input routing, and the component system.

`ScreenSurface` extends `ScreenObject` with:

- `ICellSurface Surface` — the cell data
- `IRenderer? Renderer` — the host-provided renderer
- `IFont Font` + `Point FontSize` — font and scale
- `Color Tint` — post-render tint overlay
- `bool UsePixelPositioning` — coordinate mode switch
- Mouse events (`MouseEnter`, `MouseExit`, `MouseMove`, `MouseButtonClicked`)

### Surface Ownership

`ScreenSurface` holds a **reference** to an `ICellSurface`, not ownership in the strict sense. The surface can be replaced at runtime via `Surface { set; }` or `SetSurface(...)`. When the surface is swapped, the old surface's `IsDirtyChanged` event is unsubscribed and the new one subscribed, then `OnSurfaceChanged(old)` fires for subclasses to react (e.g., the renderer discards its cached texture).

This design allows multiple `ScreenSurface` instances to share the same `ICellSurface` (with `QuietSurfaceHandling = true` on secondary consumers).

### Rendering Integration

Each frame the scene-graph driver calls `ScreenSurface.Render(TimeSpan delta)`:

```csharp
public override void Render(TimeSpan delta)
{
    // 1. Render components (e.g., ControlHost draws controls onto the surface)
    foreach (IComponent c in ComponentsRender) c.Render(this, delta);

    // 2. Renderer refresh + draw-call enqueue
    if (_renderer != null)
    {
        _renderer.Refresh(this, ForceRendererRefresh);  // re-bake texture if dirty
        _renderer.Render(this);                          // enqueue to GameHost.DrawCalls
        ForceRendererRefresh = false;
    }

    // 3. Recurse into visible children
    foreach (IScreenObject child in Children)
        if (child.IsVisible) child.Render(delta);
}
```

The `IRenderer` contract:

```csharp
public interface IRenderer : IDisposable
{
    ITexture? Output { get; }            // backing texture (result of Refresh)
    List<IRenderStep> Steps { get; }     // ordered pipeline steps
    void Refresh(IScreenSurface surface, bool force);   // cell data → texture
    void Render(IScreenSurface surface);                // texture → draw call queue
    void OnHostUpdated(IScreenSurface surface);         // called when surface/font changes
}
```

`Refresh` is guarded by `IsDirty || force`. It orchestrates `IRenderStep.Refresh` → `IRenderStep.Composing` → `IRenderStep.Render` for each registered step (sorted ascending by `SortOrder`). If _any_ step's `Refresh` returns `true`, the `Composing` pass runs for all steps; otherwise it is skipped (texture cache hit).

The renderer is obtained from `GameHost.Instance.GetRenderer(DefaultRendererName)` during `ScreenSurface` construction. The default name is `Renderers.Constants.RendererNames.Default` (`"ScreenSurface"`). Subclasses override `DefaultRendererName` to request a different renderer (e.g., `LayeredScreenSurface` requests `"LayeredScreenSurface"`).

`ForceRendererRefresh` is a one-shot flag that bypasses the dirty check for a single frame — useful after font changes or surface swaps.

### Scene Graph (IScreenObject)

`ScreenObject` provides the full scene-graph contract. Key semantics:

**Parent / Children:**
```csharp
IScreenObject? Parent { get; set; }   // setting Parent also syncs Children collection
ScreenObjectCollection Children { get; }
```

Setting `Parent` automatically adds/removes the object from the old and new parent's `Children` collection. `Children` is a `ScreenObjectCollection` which enforces the bidirectional invariant.

**Update propagation:**
```csharp
// ScreenObject.Update
void Update(TimeSpan delta)
{
    foreach (IComponent c in ComponentsUpdate) c.Update(this, delta);
    foreach (IScreenObject child in Children)
        if (child.IsEnabled) child.Update(delta);
}

// ScreenSurface.Update adds effects tick:
public override void Update(TimeSpan delta)
{
    Surface.Effects.UpdateEffects(delta);
    base.Update(delta);  // → ScreenObject.Update
}
```

**Render propagation:** `ScreenSurface.Render` runs its own renderer, then recurses into children. Children render on top of their parent's output (draw-call ordering).

**Visibility / enabled:**
- `IsVisible` gates `Render`. An invisible object doesn't render itself or its children.
- `IsEnabled` gates `Update`. A disabled object doesn't update itself or its children.

**Components (`IComponent`):**
Components are sorted into five lists on `ScreenObject`: `ComponentsUpdate`, `ComponentsRender`, `ComponentsKeyboard`, `ComponentsMouse`, `ComponentsEmpty`. Each component declares which lists to participate in via `SortOrder` and flags on the component itself.

### Coordinate System

`ScreenSurface` has two coordinate modes controlled by `UsePixelPositioning`:

```csharp
public override void UpdateAbsolutePosition()
{
    if (UsePixelPositioning)
        AbsolutePosition = Position;                        // Position is already in pixels
    else
        AbsolutePosition = (FontSize * Position);           // Position in cells → pixels

    if (!IgnoreParentPosition)
        AbsolutePosition += Parent?.AbsolutePosition ?? Point.Zero;

    foreach (IScreenObject child in Children)
        child.UpdateAbsolutePosition();
}
```

- **Default mode (`UsePixelPositioning = false`):** `Position` is in **cell units**. `AbsolutePosition` = `Position × FontSize + Parent.AbsolutePosition`. A surface at `(5, 3)` with 8×16 font cells is at pixel `(40, 48)` plus parent offset.
- **Pixel mode (`UsePixelPositioning = true`):** `Position` is in **pixels** directly. Used for precise sub-cell placement or when mixing with non-cell-aligned objects.

`AbsoluteArea` gives the pixel-space bounding rectangle:
```csharp
Rectangle AbsoluteArea => new(AbsolutePosition.X, AbsolutePosition.Y, WidthPixels, HeightPixels);
// WidthPixels  = Surface.View.Width  * FontSize.X
// HeightPixels = Surface.View.Height * FontSize.Y
```

Note: `WidthPixels`/`HeightPixels` are based on the **viewport** size, not the full buffer. A scrollable surface with a 80×25 view but 80×500 buffer presents as 80×25 cells on screen.

`ScreenObject` (base class) also has a `PositionFontSize` field that scales `AbsolutePosition` for non-`ScreenSurface` objects that need similar cell-relative positioning.

### Font System

`IFont` is a host-provided interface (implemented by `SadFont`). It represents a **sprite-sheet atlas** mapping glyph indices to pixel rectangles.

```csharp
public interface IFont : IDisposable
{
    string Name { get; }
    int GlyphWidth { get; }         // width of each glyph tile in pixels
    int GlyphHeight { get; }        // height of each glyph tile in pixels
    int GlyphPadding { get; set; }  // spacing between tiles in the sheet
    int TotalGlyphs { get; }        // Columns × Rows
    int SolidGlyphIndex { get; set; }    // the all-white glyph, used for solid fills
    ITexture Image { get; set; }         // the atlas texture (host-specific)
    Rectangle GetGlyphSourceRectangle(int glyph);
    Dictionary<int, Rectangle> GlyphRectangles { get; }
    Dictionary<string, GlyphDefinition> GlyphDefinitions { get; }
    Point GetFontSize(IFont.Sizes size);  // Quarter/Half/One/Two/Three/Four
}
```

`SadFont` (the concrete class) adds `Columns`, `Rows`, and `FilePath`. Fonts are loaded and registered in `GameHost.Fonts` at startup; `GameHost.DefaultFont` and `GameHost.DefaultFontSize` provide the fallback.

`ScreenSurface.Font` and `ScreenSurface.FontSize` are independent: `Font` is the atlas, `FontSize` is a `Point` that may differ from `(GlyphWidth, GlyphHeight)` (e.g., `IFont.Sizes.Two` gives `(GlyphWidth*2, GlyphHeight*2)`). This lets surfaces share a font but render at different scales.

When `Font` or `FontSize` changes, `OnFontChanged(oldFont, oldSize)` fires, `IsDirty` is set, and `UpdateAbsolutePosition()` is not called automatically — the caller is responsible for re-layout if needed. The renderer's `OnHostUpdated` is invoked to allow texture reallocation.

**Extended fonts (`IFont.IsSadExtended = true`):** The `SadConsole` extended font format adds named `GlyphDefinition` entries and supports `CellDecorator` overlay glyphs, enabling rich box-drawing, icons, and multi-layer effects without additional surfaces.

---

## The Data/Display Split

### Why They're Separate

`CellSurface` contains no rendering code and no references to any host-specific type. It can be created, manipulated, serialized, and passed around without a game loop running. This is the critical design choice that enables:

- **Off-screen buffers** — generate content in `CellSurface`, display it later in a `ScreenSurface`
- **Asset loading** — REXPaint, TheDraw, Playscii importers return `CellSurface` instances
- **Shared data** — two `ScreenSurface` instances can reference the same `CellSurface`; one displays it, one edits it
- **Serialization** — `[DataContract]` / `[JsonObject]` on `CellSurface` is clean because there are no renderer or host references to skip
- **Testing** — surface logic is fully testable without a running host

### When to Use CellSurface Alone

Use a standalone `CellSurface` when:
- Generating tilemap data procedurally before display
- Implementing an off-screen scratch pad for compositing
- Reading/writing `.xp` or other file formats
- Writing tests for string parsing, fill, or copy logic

### When to Use ScreenSurface

Use `ScreenSurface` (or a subclass) when:
- The surface needs to appear on screen
- You need position, parent/child relationships, or components
- You need mouse/keyboard input
- You need animations (effects + `Update` tick)

### How Rendering Bridges the Gap

The flow each frame:

```
CellSurface.Cells[]         (data: glyphs, colors)
        ↓  IsDirty = true
ScreenSurface.Render(delta)
        ↓
IRenderer.Refresh(surface)  (host: cells → backing texture ITexture)
        ↓
IRenderer.Render(surface)   (host: enqueue draw call to GameHost.DrawCalls)
        ↓
GameHost.SharedSpriteBatch  (host: blit all draw calls to Global.RenderOutput)
        ↓
OS Window                   (host: final blit)
```

The renderer reads `surface.Surface.Cells`, `surface.Surface.View`, `surface.Font`, and `surface.FontSize` during `Refresh`. For each visible cell, it maps `cell.Glyph` through `Font.GetGlyphSourceRectangle(glyph)` and draws the source rectangle from the font atlas to the backing texture at the correct pixel position, colored by `cell.Foreground` and `cell.Background`.

---

## Key Patterns and Extension Points

### Extension Methods as the Editing API

All cell mutation methods are **static extension methods** on `ISurface` (not instance methods on `CellSurface`). This means:
1. The editing API is available on any type that implements `ISurface` (including `ScreenSurface` via `IScreenSurfaceEditable`).
2. Adding new editor operations doesn't require modifying core types.
3. The core `CellSurface` class stays focused on data management; operations are a separate concern.

### The ISurface Indirection

`ISurface` is a single-property interface:
```csharp
public interface ISurface
{
    ICellSurface Surface { get; }
}
```

This pattern threads extension methods through composite types. `ScreenSurface` implements `ISurface` by returning `this.Surface` (the inner `ICellSurface`). `CellSurface` implements it by returning `this`. So the same `Print(...)` extension method works on both without casting.

### Partial Class Structure

`ICellSurface` is declared `partial` and split across:

| File | Contents |
|------|----------|
| `ICellSurface.cs` | Core interface definition (properties, events, viewport) |
| `ICellSurface.Editor.cs` | `CellSurfaceEditor` static class — all extension methods |
| `ICellSurface.Static.cs` | Static line-style arrays (`ConnectedLineThin`, etc.) and helpers |

`ScreenSurface` is `partial` and split across:

| File | Contents |
|------|----------|
| `ScreenSurface.cs` | Core class: Surface, Font, Renderer, Update, Render, Resize |
| `ScreenSurface.Input.cs` | Mouse event properties and handler overrides |

### Observable Dirty Chain

The dirty signal propagates as follows:

```
ColoredGlyphBase.IsDirty = true
    → IsDirtySet event  (used by EffectsManager internally)

CellSurface.IsDirty = true
    → IsDirtyChanged event
        → ScreenSurface._isDirtyChangedEventHandler
            → OnIsDirtyChanged() (virtual, overridable by subclasses)
```

Setting any property on a `ColoredGlyphBase` sets `cell.IsDirty = true`. Most editor extension methods then explicitly set `surface.IsDirty = true` afterwards, so the renderer is always notified via the event chain even if no individual cell fires first.

### IRenderer + IRenderStep — Extensibility at the Rendering Layer

`IRenderer` exposes `List<IRenderStep> Steps`. Each step implements three phases:

```csharp
interface IRenderStep
{
    uint SortOrder { get; set; }
    bool Refresh(IRenderer renderer, IScreenSurface screenObject);  // cells → step texture
    void Composing(IRenderer renderer, IScreenSurface screenObject); // step → renderer output
    void Render(IRenderer renderer, IScreenSurface screenObject);    // renderer output → draw queue
}
```

Steps are registered by name in `GameHost._rendererSteps` and added to a renderer's `Steps` list. Third-party code can inject custom steps (e.g., a lighting overlay) without modifying the renderer or core types.

### Subclassing Pattern

The standard pattern for extending `ScreenSurface`:

```csharp
public class MyConsole : ScreenSurface
{
    public MyConsole(int width, int height) : base(width, height) { }

    // Override rendering virtual hooks
    protected override void OnFontChanged(IFont oldFont, Point oldFontSize) { ... }
    protected override void OnSurfaceChanged(ICellSurface oldSurface) { ... }
    protected override void OnRendererChanged() { ... }
    protected override void OnIsDirtyChanged() { ... }

    // Override update/render
    public override void Update(TimeSpan delta) { ...; base.Update(delta); }
    public override void Render(TimeSpan delta) { base.Render(delta); ... }
}
```

`Console` adds a `Cursor` component this way. `LayeredScreenSurface` adds a `LayeredSurface` component. `UI.ControlsConsole` attaches `ControlHost`. All of them follow the same pattern: add a component in the constructor, override virtual hooks as needed.

### Key File Reference

| File | Role |
|------|------|
| `SadConsole/CellSurface.cs` | `CellSurface` class — data, viewport, resize, constructors |
| `SadConsole/ICellSurface.cs` | Core `ICellSurface` partial interface |
| `SadConsole/ICellSurface.Editor.cs` | `CellSurfaceEditor` — all editing/drawing extension methods |
| `SadConsole/ICellSurface.Static.cs` | Static line-style arrays |
| `SadConsole/ICellSurfaceResize.cs` | `Resize` contract |
| `SadConsole/ICellSurfaceSettable.cs` | `SetSurface` contract |
| `SadConsole/ISurface.cs` | Single-property shim enabling extension methods on composites |
| `SadConsole/ColoredGlyphBase.cs` | Abstract cell type |
| `SadConsole/ColoredGlyph.cs` | Concrete cell type |
| `SadConsole/ScreenSurface.cs` | `ScreenSurface` — surface + renderer + scene-graph node |
| `SadConsole/ScreenSurface.Input.cs` | `ScreenSurface` mouse input partial |
| `SadConsole/ScreenObject.cs` | `ScreenObject` — scene-graph base |
| `SadConsole/IScreenObject.cs` | Scene-graph contract |
| `SadConsole/IScreenSurface.cs` | `IScreenSurface` — displayable surface contract |
| `SadConsole/IScreenSurfaceEditable.cs` | `IScreenSurfaceEditable = IScreenSurface + ISurface` |
| `SadConsole/IFont.cs` | Font contract |
| `SadConsole/SadFont.cs` | Concrete `SadFont` implementation |
| `SadConsole/Effects/EffectsManager.cs` | Per-cell animated effect scheduling |
| `SadConsole/Renderers/IRenderer.cs` | Renderer contract |
| `SadConsole/Renderers/IRenderStep.cs` | Render step contract |
| `SadConsole/Renderers/Constants.cs` | Renderer/step names and sort orders |

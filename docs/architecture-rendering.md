# SadConsole Rendering Architecture

> **Audience:** Contributors who understand graphics programming basics (render targets, sprite batching, draw calls) but are new to SadConsole's rendering internals.

---

## Overview

SadConsole's rendering system converts a tree of `IScreenSurface` objects — each holding a flat grid of colored glyphs — into pixels on screen. It does this through a **three-tier compositing pipeline**:

1. Each active `IRenderStep` draws content into its own private texture.
2. An `IRenderer` composites all of its steps' textures into a single **output texture** (`IRenderer.Output`).
3. The `GameHost` flushes all renderers' output textures into a single global `RenderOutput` texture, which is then blitted to the OS window.

The pipeline is host-independent: the contracts are defined in `SadConsole/Renderers/`, and MonoGame and SFML both implement them using their respective graphics APIs.

---

## Key Concepts

### `IRenderer` (`SadConsole/Renderers/IRenderer.cs`)

An `IRenderer` is responsible for one `IScreenSurface`. It:

- Owns an ordered `List<IRenderStep> Steps`.
- Owns `ITexture Output` — the composited result texture for this surface.
- Exposes `Refresh(IScreenSurface, bool force)` — updates the output texture from the current cell state.
- Exposes `Render(IScreenSurface)` — enqueues draw calls into the global draw queue.
- Tracks `IsForced` to skip dirty-checking and always redraw.

The concrete implementation (`ScreenSurfaceRenderer`) also holds `_backingTexture` — a host-specific render target (`RenderTarget2D` in MonoGame, `RenderTexture` in SFML) that is the writeable form of `Output`.

### `IRenderStep` (`SadConsole/Renderers/IRenderStep.cs`)

An `IRenderStep` is a composable unit of rendering work. Each step has:

| Member | Purpose |
|---|---|
| `Name` | String identifier (see `Constants.RenderStepNames`) |
| `SortOrder` | `uint`; lower runs first |
| `Refresh(...)` | Redraws the step's private texture if needed. Returns `true` if it wrote something new and wants to be composited. |
| `Composing(...)` | Blits the step's private texture onto the renderer's output texture. |
| `Render(...)` | Enqueues draw calls into `GameHost.DrawCalls`. |
| `SetData(object)` | Injects supplemental data (e.g., a `ControlHost` reference). |
| `Reset()` | Disposes and nulls internal textures. |

Steps can optionally implement `IRenderStepTexture` to expose their private `CachedTexture` to outside callers (used by steps like `SurfaceRenderStep` and `ControlHostRenderStep`).

### The Output Texture (`IRenderer.Output`)

This is the renderer's final product — a fully composited texture of everything the renderer knows how to draw for one surface. It is:

- Created and sized to match the surface's `AbsoluteArea` (pixel dimensions).
- Rebuilt automatically if the surface is resized.
- Written during the **Composing** phase of `Refresh`.
- Read during the **Render** phase — `OutputSurfaceRenderStep` enqueues it as a `DrawCallTexture` into `GameHost.DrawCalls`.

### `GameHost` (`SadConsole/GameHost.cs`)

The abstract singleton that owns the game loop. Rendering-specific responsibilities:

- `_renderers` / `_rendererSteps` — string-keyed dictionaries of `Type`. Hosts register concrete classes here at startup.
- `GetRenderer(name)` / `GetRendererStep(name)` — factory methods that instantiate registered types by name.
- `DrawCalls` — a `Queue<IDrawCall>` that accumulates all enqueued draw calls during a frame.

### `Global.RenderOutput`

A single, screen-sized render target created by the host. All surfaces' output textures are composited here in one pass. This is the final texture the host blits to the OS window.

---

## The Render Loop (Per Frame)

```
GameHost.DrawCalls.Clear()
    │
    └─▶ Screen.Render(delta)                   // Walk the scene graph
            │
            └─▶ [For each IScreenSurface node]
                    ├── Components[].Render()   // Component render hooks
                    ├── renderer.Refresh(surface, force)   ◄── Build output texture
                    │       │
                    │       ├── Resize _backingTexture / CachedRenderRects if needed
                    │       ├── step.Refresh(...)   ◄── each step redraws its private texture
                    │       └── IF any step returned true OR IsForced:
                    │               Clear _backingTexture
                    │               Open sprite batch → _backingTexture
                    │               step.Composing(...)   ◄── each step blits onto _backingTexture
                    │               End sprite batch  (_backingTexture is now complete)
                    │
                    ├── renderer.Render(surface)   ◄── Enqueue draw calls
                    │       └── step.Render(...)   ◄── OutputSurfaceRenderStep enqueues DrawCallTexture
                    │                               ◄── TintSurfaceRenderStep enqueues DrawCallColor
                    │
                    └── children[].Render(delta)   // Recurse

GameHost.InvokeFrameDraw()                     // FrameRender event fires here

Set render target → Global.RenderOutput
Clear RenderOutput
Open shared sprite batch
    └─▶ For each DrawCall in GameHost.DrawCalls:
            call.Draw()   ◄── blits each surface's _backingTexture into RenderOutput
End sprite batch

IF Settings.DoFinalDraw:
    Set render target → window back buffer
    Blit RenderOutput → window (with RenderRect scaling)
```

---

## RenderStep Lifecycle

### Registration

At startup, the host's `Game.Initialize()` (or `SadConsoleGameComponent.Initialize()` for MonoGame) calls:

```csharp
GameHost.SetRenderer("screensurface", typeof(Renderers.ScreenSurfaceRenderer));
GameHost.SetRendererStep("surface", typeof(Renderers.SurfaceRenderStep));
// ... etc.
```

This populates the string → `Type` dictionaries in `GameHost`. Steps are instantiated lazily via `Activator.CreateInstance`.

### Construction and Ordering

`ScreenSurfaceRenderer` calls `AddDefaultSteps()` in its constructor, then sorts:

```csharp
Steps.Add(new SurfaceRenderStep());       // SortOrder = 50
Steps.Add(new OutputSurfaceRenderStep()); // SortOrder = 50
Steps.Add(new TintSurfaceRenderStep());   // SortOrder = 90
Steps.Sort(RenderStepComparer.Instance);  // ascending by SortOrder
```

Sort values for all built-in steps (`Constants.RenderStepSortValues`):

| Step | Sort Value | Phase activity |
|---|---|---|
| `Window` (modal overlay) | 10 | Composing |
| `Surface` (cell grid) | 50 | Refresh + Composing |
| `Output` (enqueue output texture) | 50 | Render |
| `EntityManager` | 60 | Refresh + Composing |
| `Cursor` | 70 | Refresh + Composing |
| `ControlHost` | 80 | Refresh + Composing |
| `Tint` | 90 | Render |

Steps with equal sort values run in insertion order.

### Phase Separation

The three phases are called sequentially and serve distinct roles:

- **`Refresh`** — Update private state and textures. May be skipped if not dirty. Returns `true` to signal that compositing is needed.
- **`Composing`** — Write into `_backingTexture` via an open sprite batch. Only called if at least one `Refresh` returned `true` or `IsForced` is set.
- **`Render`** — Enqueue draw calls into `GameHost.DrawCalls`. Always called. Does _not_ write to `_backingTexture`.

This separation means expensive pixel work (Refresh/Composing) is only done when content changes, while the draw call submission (Render) always runs so the renderer stays in the queue.

---

## The Output Texture in Detail

There are actually **three texture levels** in a typical frame:

```
Level 1 (per step):
  SurfaceRenderStep.BackingTexture      ← glyphs drawn here
  ControlHostRenderStep.BackingTexture  ← controls drawn here
  CursorRenderStep.BackingTexture       ← cursor drawn here

    │  (step.Composing blits each of these down)
    ▼

Level 2 (per renderer):
  ScreenSurfaceRenderer._backingTexture ← all steps composited here
  == IRenderer.Output

    │  (OutputSurfaceRenderStep enqueues DrawCallTexture)
    ▼

Level 3 (global):
  Global.RenderOutput                   ← all surfaces composited here

    │  (final blit to OS window)
    ▼

  Window back buffer
```

This means cell rendering is cached at the step level: `SurfaceRenderStep` only redraws its texture when the surface is `IsDirty`. The renderer-level compositing (`_backingTexture`) only runs when a step signals a change. The global `RenderOutput` is rebuilt every frame from `DrawCalls`.

---

## Host Implementations

Both hosts implement the same `IRenderer` / `IRenderStep` interfaces with host-specific graphics primitives. The logical structure is identical.

### Shared Structure

| Concept | MonoGame | SFML |
|---|---|---|
| Render target type | `RenderTarget2D` | `RenderTexture` |
| Output texture wrapper | `Host.GameTexture(_backingTexture)` | `Host.GameTexture(_backingTexture.Texture)` |
| Shared sprite batch | `Host.Global.SharedSpriteBatch` (XNA `SpriteBatch`) | `Host.Global.SharedSpriteBatch` (custom `SpriteBatch`) |
| Set render target | `GraphicsDevice.SetRenderTarget(target)` | Pass target to `SharedSpriteBatch.Reset(target, ...)` |
| Flush render target | `GraphicsDevice.SetRenderTarget(null)` | `target.Display()` |

### MonoGame-Specific: `IRendererMonoGame`

MonoGame renderers implement an additional interface (`SadConsole.Host.MonoGame/Renderers/IRendererMonoGame.cs`):

```csharp
public interface IRendererMonoGame
{
    BlendState MonoGameBlendState { get; set; }
    XnaRectangle[] CachedRenderRects { get; }
    SpriteBatch LocalSpriteBatch { get; }
}
```

`LocalSpriteBatch` is a per-renderer `SpriteBatch` used by `SurfaceRenderStep` during its `Refresh` phase. This is separate from `Host.Global.SharedSpriteBatch`, which is used during the `Composing` phase.

SFML doesn't need this distinction because SFML's `RenderTexture` is the render target itself — you can open a sprite batch directly against it without managing a separate "current render target" on a device.

### MonoGame Game Component

MonoGame wraps the draw phase in `SadConsoleGameComponent : DrawableGameComponent`. Its `Draw(GameTime)` method implements the same logic as SFML's `while` loop body: clear `DrawCalls`, walk the scene graph, flush to `RenderOutput`, blit to screen.

### SFML Direct Loop

SFML uses an explicit `while (window.IsOpen)` loop in `SadConsole.Game.Run()`. It directly calls `Screen.Render(delta)`, flushes `DrawCalls` to `Global.RenderOutput`, and then presents `RenderOutput` to the window.

---

## Built-In Renderer Variants

| Name (constant) | Class | Description |
|---|---|---|
| `"screensurface"` / `"default"` | `ScreenSurfaceRenderer` | Standard step-based renderer. Used by `ScreenSurface`, `Console`, etc. |
| `"optimizedscreensurface"` | `OptimizedScreenSurfaceRenderer` | No steps. Draws cells directly to `_backingTexture` in `Refresh`. Faster but not extensible. |
| `"window"` | `WindowRenderer` | Extends `ScreenSurfaceRenderer`. Adds the `Window` step for modal overlay tinting. |
| `"layeredscreensurface"` | `LayeredRenderer` | Extends `ScreenSurfaceRenderer`. Adds the `SurfaceLayered` step for multi-layer surfaces. |

---

## Extension Points

### Adding a Custom RenderStep

1. Implement `IRenderStep`:

```csharp
public class MyGlowStep : IRenderStep
{
    public string Name => "my_glow";
    public uint SortOrder { get; set; } = 65; // run after EntityManager, before ControlHost

    public bool Refresh(IRenderer renderer, IScreenSurface screen,
                        bool backingTextureChanged, bool isForced)
    {
        // Return true if this step drew something new.
        return false;
    }

    public void Composing(IRenderer renderer, IScreenSurface screen)
    {
        // Blit onto renderer's _backingTexture via the open sprite batch.
    }

    public void Render(IRenderer renderer, IScreenSurface screen)
    {
        // Optionally enqueue additional draw calls.
    }

    public void SetData(object data) { }
    public void Reset() { }
    public void Dispose() { }
}
```

2. Register with the host (optional, only if you want name-based lookup):

```csharp
GameHost.Instance.SetRendererStep("my_glow", typeof(MyGlowStep));
```

3. Add to a renderer's `Steps` list:

```csharp
// Directly:
mySurface.Renderer.Steps.Add(new MyGlowStep());
mySurface.Renderer.Steps.Sort(RenderStepComparer.Instance);

// Or by name (if registered):
mySurface.Renderer.Steps.Add(GameHost.Instance.GetRendererStep("my_glow"));
mySurface.Renderer.Steps.Sort(RenderStepComparer.Instance);
```

### Adding a Custom Renderer

1. Subclass `ScreenSurfaceRenderer` and override `AddDefaultSteps()`:

```csharp
public class MyRenderer : ScreenSurfaceRenderer
{
    protected override void AddDefaultSteps()
    {
        base.AddDefaultSteps();
        Steps.Add(new MyGlowStep());
    }
}
```

2. Register and assign:

```csharp
GameHost.Instance.SetRenderer("my_renderer", typeof(MyRenderer));
mySurface.Renderer = GameHost.Instance.GetRenderer("my_renderer");
```

Or to make it the default for a surface type, override `DefaultRendererName`:

```csharp
public class MySurface : ScreenSurface
{
    public override string DefaultRendererName => "my_renderer";
}
```

---

## Diagram: Full Render Pipeline

```
╔═══════════════════════════════════════════════════════════════╗
║  SCENE GRAPH  (Screen.Render called by game loop)             ║
║                                                               ║
║  ScreenSurface A                                              ║
║  ├─ IRenderer (ScreenSurfaceRenderer)                         ║
║  │   ├─ Step: SurfaceRenderStep     [sort 50]                 ║
║  │   │   └─ BackingTexture ←── glyphs drawn here (Refresh)   ║
║  │   │         │                                              ║
║  │   │         └──── blitted to _backingTexture (Composing)   ║
║  │   │                                                        ║
║  │   ├─ Step: ControlHostRenderStep [sort 80]                 ║
║  │   │   └─ BackingTexture ←── controls drawn here (Refresh)  ║
║  │   │         │                                              ║
║  │   │         └──── blitted to _backingTexture (Composing)   ║
║  │   │                                                        ║
║  │   ├─ Step: OutputSurfaceRenderStep [sort 50]               ║
║  │   │         └──── enqueues DrawCallTexture (Render)        ║
║  │   │                   └─ points to _backingTexture         ║
║  │   │                                                        ║
║  │   ├─ Step: TintSurfaceRenderStep [sort 90]                 ║
║  │   │         └──── enqueues DrawCallColor if tint≠0 (Render)║
║  │   │                                                        ║
║  │   └─ _backingTexture == IRenderer.Output                   ║
║  │                                                            ║
║  └─ Children[]  (each recurses the same pattern)              ║
╚═══════════════════════════════════════════════════════════════╝
                    │
                    │  GameHost.DrawCalls queue
                    ▼
╔═══════════════════════════════════════════════════════════════╗
║  GLOBAL COMPOSITE                                             ║
║                                                               ║
║  Global.RenderOutput  (screen-sized render target)            ║
║  ├─ DrawCallTexture → Surface A's _backingTexture             ║
║  ├─ DrawCallColor  → Surface A's tint                         ║
║  ├─ DrawCallTexture → Surface B's _backingTexture             ║
║  └─ ...                                                       ║
╚═══════════════════════════════════════════════════════════════╝
                    │
                    │  DoFinalDraw blit
                    ▼
╔═══════════════════════════════════════════════════════════════╗
║  OS WINDOW  (scaled/fitted to Settings.Rendering.RenderRect)  ║
╚═══════════════════════════════════════════════════════════════╝
```

---

## Key Source Files

| File | Role |
|---|---|
| `SadConsole/Renderers/IRenderer.cs` | Renderer contract |
| `SadConsole/Renderers/IRenderStep.cs` | RenderStep contract |
| `SadConsole/Renderers/IRenderStepTexture.cs` | Optional texture exposure for steps |
| `SadConsole/Renderers/Constants.cs` | All names and sort values |
| `SadConsole/Renderers/RenderStepComparer.cs` | `IComparer` for step ordering |
| `SadConsole/GameHost.cs` | Renderer/step registry; `DrawCalls` queue |
| `SadConsole/ScreenSurface.cs` | `Render()` override calling `renderer.Refresh` + `renderer.Render` |
| `SadConsole.Host.SFML/Game.cs` | SFML game loop and final blit |
| `SadConsole.Host.SFML/Renderers/ScreenSurfaceRenderer.cs` | SFML concrete renderer |
| `SadConsole.Host.SFML/Renderers/Steps/` | All SFML step implementations |
| `SadConsole.Host.MonoGame/MonoGame/SadConsoleGameComponent.Mono.cs` | MonoGame draw phase |
| `SadConsole.Host.MonoGame/Renderers/ScreenSurfaceRenderer.cs` | MonoGame concrete renderer |
| `SadConsole.Host.MonoGame/Renderers/IRendererMonoGame.cs` | MonoGame-specific renderer interface |
| `SadConsole.Host.MonoGame/Renderers/Steps/` | All MonoGame step implementations |

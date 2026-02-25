# SadConsole Architecture

> A practical guide for new contributors.

---

## 1. High-Level Overview

SadConsole is a C#/.NET cross-platform **tile-based / ASCII game engine**. It simulates old-school terminal and console programs while running on modern hardware via whichever rendering backend you choose.

The central design principle is **separation of concerns between the object model and rendering**:

- The **core library** (`SadConsole`) defines the object model — surfaces, cells, fonts, input handling, the scene graph, entities, the string parser, effects, and the GUI controls system. It contains **zero rendering code**.
- **Host libraries** (`SadConsole.Host.*`) each wrap a specific rendering framework (MonoGame, SFML, FNA, KNI) and implement the contracts defined by core.

A consumer app references only one host library; that host pulls in core as a transitive dependency.

---

## 2. Project Structure

```
SadConsole/                     # Core library — object model, no rendering
SadConsole.Host.MonoGame/       # MonoGame rendering host
SadConsole.Host.SFML/           # SFML rendering host
SadConsole.Host.FNA/            # FNA rendering host
SadConsole.Host.KNI/            # KNI rendering host
SadConsole.Host.MonoGameWPF/    # MonoGame + WPF window host
SadConsole.Host.Shared/         # Shared code used across hosts
SadConsole.Extended/            # Optional add-on: extra components, transitions, UI helpers
SadConsole.Analyzers/           # Roslyn analyzers for the library
Tests/                          # Unit and integration tests
PerformanceTests/               # Performance benchmarks
templates/                      # dotnet new project templates
Fonts/                          # Bundled bitmap font assets
```

### Core library sub-folders (`SadConsole/`)

| Folder | Purpose |
|---|---|
| `Components/` | `IComponent` / `IComponentHost` system — attach behaviour to any screen object |
| `Configuration/` | Fluent builder API (`Builder`, `IConfigurator`) for game startup |
| `DrawCalls/` | `IDrawCall` — deferred draw-call queue filled each frame by renderers |
| `Effects/` | Cell visual effects (blink, fade, etc.) |
| `Entities/` | `Entity`, `EntityManager` — thousands of movable objects on a surface |
| `Input/` | Keyboard and mouse state abstractions |
| `Instructions/` | Chainable async instruction system for animations / sequences |
| `Renderers/` | Core-side renderer contracts: `IRenderer`, `IRenderStep`, `ITexture` |
| `Serialization/` | JSON serialization helpers (Newtonsoft.Json) |
| `StringParser/` | Inline color/effect markup parser for `ColoredString` |
| `UI/` | Controls subsystem: `ControlHost`, `ControlsConsole`, `WindowConsole`, `Colors` |
| `UI/Controls/` | Individual control widgets (Button, TextBox, ListBox, etc.) |
| `Ansi/` | ANSI art importer |
| `Readers/` | Importers for RexPaint, TheDraw, Playscii |
| `SplashScreens/` | Built-in startup splash screen management |

---

## 3. The Core / Host Separation

```
┌────────────────────────────────────────┐
│            Your Game / App             │
└──────────────────┬─────────────────────┘
                   │ references
┌──────────────────▼─────────────────────┐
│      SadConsole.Host.MonoGame          │  (or SFML / FNA / KNI)
│  - Game : GameHost                     │
│  - ScreenSurfaceRenderer : IRenderer   │
│  - GameTexture : ITexture              │
│  - SadFont : IFont                     │
└──────────────────┬─────────────────────┘
                   │ implements contracts defined in
┌──────────────────▼─────────────────────┐
│           SadConsole (core)            │
│  - GameHost (abstract)                 │
│  - IRenderer / IRenderStep             │
│  - ITexture / IFont                    │
│  - IScreenObject / IScreenSurface      │
│  - ICellSurface / ColoredGlyph         │
└────────────────────────────────────────┘
```

The host is responsible for:
- Subclassing `GameHost` and assigning `GameHost.Instance`.
- Registering concrete `IRenderer` and `IRenderStep` types via `GameHost._renderers` / `_rendererSteps`.
- Providing concrete implementations of `ITexture` and `IFont`.
- Loading default fonts.
- Driving the update/render loop and forwarding input.

Core never calls any rendering API directly. It talks only to `IRenderer`, `ITexture`, and `IFont`.

---

## 4. Key Abstractions

### `GameHost` (`SadConsole/GameHost.cs`)
Abstract singleton (`GameHost.Instance`). Hosts subclass this. Owns the font registry, renderer/step type registry, the root screen object, and fires `FrameUpdate` / `FrameRender` events.

### `IScreenObject` (`SadConsole/IScreenObject.cs`)
The base contract for anything in the scene graph. Provides parent/child relationships, component attachment, position, visibility, enable state, and `Update` / `Render` / `ProcessKeyboard` / `ProcessMouse` lifecycle callbacks.

### `IScreenSurface` (`SadConsole/IScreenSurface.cs`)
Extends `IScreenObject`. Adds a cell surface (`ICellSurface`), a font (`IFont`), a renderer (`IRenderer`), pixel dimensions, tint, mouse events, and dirty state tracking.

### `ICellSurface` (`SadConsole/ICellSurface.cs`)
The raw data model — a flat array of `ColoredGlyph` cells with a width/height and an optional viewport. Provides the editor methods for printing, clearing, filling, and copying regions.

### `ColoredGlyph` (`SadConsole/ColoredGlyph.cs`)
A single cell: glyph index, foreground color, background color, mirror flags, decorators, and an effect reference.

### `IRenderer` / `IRenderStep` (`SadConsole/Renderers/`)
- `IRenderer` — given an `IScreenSurface`, produces an `ITexture` (the cached backing texture) via `Refresh()`, then enqueues draw calls via `Render()`.
- `IRenderStep` — a composable sub-step of a renderer (surface cells, tint, entity layer, controls layer, etc.). Each step has `Refresh` → `Composing` → `Render` phases. Steps are sorted by `SortOrder`.

### `ITexture` (`SadConsole/ITexture.cs`)
Host-provided pixel buffer. Core can read/write raw pixel colors and convert to `ICellSurface`.

### `IFont` (`SadConsole/IFont.cs`)
Host-provided sprite-sheet font. Exposes glyph dimensions, padding, solid/unsupported glyph indices, and per-glyph source rectangles.

### `IComponent` / `IComponentHost` (`SadConsole/Components/`)
Behaviour objects attached to any `IScreenObject`. Components declare which lifecycle events they participate in (`IsUpdate`, `IsRender`, `IsMouse`, `IsKeyboard`).

---

## 5. Data Flow — Rendering a Frame

```
Host game loop tick
  │
  ├─ Update phase
  │    GameHost.FrameUpdate
  │    → IScreenObject.Update(delta)          [recursive: children, components]
  │         └─ IComponent.Update(...)
  │
  └─ Render phase
       GameHost.FrameRender
       → IScreenObject.Render(delta)          [recursive: children]
            └─ IScreenSurface.Renderer.Refresh(surface)
                 │  (only if surface.IsDirty or IsForced)
                 │  foreach IRenderStep:
                 │    step.Refresh(...)        ← re-rasterises cells into backing texture
                 │    step.Composing(...)      ← composites layers onto IRenderer.Output
                 └─ IScreenSurface.Renderer.Render(surface)
                      └─ foreach IRenderStep:
                           step.Render(...)   ← enqueues IDrawCall into host draw-call list

       Host flushes draw-call list
         → SpriteBatch.Draw(backingTexture, screenPosition)
              └─ GPU draws pixels
```

Key points:
- Surfaces cache their rendered state in a `RenderTarget2D` (MonoGame) / equivalent. Only dirty or forced surfaces are re-rasterised.
- Draw calls are collected into a list and flushed by the host in one batch, minimising state switches.
- The host's `ScreenSurfaceRenderer` implements both `IRenderer` and the MonoGame-specific `IRendererMonoGame`, which grants access to `SpriteBatch` and `RenderTarget2D`.

---

## 6. Controls / UI Subsystem

The controls system lives in `SadConsole/UI/` and `SadConsole/UI/Controls/`.

```
ControlsConsole (ScreenSurface subclass)
  └─ ControlHost  (IComponent, attached automatically)
       └─ List<ControlBase>
            ├─ Button
            ├─ TextBox
            ├─ ListBox<T>
            ├─ ComboBox<T>
            ├─ CheckBox / RadioButton
            ├─ ScrollBar
            ├─ ProgressBar
            ├─ TabControl
            ├─ DrawingArea
            └─ ... (more)
```

- **`ControlHost`** (`UI/ControlHost.cs`) is an `IComponent` that you add to any `IScreenSurface`. It manages focus, keyboard routing, mouse routing, and rendering of controls. `ControlsConsole` is a convenience class that pre-attaches `ControlHost`.
- **`ControlBase`** (`UI/Controls/ControlBase.cs`) is the base class for all widgets. Each control owns its own render logic (drawing itself onto the parent surface's cell array) and participates in the same update/keyboard/mouse lifecycle as screen objects.
- **Theming** — each control has a companion `*.Theme.cs` partial that holds `Colors` and `ThemeStates` for normal/focused/disabled/hover/selected visual states. The `Colors` class (`UI/Colors.cs`) provides a full palette that can be replaced per-console or globally.
- **`WindowConsole`** (`UI/WindowConsole.cs`) extends `ControlsConsole` with draggable modal window behaviour.

---

## 7. Extension Points

| Mechanism | Where | Use case |
|---|---|---|
| `IComponent` | Attach to any `IScreenObject` | Add custom update/render/input behaviour |
| Custom `IRenderStep` | Register via `GameHost` | Inject new visual layers into the render pipeline |
| Custom `IRenderer` | Register via `GameHost` | Replace the entire render strategy for a surface type |
| Subclass `ScreenSurface` | Core | Specialised surface types (e.g., `AnimatedScreenObject`, `LayeredScreenSurface`) |
| Subclass `ControlBase` | UI/Controls | New GUI widgets |
| `IConfigurator` / `Builder` | `Configuration/` | Hook into game startup pipeline |
| `ColoredString` parser | `StringParser/` | Custom markup commands in strings |
| `SadConsole.Extended` | Separate package | Ready-made extras: `SmoothMove`, `MouseDrag`, transition effects, C64/classic keyboard handlers, extended UI |

---

## 8. Key Namespaces and Important Files

| Namespace | Files | Notes |
|---|---|---|
| `SadConsole` | `GameHost.cs`, `ScreenObject.cs`, `ScreenSurface.cs`, `CellSurface.cs`, `ColoredGlyph.cs`, `Settings.cs` | Core object model |
| `SadConsole.Renderers` | `IRenderer.cs`, `IRenderStep.cs`, `ITexture.cs` (core); `ScreenSurfaceRenderer.cs`, `Steps/` (host) | Render contracts + MonoGame impl |
| `SadConsole.Components` | `IComponent.cs`, `IComponentHost.cs`, `Cursor.cs`, `LayeredSurface.cs` | Component system |
| `SadConsole.Input` | `Keyboard.cs`, `Mouse.cs`, `MouseScreenObjectState.cs` | Input abstractions |
| `SadConsole.UI` | `ControlHost.cs`, `ControlsConsole.cs`, `WindowConsole.cs`, `Colors.cs` | Controls infrastructure |
| `SadConsole.UI.Controls` | `ControlBase.cs`, `Button.cs`, `TextBox.cs`, `ListBox.cs`, … | Individual widgets |
| `SadConsole.Configuration` | `Builder.cs`, `IConfigurator.cs` | Game startup fluent API |
| `SadConsole.Host` (MonoGame) | `Game.Mono.cs`, `GameTexture.cs`, `Keyboard.cs`, `Mouse.cs` | MonoGame host entry point |
| `SadConsole.Renderers` (MonoGame) | `ScreenSurfaceRenderer.cs`, `Steps/` | Concrete MonoGame rendering |
| `SadConsole.Entities` | `Entity.cs`, `EntityManager.cs` | High-count movable objects |
| `SadConsole.StringParser` | `ColoredString.Parse.cs` | Inline markup (`[c:r f:red]text[c:u]`) |

### Files to read first as a new contributor

1. `SadConsole/IScreenObject.cs` — understand the scene-graph contract
2. `SadConsole/IScreenSurface.cs` — understand what a renderable surface is
3. `SadConsole/Renderers/IRenderer.cs` + `IRenderStep.cs` — understand the render pipeline contracts
4. `SadConsole/GameHost.cs` — understand what a host must provide
5. `SadConsole.Host.MonoGame/Game.Mono.cs` — see how MonoGame satisfies that contract
6. `SadConsole.Host.MonoGame/Renderers/ScreenSurfaceRenderer.cs` — see a concrete renderer
7. `SadConsole/UI/ControlHost.cs` — see how controls layer on top of a surface

---

## 9. Game Loop

Understanding how a single frame happens is the key to understanding how SadConsole objects get driven. The SFML host provides the clearest view because its loop is explicit and self-contained in one method. MonoGame achieves exactly the same result but delegates to the framework's component model.

### SFML host — the canonical loop (`SadConsole.Host.SFML/Game.cs`)

After `Game.Create(...)` is called, initialization runs synchronously:

1. **Fonts** are loaded and the default font is selected.
2. The **SFML `RenderWindow`** is created at the requested resolution.
3. Two **`SFML.System.Clock`** instances are created — one for the update delta, one for the draw delta — to measure elapsed time independently for each phase.
4. **Renderer and step types** are registered in the `GameHost` type registry (`SetRenderer` / `SetRendererStep`). These are *type* registrations, not instances; concrete objects are created on demand per surface.
5. Remaining `IConfigurator` startup objects run (creating the starting console, invoking the user's `OnStart` callback, etc.).

`Game.Run()` then enters a `while (window.IsOpen)` loop. Each iteration is one frame:

```
┌─────────────────────────────────────────────────────────────────┐
│  Frame N                                                        │
│                                                                 │
│  ① Measure update delta                                         │
│     UpdateFrameDelta = UpdateTimer.Elapsed; UpdateTimer.Restart │
│                                                                 │
│  ② UPDATE  (Settings.DoUpdate)                                  │
│     a. Run RootComponents  (pre-screen global logic)            │
│     b. Keyboard.Update(delta) → route to focused screen object  │
│     c. Mouse.Update(delta) → Mouse.Process()                    │
│     d. Screen.Update(delta)  ← walks scene graph recursively:   │
│            IScreenObject.Update                                 │
│              → child.Update  (each child)                       │
│              → IComponent.Update  (each attached component)     │
│     e. InvokeFrameUpdate() → fires GameHost.FrameUpdate event   │
│                                                                 │
│  ③ Measure draw delta                                           │
│     DrawFrameDelta = DrawTimer.Elapsed; DrawTimer.Restart       │
│                                                                 │
│  ④ DRAW  (Settings.DoDraw)                                      │
│     a. DrawCalls.Clear()                                        │
│     b. Screen.Render(delta)  ← walks scene graph recursively:   │
│            IScreenSurface.Renderer.Refresh(surface) [if dirty]  │
│              → each IRenderStep.Refresh / Composing             │
│            IScreenSurface.Renderer.Render(surface)              │
│              → each IRenderStep.Render → enqueue IDrawCall      │
│     c. InvokeFrameDraw() → fires GameHost.FrameRender event     │
│     d. Clear RenderOutput texture                               │
│     e. SharedSpriteBatch.Begin                                  │
│        foreach DrawCall → call.Draw()  (rasterise to texture)   │
│        SharedSpriteBatch.End                                    │
│     f. (DoFinalDraw) Blit RenderOutput → window back buffer     │
│                                                                 │
│  ⑤ GraphicsDevice.Display()      ← flip window buffer           │
│  ⑥ GraphicsDevice.DispatchEvents() ← process OS window events  │
└─────────────────────────────────────────────────────────────────┘
```

Key timing detail: the update delta and draw delta are **measured separately**. If drawing is skipped (`Settings.DoDraw = false`) the update clock still ticks accurately. `Settings.DoUpdate` and `Settings.DoDraw` are independent flags, so either phase can be disabled without affecting the other.

### MonoGame host — same logic, framework-managed loop

The MonoGame host achieves the same result but does not own an explicit `while` loop. Instead:

- **`SadConsole.Host.Game`** (`MonoGame/Game.Mono.cs`) subclasses `Microsoft.Xna.Framework.Game`. MonoGame's own framework owns the loop.
- In `Game.Initialize()`, a **`SadConsoleGameComponent`** (`MonoGame/SadConsoleGameComponent.Mono.cs`) — a `DrawableGameComponent` — is created and added to `Game.Components`. MonoGame calls its `Update` and `Draw` overrides automatically each frame.
- **`SadConsoleGameComponent.Update(GameTime)`** performs exactly the same steps as the SFML update phase — RootComponents, keyboard, mouse, `Screen.Update`, `InvokeFrameUpdate`.
- **`SadConsoleGameComponent.Draw(GameTime)`** performs exactly the same steps as the SFML draw phase — clear DrawCalls, `Screen.Render`, `InvokeFrameDraw`, render to `RenderOutput`, blit to back buffer.
- Timing comes from MonoGame's `GameTime.ElapsedGameTime` rather than explicit SFML clocks.
- SadConsole-specific initialization (`SadConsole.Game.Initialize()`) runs inside `SadConsoleGameComponent.Initialize()`, which MonoGame triggers as part of its own startup sequence.

The split across `Game.Mono.cs`, `SadConsoleGameComponent.Mono.cs`, and `SadConsole.Game.cs` looks complex at first, but the execution path through those files mirrors the single-method loop in the SFML host exactly.

### What drives your game objects each frame

| Call site | What it drives |
|---|---|
| `Screen.Update(delta)` | Recursively updates every `IScreenObject` in the scene graph and all their attached `IComponent`s |
| `Screen.Render(delta)` | Recursively renders every `IScreenSurface` — refreshes dirty surfaces and enqueues draw calls |
| `GameHost.FrameUpdate` event | Per-frame hook for user code that doesn't want to subclass |
| `GameHost.FrameRender` event | Post-scene-graph hook for user code that wants to enqueue extra draw calls |
| `RootComponent.Run(delta)` | Pre-screen global components that run before the scene graph update |

The `Screen` property on `GameHost` is the root of the scene graph — typically the starting console created during startup, but replaceable at any time to swap game states.

---

## 10. Dependency Summary

```
Your App
  └─ SadConsole.Host.MonoGame
       ├─ SadConsole          (core — no rendering deps)
       ├─ SadRogue.Primitives (Point, Rectangle, Color, etc.)
       └─ MonoGame.Framework  (windowing + GPU)

Optional:
  └─ SadConsole.Extended
       └─ SadConsole
```

`SadRogue.Primitives` is the shared math/geometry library used throughout core and hosts for `Point`, `Rectangle`, `Color`, and related types. It is not a SadConsole-specific library — it comes from the GoRogue project family.

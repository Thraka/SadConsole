![SadConsole Logo](https://raw.githubusercontent.com/Thraka/SadConsole/master/images/SadConsoleLogo.gif)

[![Chat on discord](https://img.shields.io/discord/501465397518925843.svg)](https://discord.gg/pAFNKYjczM)
[![NuGet](https://img.shields.io/nuget/v/SadConsole.svg)][nuget]

SadConsole is a C#-based .NET cross-platform terminal, ASCII, console, and game engine. It simulates these types of programs and can be used to make ASCII-styled games for modern platforms. At its heart, SadConsole is a tile-based game engine with an object model similar to a traditional console application.

This host renders SadConsole through SkiaSharp on an OpenGL GPU without owning a native window. A client application supplies the window, event loop, OpenGL-backed `GRContext`, GPU `SKCanvas`, frame timing, and native input translation. The host does not provide a raster fallback.

The client owns the `GRContext`. Its OpenGL context must be current on the calling thread while creating, rendering, mutating textures, reading texture pixels, or disposing the game. The context must outlive the game and all host textures.

## Startup

Configure SadConsole with the normal builder APIs, as demonstrated by the BasicInterpreter sample, and then create and start the host:

```csharp
Builder startup = new Builder()
    .SetWindowSizeInCells(80, 25)
    .SetStartingScreen(game => new Console(80, 25))
    .IsStartingScreenFocused(true)
    .ConfigureFonts();

Game.Create(startup, graphicsContext);
Game.Instance.Run();
```

`graphicsContext` must be an active OpenGL `GRContext`. `Game.Create` verifies the backend and allocates a validation render target; it throws if the context is abandoned or cannot create texture-backed Skia resources. For controls such as Blazor's `SKGLView`, initialize the game from the first OpenGL paint callback so the control's context is current.

`Game.Run()` starts the SadConsole lifecycle but does not block or create a native event loop. Do not use the shared `Builder.Run()` convenience extension with this host because that extension disposes the game as soon as the non-blocking `Run()` method returns.

## Client frame bridge

The client should perform the following work from its own frame or paint callbacks:

```csharp
TimeSpan elapsed = clock.Elapsed;
clock.Restart();

Game.Instance.Update(elapsed);
Game.Instance.Render(canvas, destination, elapsed);
```

The canvas must be GPU-backed by the same `GRContext` supplied to `Game.Create`; raster canvases and canvases from another context are rejected. The overload without a destination rectangle renders at SadConsole's logical pixel size. The destination overload scales the logical output to the client-provided rectangle using nearest-neighbor sampling.

Call `Game.Instance.Stop()` and `Game.Instance.Dispose()` while the associated OpenGL context is current when the client closes. Disposing the game releases its GPU resources but does not dispose the client-owned `GRContext`.

## GPU textures

`GameTexture` stores persistent content in budgeted GPU `SKSurface` render targets and exposes texture-backed `SKImage` snapshots for drawing. Encoded images and existing `SKBitmap` instances are uploaded and are not retained as the render representation.

The `ITexture.GetPixel` and `GetPixels` methods synchronously read data from the GPU. `SetPixel` draws directly into the GPU render target, while `SetPixels` creates a temporary CPU upload buffer. Avoid per-frame pixel readback and bulk uploads when possible.

OpenGL context loss is terminal for an existing game. If the context is abandoned or replaced, dispose the game, create a new context, and recreate the game and its textures.

## Input bridge

Translate native key and pointer values in the client and publish snapshots through the exposed state objects:

```csharp
Game.Instance.KeyboardState.SetState(pressedSadConsoleKeys, capsLock, numLock);
Game.Instance.MouseState.SetState(pointerPosition, leftDown, rightDown, middleDown, wheelValue);
```

Mouse positions are logical SadConsole render coordinates. If the client scales or offsets the render destination, transform native pointer coordinates back into that logical coordinate space before calling `SetState`.

## Client-owned operations

`ResizeWindow` and `ToggleFullScreen` cannot directly manipulate a native window. Clients can handle `ViewportSizeRequested` and `FullscreenRequested` to apply those requests to their own windowing system.

The initial renderer supports `Console` and `ScreenSurface`, including nested screen objects, bitmap-font colors, decorators, mirroring, opacity, and viewport regions. Specialized controls, layered surfaces, entities, and row-font renderers are not registered yet.

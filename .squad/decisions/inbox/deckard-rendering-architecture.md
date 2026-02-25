# Decision Note: Rendering Architecture Document

**Author:** Deckard  
**Date:** 2026  
**Status:** Informational — architecture documented, no API changes made

## What Was Done

Authored `docs/architecture-rendering.md` covering the full rendering pipeline for SadConsole.

## Key Findings

### Pipeline Structure
The rendering system uses a three-tier compositing model:
1. Each `IRenderStep` renders into its own private texture.
2. `IRenderer` composites all steps into its `Output` texture (`_backingTexture`) during the `Composing` phase.
3. `GameHost` flushes all renderers' output textures into `Global.RenderOutput`, which is blitted to the OS window.

### IRenderStep — Three Phases
Steps implement three distinct methods called in strict order each frame:
- **`Refresh`** — update private state/texture; return `true` to trigger compositing.
- **`Composing`** — blit private texture onto the renderer's `_backingTexture` (only called if any step requested it).
- **`Render`** — enqueue draw calls into `GameHost.DrawCalls` (always called).

This caching design means cells are only re-rasterized when `IsDirty` is true or a force-refresh is requested.

### Registration System
Renderers and steps are string-keyed singletons of `Type` in `GameHost`. Hosts register concrete types at startup (`SetRenderer`, `SetRendererStep`); instances are created on demand via `Activator.CreateInstance`. This is the primary extension point.

### Host Differences
MonoGame and SFML implement identical logical structures with different graphics APIs. MonoGame adds `IRendererMonoGame` (a `LocalSpriteBatch` per renderer, `MonoGameBlendState`) because XNA's render target is a device-level concept. SFML doesn't need this because its `RenderTexture` is self-contained.

`OptimizedScreenSurfaceRenderer` exists as a step-free fast path for surfaces that don't need extensibility.

## Implications for the Team

- Any new rendering feature should be implemented as a new `IRenderStep`, not by modifying existing renderers. Steps are the intended extension point.
- If a step needs to write to `_backingTexture`, it must do so in `Composing` (sprite batch is open). Writing in `Refresh` or `Render` will not target the correct texture.
- The `SortOrder` values in `Constants.RenderStepSortValues` should be treated as reserved ranges. Custom steps should pick values in the gaps (e.g., 15–49, 51–59).
- Gaff should be aware: `IRendererMonoGame.LocalSpriteBatch` is created once per renderer instance — dispose it properly in custom renderers that subclass `ScreenSurfaceRenderer`.

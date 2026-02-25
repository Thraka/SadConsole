# Decision: CellSurface / ScreenSurface Architecture Documentation

**Author:** Deckard  
**Date:** 2026  
**Status:** Record / FYI

## Context

Thraka requested a thorough architecture document for `CellSurface` and `ScreenSurface` — the two foundational types in the library. This records the key architectural patterns documented so the team can reference them.

## Key Findings

### Data / Display Split is Intentional

`CellSurface` is a pure data object with zero rendering or host dependencies. `ScreenSurface` wraps an `ICellSurface` reference and adds rendering, scene-graph position, and input. This separation is load-bearing — it enables off-screen buffers, asset importers, serialization, and shared surfaces.

**Team implication:** Never add rendering code to `CellSurface`. Never add host-specific imports to `CellSurface.cs` or `ICellSurface.Editor.cs`.

### Editing API Is Extension Methods Only

All cell mutation methods (Print, Fill, Clear, Copy, Shift, Draw*) are `static` extension methods on `ISurface` in `CellSurfaceEditor` (`ICellSurface.Editor.cs`). Adding new editing operations means adding more extension methods there — not adding instance methods to `CellSurface`.

### ISurface Is the Shim

The `ISurface` interface (single property: `ICellSurface Surface { get; }`) is what threads extension methods through composite types. Any new type that should support the full editing API should implement `ISurface`.

### Surface Ownership Is by Reference

`ScreenSurface.Surface` is a settable reference. Surfaces can be shared between `ScreenSurface` instances. Use `QuietSurfaceHandling = true` on secondary consumers to suppress redundant dirty-change events.

### Coordinate System Has Two Modes

`ScreenSurface.Position` is in **cell units** by default; `AbsolutePosition` = `Position × FontSize + Parent.AbsolutePosition`. `UsePixelPositioning = true` puts `Position` in raw pixels. All layout code must be aware of which mode is active.

## Document

Full architecture document: `docs/architecture-surfaces.md`

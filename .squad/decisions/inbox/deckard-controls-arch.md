# Decision: Controls System Architecture Document Published

**Author:** Deckard  
**Date:** 2026  
**Status:** Informational (no code changes)

## Summary

A thorough architecture document for SadConsole's controls system has been written to `docs/architecture-controls.md`.

## What was documented

The document covers:

1. **Control hierarchy** — full inheritance tree from `ControlBase` through all 20+ concrete controls, including `CompositeControl` and `Panel` for nested-control scenarios.
2. **Container system** — `ControlHost` as an `IComponent`, focus management (the `FocusedControl` ↔ `IsFocused` round-trip), tab order via `TabIndex` + `ReOrderControls`, mouse capture, and cross-console tabbing.
3. **Rendering pipeline** — the self-painting pattern (`UpdateAndRedraw` writes into `Surface`), `ControlHostRenderStep` (sort order 80), `BackingTexture` → `Composing` blit, recursive `ProcessContainer` for nested containers.
4. **Theme system** — `Colors` (palette) → `ThemeStates` (6-slot per-state appearances) → `ControlBase.ThemeState`, `AdjustableColor` (brightness-mapped colors), priority order in `GetStateAppearance`.
5. **Input handling** — reverse-order mouse hit testing, `IsMouseButtonStateClean` guard, keyboard routed to `FocusedControl` with Tab/Shift-Tab fallback.
6. **Extension points** — subclassing `ControlBase`, `CompositeControl`, `ControlHost`; custom `Colors` schemes; `SadConsole.Extended` additions.

## Relevance to team

- **Roy** — controls are fully above the core via `IComponent`; no core changes needed for new control types.
- **Pris** — this doc is the canonical reference for controls/theme work. Key patterns: dirty-then-repaint, state-priority appearance lookup.
- **Gaff** — `ControlHostRenderStep` is the only controls-related code in host projects; it lives at `SadConsole.Host.*/Renderers/Steps/ControlHostRenderStep.cs`.
- **Rachael** — test surface for controls: instantiate a `ControlsConsole`, add controls, simulate mouse/keyboard via `ProcessMouse`/`ProcessKeyboard`.

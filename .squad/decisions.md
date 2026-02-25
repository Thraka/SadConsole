# Team Decisions

This file is the authoritative decision ledger. All agents read this. Only Scribe writes to it (by merging from the inbox).

---

<!-- Decisions will be appended here by Scribe as they are made. -->

---

## 2026-02-25 — Architecture Document Published

**Author:** Deckard | **Status:** Informational

A formal architecture document has been created at `docs/architecture.md` as the canonical reference for new contributors and the team. It covers: SadConsole purpose, project/folder structure, core/host separation, key abstractions (`GameHost`, `IScreenObject`, `IScreenSurface`, `ICellSurface`, `IRenderer`, `IRenderStep`, `ITexture`, `IFont`, `IComponent`), per-frame data flow, controls/UI subsystem, extension points, and a namespace/file reference table.

**Team implication:** Roy, Gaff, Pris, and Rachael should treat `docs/architecture.md` as the source of truth when onboarding contributors. Update it in the same PR as any significant architectural change. Gaff should verify the render pipeline section (Section 5).

---

## 2026-02-25 — Rendering Architecture Document

**Author:** Deckard | **Status:** Informational

Full rendering pipeline documented at `docs/architecture-rendering.md`. Three-tier compositing: each `IRenderStep` → renderer `_backingTexture` → `Global.RenderOutput` → OS window. `IRenderStep` has three phases called in strict per-frame order: `Refresh` (private texture, returns bool to gate compositing), `Composing` (blit to renderer output), `Render` (enqueue draw calls). Renderers and steps are string-keyed `Type` singletons in `GameHost`, instantiated on demand via `Activator.CreateInstance`. MonoGame adds `IRendererMonoGame` (per-renderer `LocalSpriteBatch`, `MonoGameBlendState`).

**Team implications:** New rendering features should be new `IRenderStep` implementations. Steps must write to `_backingTexture` only in `Composing`. `SortOrder` values in `Constants.RenderStepSortValues` are reserved — custom steps should use gaps (15–49, 51–59). Gaff: `IRendererMonoGame.LocalSpriteBatch` is created once per renderer instance — dispose it properly.

---

## 2026-02-25 — CellSurface / ScreenSurface Architecture Document

**Author:** Deckard | **Status:** Record / FYI

Full architecture document at `docs/architecture-surfaces.md`. Key decisions: `CellSurface` is a pure data object with zero rendering dependencies — never add rendering code there. All cell mutation is extension methods on `ISurface` in `CellSurfaceEditor` (`ICellSurface.Editor.cs`). `ISurface` (single property: `ICellSurface Surface { get; }`) is the shim threading extension methods through composite types. `ScreenSurface.Surface` is a settable reference — surfaces can be shared; use `QuietSurfaceHandling = true` on secondary consumers. `Position` is in cell units by default; `UsePixelPositioning = true` switches to raw pixels.

---

## 2026-02-25 — Controls System Architecture Document

**Author:** Deckard | **Status:** Informational

Full architecture document at `docs/architecture-controls.md`. Controls live in `SadConsole/UI/` via `IComponent` — no special surface subclass required. `ControlBase.UpdateAndRedraw` is abstract; each control self-paints into its own `ICellSurface`. `ControlHost` manages focus (`FocusedControl` ↔ `IsFocused`), tab order (`TabIndex` + `ReOrderControls`), mouse capture, reverse-order hit testing, and injects `ControlHostRenderStep` (sort 80). Theme resolution: control override → host `ThemeColors` → `Colors.Default`. `IsMouseButtonStateClean` guard prevents spurious clicks.

**Team implications:** Roy — controls are fully above core via `IComponent`. Pris — dirty-then-repaint and state-priority appearance lookup are the canonical patterns. Gaff — `ControlHostRenderStep` is the only controls code in host projects. Rachael — test via `ControlsConsole` + `ProcessMouse`/`ProcessKeyboard`.

---

## 2026-02-25 — Test Coverage Gaps Analysis

**Author:** Rachael | **Status:** Informational — action items for Roy, Pris, Deckard

Test suite is significantly under-coverage (~106 unit tests, 31 benchmarks across hundreds of public classes). Full report at `docs/test-coverage-gaps.md`.

**Covered (reasonably):** `CellSurface` operations, `ScreenObject` child tree, `Extended.Table`, basic serialization round-trips.

**Not covered at all:** `ColoredString` parser + all 10+ `ParseCommand*` types (highest risk), all 20+ UI controls, effects system, `Cursor` component, input subsystem, ANSI processing, file readers, `Algorithms.cs`, `LayeredScreenSurface`, `Instructions` system, all host implementations.

**Action items:** Pris — add tests for TextBox, ListBox, ScrollBar (high regression risk). Roy — add tests for `Cursor` and `ColoredString` parser before any changes. Deckard — coverage is ~5–10%; no CI gate exists; discuss whether to add a minimum threshold. Rachael — will write `ColoredString.Parse.cs` and `Cursor` tests next cycle.

---

## 2026-02-25 — architecture-surfaces.md Has Interface Stub Errors

**Author:** Holden | **Status:** Flag for Deckard — corrections required

Holden reviewed `docs/architecture-surfaces.md` against source. Document is broadly correct but `IRenderer` and `IRenderStep` interface stubs contain factual errors that will mislead implementors. Full review at `docs/architecture-surfaces-review.md`.

**Highest-priority fixes:**
1. `IRenderer.OnHostUpdated` — doc shows `void OnHostUpdated(IScreenSurface surface)`; code has `void OnHostUpdated(IScreenObject host)` (`IRenderer.cs:53`).
2. `IRenderStep.Refresh` — doc shows 2 params; code has 4: `bool Refresh(IRenderer renderer, IScreenSurface screenObject, bool backingTextureChanged, bool isForced)` (`IRenderStep.cs:39`).
3. `IRenderer` stub is incomplete — `ITexture? Output` should be non-nullable; `Steps` should be `{ get; set; }`; missing `string Name`, `byte Opacity`, `bool IsForced`.
4. `IRenderStep` stub is also incomplete — four members entirely absent: `Name`, `SetData`, `Reset`, `OnHostUpdated`.
5. `LayeredScreenSurface` renderer claim wrong — code disposes default renderer and directly assigns a new one in constructor; no `DefaultRendererName` override exists.
6. `SetSurface(...)` vs `Surface { set; }` conflated — `SetSurface` (from `ICellSurfaceSettable`) remaps cell array within a `CellSurface`; `Surface { set; }` (from `ISurfaceSettable`) replaces the `ICellSurface` reference on `ScreenSurface`. Different operations, not interchangeable.

**Secondary issues:** `Effects.DropInvalidCells()` vs `Effects.RemoveAll()` on resize with `clear=true`; `IsDirtySet` not subscribed by `EffectsManager` (propagation only via subclass override of `OnIsDirtyChanged()`); `OnIsDirtyChanged()` base is a no-op; `EffectsManager` clones cell state (undocumented); `ICellSurface` base interfaces (`IGridView<ColoredGlyphBase>`, `IEnumerable<ColoredGlyphBase>`) not documented; `ConnectedLineEmpty` static array missing from doc.

# Team Decisions — Archive

Decisions dated before 2026-02-27 (30+ days old). Maintained for reference.

---
## 2026-03-02 — Font Architecture Analysis Complete

**Author:** Roy | **Date:** 2026-02-26 | **Status:** Informational

Completed deep-dive analysis of the SadConsole font system. Full technical specification written to `.squad/agents/roy/font-analysis.md` for use in Deckard's architecture document.

**Key findings:** Clean core/host separation (no circular dependencies); font size scaling is logical, not physical (Quarter/Half/One/Two/Three/Four multipliers apply at render time); named glyphs via GlyphDefinition enable rich typography without extra surface layers; cell decorators are lightweight and stackable (~3 ints + flags per decorator); font serialization avoids texture bloat by storing only font name (resolved at deserialization); legacy remapping code preserved for backward compatibility.

**Deliverables:** `.squad/agents/roy/font-analysis.md` (30KB+, 17 sections); updated `.squad/agents/roy/history.md`.

---


---
## 2026-02-25 — Font Architecture is Sound Across All Hosts

**Author:** Gaff (Host Dev) | **Date:** 2026-02-25 | **Status:** Informational

Deep analysis of font loading, glyph mapping, and rendering across MonoGame, SFML, FNA, and KNI hosts concludes the font architecture is **cross-host consistent and well-designed**. No breaking changes or architectural refactors are needed.

**Key findings:** Grid layout calculation is identical across all hosts (single formula in `SadFont.GenerateGlyphSourceRectangle()`); texture type abstraction (`ITexture` → per-host `GameTexture`) works well; rendering patterns are parallel (all use same glyph rect lookup); extended fonts, decorators, and font scaling work consistently; rectangle types, font editing, and texture disposal are all correct (not issues).

**Recommendations for Deckard's architecture document:** Include 7 sections (data model, grid formula, texture loading, rendering pipeline, scaling, extended fonts, fallback). For Rachael: 6 regression tests needed. For future hosts: 4-step integration guide.

**Deliverables:** `.squad/agents/gaff/font-analysis.md` (38.7KB); updated `.squad/agents/gaff/history.md`.

---


---
## 2026-03-02 — User Directive: Ignore SadConsole.Fonting

**By:** Thraka (via Copilot) | **Date:** 2026-03-02 | **Status:** Record

User directive: **Ignore SadConsole.Fonting** — it is an unfinished experiment. Do not include it in documentation or analysis of the font system.

**Rationale:** User request to exclude incomplete work from team documentation.

---


---
## 2026-02-25 — Architecture Document Published

**Author:** Deckard | **Status:** Informational

A formal architecture document has been created at `docs/architecture.md` as the canonical reference for new contributors and the team. It covers: SadConsole purpose, project/folder structure, core/host separation, key abstractions (`GameHost`, `IScreenObject`, `IScreenSurface`, `ICellSurface`, `IRenderer`, `IRenderStep`, `ITexture`, `IFont`, `IComponent`), per-frame data flow, controls/UI subsystem, extension points, and a namespace/file reference table.

**Team implication:** Roy, Gaff, Pris, and Rachael should treat `docs/architecture.md` as the source of truth when onboarding contributors. Update it in the same PR as any significant architectural change. Gaff should verify the render pipeline section (Section 5).

---


---
## 2026-02-25 — Rendering Architecture Document

**Author:** Deckard | **Status:** Informational

Full rendering pipeline documented at `docs/architecture-rendering.md`. Three-tier compositing: each `IRenderStep` → renderer `_backingTexture` → `Global.RenderOutput` → OS window. `IRenderStep` has three phases called in strict per-frame order: `Refresh` (private texture, returns bool to gate compositing), `Composing` (blit to renderer output), `Render` (enqueue draw calls). Renderers and steps are string-keyed `Type` singletons in `GameHost`, instantiated on demand via `Activator.CreateInstance`. MonoGame adds `IRendererMonoGame` (per-renderer `LocalSpriteBatch`, `MonoGameBlendState`).

**Team implications:** New rendering features should be new `IRenderStep` implementations. Steps must write to `_backingTexture` only in `Composing`. `SortOrder` values in `Constants.RenderStepSortValues` are reserved — custom steps should use gaps (15–49, 51–59). Gaff: `IRendererMonoGame.LocalSpriteBatch` is created once per renderer instance — dispose it properly.

---


---
## 2026-02-25 — CellSurface / ScreenSurface Architecture Document

**Author:** Deckard | **Status:** Record / FYI

Full architecture document at `docs/architecture-surfaces.md`. Key decisions: `CellSurface` is a pure data object with zero rendering dependencies — never add rendering code there. All cell mutation is extension methods on `ISurface` in `CellSurfaceEditor` (`ICellSurface.Editor.cs`). `ISurface` (single property: `ICellSurface Surface { get; }`) is the shim threading extension methods through composite types. `ScreenSurface.Surface` is a settable reference — surfaces can be shared; use `QuietSurfaceHandling = true` on secondary consumers. `Position` is in cell units by default; `UsePixelPositioning = true` switches to raw pixels.

---


---
## 2026-02-25 — Controls System Architecture Document

**Author:** Deckard | **Status:** Informational

Full architecture document at `docs/architecture-controls.md`. Controls live in `SadConsole/UI/` via `IComponent` — no special surface subclass required. `ControlBase.UpdateAndRedraw` is abstract; each control self-paints into its own `ICellSurface`. `ControlHost` manages focus (`FocusedControl` ↔ `IsFocused`), tab order (`TabIndex` + `ReOrderControls`), mouse capture, reverse-order hit testing, and injects `ControlHostRenderStep` (sort 80). Theme resolution: control override → host `ThemeColors` → `Colors.Default`. `IsMouseButtonStateClean` guard prevents spurious clicks.

**Team implications:** Roy — controls are fully above core via `IComponent`. Pris — dirty-then-repaint and state-priority appearance lookup are the canonical patterns. Gaff — `ControlHostRenderStep` is the only controls code in host projects. Rachael — test via `ControlsConsole` + `ProcessMouse`/`ProcessKeyboard`.

---


---
## 2026-02-25 — Test Coverage Gaps Analysis

**Author:** Rachael | **Status:** Informational — action items for Roy, Pris, Deckard

Test suite is significantly under-coverage (~106 unit tests, 31 benchmarks across hundreds of public classes). Full report at `docs/test-coverage-gaps.md`.

**Covered (reasonably):** `CellSurface` operations, `ScreenObject` child tree, `Extended.Table`, basic serialization round-trips.

**Not covered at all:** `ColoredString` parser + all 10+ `ParseCommand*` types (highest risk), all 20+ UI controls, effects system, `Cursor` component, input subsystem, ANSI processing, file readers, `Algorithms.cs`, `LayeredScreenSurface`, `Instructions` system, all host implementations.

**Action items:** Pris — add tests for TextBox, ListBox, ScrollBar (high regression risk). Roy — add tests for `Cursor` and `ColoredString` parser before any changes. Deckard — coverage is ~5–10%; no CI gate exists; discuss whether to add a minimum threshold. Rachael — will write `ColoredString.Parse.cs` and `Cursor` tests next cycle.

---


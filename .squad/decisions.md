# Team Decisions

This file is the authoritative decision ledger. All agents read this. Only Scribe writes to it (by merging from the inbox).

---

## 2026-03-02 — Font Architecture Analysis Complete

**Author:** Roy | **Date:** 2026-02-26 | **Status:** Informational

Completed deep-dive analysis of the SadConsole font system. Full technical specification written to `.squad/agents/roy/font-analysis.md` for use in Deckard's architecture document.

**Key findings:** Clean core/host separation (no circular dependencies); font size scaling is logical, not physical (Quarter/Half/One/Two/Three/Four multipliers apply at render time); named glyphs via GlyphDefinition enable rich typography without extra surface layers; cell decorators are lightweight and stackable (~3 ints + flags per decorator); font serialization avoids texture bloat by storing only font name (resolved at deserialization); legacy remapping code preserved for backward compatibility.

**Deliverables:** `.squad/agents/roy/font-analysis.md` (30KB+, 17 sections); updated `.squad/agents/roy/history.md`.

---

## 2026-02-25 — Font Architecture is Sound Across All Hosts

**Author:** Gaff (Host Dev) | **Date:** 2026-02-25 | **Status:** Informational

Deep analysis of font loading, glyph mapping, and rendering across MonoGame, SFML, FNA, and KNI hosts concludes the font architecture is **cross-host consistent and well-designed**. No breaking changes or architectural refactors are needed.

**Key findings:** Grid layout calculation is identical across all hosts (single formula in `SadFont.GenerateGlyphSourceRectangle()`); texture type abstraction (`ITexture` → per-host `GameTexture`) works well; rendering patterns are parallel (all use same glyph rect lookup); extended fonts, decorators, and font scaling work consistently; rectangle types, font editing, and texture disposal are all correct (not issues).

**Recommendations for Deckard's architecture document:** Include 7 sections (data model, grid formula, texture loading, rendering pipeline, scaling, extended fonts, fallback). For Rachael: 6 regression tests needed. For future hosts: 4-step integration guide.

**Deliverables:** `.squad/agents/gaff/font-analysis.md` (38.7KB); updated `.squad/agents/gaff/history.md`.

---

## 2026-02-26 — Font System Architecture Document Created

**Author:** Deckard | **Date:** 2026-02-26 | **Status:** Informational

Created `docs/architecture-fonts.md` — comprehensive architecture reference document for SadConsole's font system. Synthesized from Roy's core font analysis and Gaff's host rendering analysis.

**Scope:** IFont interface and SadFont implementation; .font JSON format and PNG spritesheet layout; glyph indexing and rectangle lookup; extended font support (GlyphDefinition, CellDecorator); font size scaling model; font registration and loading pipeline (GameHost.Fonts, LoadFont, FontConfig); surface-font integration and coordinate impact; host rendering pipeline across all hosts (MonoGame, SFML, FNA, KNI); font serialization strategy (name-based); design principles. **Excluded per user directive:** SadConsole.Fonting (unfinished experiment).

**Team implications:** Roy — font metadata types fully documented (refer contributors to this doc for font-related core changes); Gaff — host rendering pipeline captured (verify accuracy against future host changes); Pris — Font/FontSize independence section documents control usage; Rachael — test coverage gaps documented per Gaff's analysis; All — SadConsole.Fonting deliberately excluded.

**Status:** ✅ Complete. Architecture document ready for team reference and contributor onboarding.

---

## 2026-03-02 — Font Architecture Verification & Corrections Complete

**Author:** Holden (verification), Deckard (corrections) | **Date:** 2026-03-02 | **Status:** Complete

Holden verified `docs/architecture-fonts.md` against SadConsole source and found the document 99% accurate, with one minor gap identified in Section 7 (font change notification mechanism). The gap: Section 7 incorrectly stated that `Renderer?.OnHostUpdated(this)` is explicitly called on font changes. Actual behavior is **passive**: `SurfaceRenderStep.Refresh()` automatically detects dimension changes via `AbsoluteArea` and reallocates the backing texture.

Deckard applied 5 corrections:
1. Section 3: Clarified `IsSadExtended` as a **feature flag** in `GlyphDefinition` (distinct from SadFont.Extended readonly bool)
2. Section 3: Documented `GlyphDefinition` **independence from SadFont** — design pattern for named glyphs without file changes
3. Section 6: Added documentation for **`CellDecoratorHelpers`** utilities (GetRectangleFromDecorators, GetCountFromDecorators, MirrorDecorators)
4. Section 2: Removed KNI host reference per Thraka directive (experimental)
5. Section 7: Revised font change notification to explain **passive texture reallocation detection**

**Files modified:** `docs/architecture-fonts.md`

**Team directives captured:**
- **KNI host:** Experimental — do not document (Thraka)
- **SadConsole.Fonting:** Excluded (already done)

**Status:** ✅ Font architecture documentation complete, verified, and corrected.

---

## 2026-03-02 — User Directive: KNI Host is Experimental

**By:** Thraka (via Copilot) | **Date:** 2026-03-02 | **Status:** Record

User directive: **KNI host is experimental** — do not document it in any way. Do not include in team documentation or architecture analysis.

**Rationale:** User request to exclude unfinished/experimental work from team documentation.

---

## 2026-03-02 — User Directive: Ignore SadConsole.Fonting

**By:** Thraka (via Copilot) | **Date:** 2026-03-02 | **Status:** Record

User directive: **Ignore SadConsole.Fonting** — it is an unfinished experiment. Do not include it in documentation or analysis of the font system.

**Rationale:** User request to exclude incomplete work from team documentation.

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

---

## 2026-03-04 — User Directive: ANSI Parser Overhaul Scope

**By:** Thraka (via Copilot) | **Date:** 2026-03-04 | **Status:** Record

Overhaul the ANSI parser (SadConsole/Ansi/) to support the cterm.adoc spec.

**IN SCOPE:**
- Core CSI sequences: full cursor movement (CUU/CUD/CUF/CUB/CHA/CUP/CNL/CPL/HVP), erase (ED/EL/ECH), insert/delete (ICH/DCH/IL/DL), scroll (SU/SD/SL/SR), repeat (REP), tab stops (CHT/CBT/TBC/HTS)
- Full SGR: 256-color palette (38;5;N / 48;5;N), 24-bit true color (38;2;R;G;B / 48;2;R;G;B), dim, blink, concealed, bright fg/bg (90-107), default colors (39/49)
- Fe/Fp/Fs escape sequences: DECSC/DECRC, NEL, RI, HTS, RIS
- DEC private modes (DECSET/DECRST): auto-wrap, origin mode, cursor show/hide, bright background/blink modes
- Scroll margins: DECSTBM (top/bottom), DECSLRM (left/right)
- OSC palette redefinition: OSC 4
- DCS loadable fonts

**OUT OF SCOPE:**
- Device status/attribute queries (DA, DSR)
- Mouse reporting modes
- Sixel graphics
- ANSI music
- SyncTERM-specific APC commands

---

## 2026-03-04 — User Directive: Terminal Namespace with Standalone SadConsole.Terminal

**By:** Thraka (via Copilot) | **Date:** 2026-03-04 | **Status:** Record

**First directive:** SadConsole.Terminal uses namespace `SadConsole.Terminal`. Existing `SadConsole.Ansi.*` classes are deprecated with `[Obsolete]` attributes and thin-wrapper delegation to the new system. New files go under `SadConsole/Terminal/`. Side-by-side coexistence — old code keeps working, removal on Thraka's timeline.

**Clarifying directive:** SadConsole.Terminal is a completely independent, standalone API. The existing SadConsole.Ansi classes do NOT delegate to the new system. No [Obsolete] wrappers, no thin delegation. The two systems coexist side-by-side and users choose which to use. The old Ansi system remains untouched.

---

## 2026-03-04 — User Directive: Terminal.Parser Handler Callback Pattern

**By:** Thraka (via Copilot) | **Date:** 2026-03-04 | **Status:** Record

Terminal.Parser uses the handler callback pattern. Parser calls methods on an `ITerminalHandler` interface (e.g., `OnPrint(char)`, `OnCsiDispatch(params, final)`, `OnEscDispatch(final)`, `OnOscString(payload)`) instead of emitting command objects. Zero allocation, no command classes or structs.

---

## 2026-03-04 — Terminal.Parser Test Contract (WI-0.2)

**Author:** Rachael (Tester) | **Date:** 2026-03-04 | **Status:** Complete

Per Deckard's overhaul plan WI-0.2 and Thraka's directives on namespace (`SadConsole.Terminal`) and handler callback pattern (`ITerminalHandler`), wrote the full test suite for `Terminal.Parser` before the implementation exists (TDD).

**ITerminalHandler contract:**
```csharp
public interface ITerminalHandler
{
    void OnPrint(char ch);
    void OnC0Control(byte b);
    void OnEscDispatch(byte intermediate, byte final_);
    void OnCsiDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte privatePrefix, byte final_);
    void OnOscDispatch(ReadOnlySpan<byte> payload);
    void OnDcsDispatch(ReadOnlySpan<int> parameters, ReadOnlySpan<byte> intermediates, byte final_, ReadOnlySpan<byte> payload);
}
```

**Key decisions:**
- Empty CSI params default to 0 (ECMA-48 spec compliance)
- Private prefix as `byte` (0 = absent, simpler than nullable)
- DEL (0x7F) is ignored
- UTF-8 decoding is parser's responsibility
- CAN/SUB abort without dispatch

**Deliverables:** `Tests/SadConsole.Tests/TerminalParserTests.cs` — 87 test methods across 10 categories plus edge cases and integration scenarios.

---

## 2026-03-04 — Terminal Parser Phase 0 Implemented

**Author:** Roy (Core Dev) | **Date:** 2026-03-04 | **Status:** Complete

Implemented `SadConsole.Terminal.ITerminalHandler` and the ECMA-48 `Parser` state machine under `SadConsole/Terminal/`. 

**Architecture:**
- Standalone system with handler callbacks
- Preallocated parameter/intermediate arrays (zero allocation)
- OSC/DCS payload buffering via `CollectionsMarshal.AsSpan`
- UTF-8 decoding in Ground state
- Ground/Escape/CSI/DCS/OSC states with case-sensitive dispatch

**Test Reconciliation:** Fixed `MockTerminalHandler` signature mismatches (parameter order, nullable private prefix).

**Bug Fixes:** Fixed 3 parser implementation bugs:
1. Empty CSI params — `ESC[m` now correctly dispatches with `params=[0]`
2. BEL-terminated OSC — OSC sequences ending with BEL now dispatch
3. ESC invalid byte recovery — Parser now resets to Ground on invalid escape bytes

**Status:** ✅ All 87 tests pass on net8.0/net9.0/net10.0

---

## 2026-03-04 — User Directive: Parser Encoding API

**By:** Thraka (via Copilot) | **Date:** 2026-03-04 | **Status:** Record

Future enhancement: Replace Parser's `ParserEncoding` enum with accepting a `System.Text.Encoding` instance. Default to null (passthrough: `(char)b` for raw byte-as-glyph, the current CP437 behavior). When an Encoding is provided, use its Decoder for byte→char conversion. This enables `Encoding.GetEncoding(437)`, `Encoding.UTF8`, or any custom encoding. Requires `System.Text.Encoding.CodePages` NuGet for CP437. Not blocking — current enum works fine for now.

---

## 2026-03-04 — User Directive: Auto-Grow Writer & Measurer

**By:** Thraka (via Copilot) | **Date:** 2026-03-04 | **Status:** Record

Two mechanisms for handling ANSI content taller than the surface:

1. **Auto-grow in Writer**: When a scroll-up would occur, check if `_surface is ICellSurfaceResize`. If so, grow the surface height instead of scrolling — call `Resize(viewWidth, viewHeight, totalWidth, totalHeight + 1)` keeping view size the same (it maps to the visible portion). Content accumulates below the viewport. If the surface doesn't implement ICellSurfaceResize, fall back to normal scrolling behavior.

2. **Measuring Writer (Terminal.Measurer)**: A lightweight `ITerminalHandler` that tracks cursor position and scroll count without any surface. Feed a file through it to determine the required height, then create a properly-sized surface and render with the real Writer. For use when the surface doesn't support resizing or when you need to know dimensions upfront.

**Why:** User request — replaces the old double-parse hack in SadConsole.Ansi that read files twice. Auto-grow is preferred (single pass), Measurer is the fallback.

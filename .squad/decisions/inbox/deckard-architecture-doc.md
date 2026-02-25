# Decision: Architecture Document Published

**Author:** Deckard  
**Date:** 2026  
**Status:** Informational

## Decision

A formal architecture document has been created at `docs/architecture.md` to serve as the canonical reference for new contributors and the team.

## Rationale

No single document existed explaining the core/host separation, the render pipeline, the controls subsystem, or the key interfaces. New contributors had to piece this together from source code. Having it written down removes ambiguity and speeds onboarding.

## What the Document Covers

1. High-level overview of SadConsole's purpose
2. Project/folder structure
3. The core/host separation model — core defines contracts, hosts implement them
4. Key abstractions: `GameHost`, `IScreenObject`, `IScreenSurface`, `ICellSurface`, `IRenderer`, `IRenderStep`, `ITexture`, `IFont`, `IComponent`
5. Per-frame data flow (update → refresh → composing → render → GPU flush)
6. Controls/UI subsystem — `ControlHost` as an `IComponent` on any surface
7. Extension points for library consumers
8. Key namespace and file reference table

## Implications for the Team

- Roy, Gaff, Pris, and Rachael should treat `docs/architecture.md` as the source of truth when onboarding contributors to their respective subsystems.
- If a significant architectural change is made, the document should be updated in the same PR.
- The render pipeline section (Section 5) should be reviewed by Gaff to verify it accurately reflects the host-side draw-call flush sequence.

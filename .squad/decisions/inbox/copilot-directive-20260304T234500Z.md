### 2026-03-04T23:45:00Z: User directive
**By:** Thraka (via Copilot)
**What:** Two mechanisms for handling ANSI content taller than the surface:

1. **Auto-grow in Writer**: When a scroll-up would occur, check if `_surface is ICellSurfaceResize`. If so, grow the surface height instead of scrolling — call `Resize(viewWidth, viewHeight, totalWidth, totalHeight + 1)` keeping view size the same (it maps to the visible portion). Content accumulates below the viewport. If the surface doesn't implement ICellSurfaceResize, fall back to normal scrolling behavior.

2. **Measuring Writer (Terminal.Measurer)**: A lightweight `ITerminalHandler` that tracks cursor position and scroll count without any surface. Feed a file through it to determine the required height, then create a properly-sized surface and render with the real Writer. For use when the surface doesn't support resizing or when you need to know dimensions upfront.

**Why:** User request — replaces the old double-parse hack in SadConsole.Ansi that read files twice. Auto-grow is preferred (single pass), Measurer is the fallback.

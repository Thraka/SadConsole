# Decision: Bold Brightens Default Foreground (CGA Convention)

**Author:** Roy  
**Date:** 2025-07-18  
**Status:** Implemented  

## Context

`ResolveForeground()` in `Writer.cs` only applied bold brightening (palette shift 0-7 → 8-15) when `ForegroundMode == Palette`. When `ForegroundMode == Default` (after SGR 0 reset), bold was ignored—returning gray (170,170,170) instead of bright white (255,255,255).

This caused visible rendering errors in b5-ans01.ans and likely any ANSI art that uses `ESC[0m` (reset) followed by `ESC[1m` (bold) without an explicit foreground color.

## Decision

When `ForegroundMode == Default` and `Bold` is true, resolve foreground as `Palette[15]` (bright white) instead of `DefaultForeground`. This follows the CGA convention that the default foreground IS palette index 7, and bold shifts it to palette index 15.

## Impact

- **Writer.cs:** One-line change in `ResolveForeground()` default case
- **Tests:** 3 new tests in `TerminalWriterPhase3Tests.cs` (682 total, all pass)
- **ANSI art compatibility:** Fixes white-vs-gray rendering in any file that relies on bold + default foreground

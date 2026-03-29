---
name: "pending-wrap-resolution"
description: "Pattern for resolving PendingWrap before forward cursor movement in the terminal Writer"
domain: "terminal-writer"
confidence: "high"
source: "audit"
---

## Context
The SadConsole terminal Writer uses a deferred-wrap model (PendingWrap flag) per ECMA-48 §7.1. When the cursor reaches the right margin, the wrap is deferred until the next printable character. Cursor-movement handlers must decide whether to just clear the flag (most handlers) or resolve the wrap first (advance to next line, col 0).

## Pattern: Forward-Movement PendingWrap Resolution

Any handler that moves the cursor **forward along the line** (increasing column) must **resolve** PendingWrap, not just clear it. Without resolution, forward movement from the right margin is clamped to `width-1` → a no-op.

### Detection Rule
A handler is AT RISK if:
1. It moves the cursor forward (increasing column index)
2. The movement is relative to the current position
3. The target is clamped at `width - 1`

### Fix Template
```csharp
case 'X': // Forward-moving handler
    if (State.PendingWrap && State.AutoWrap)
    {
        State.PendingWrap = false;
        State.CursorColumn = 0;
        LineFeed();
    }
    else
    {
        State.PendingWrap = false;
    }
    ApplyForwardMovement(Param(parameters, 0, 1));
    break;
```

### Handlers That Need This
- CUF (CSI C) — forward cursor movement
- CHT (CSI I) — forward tab
- C0 HT (0x09) — tab character

### Handlers That Do NOT Need This
- Absolute positioning (CUP, CHA, VPA, DECSTBM, DECOM) — target is independent of current position
- Backward/vertical relative (CUU, CUD, CUB, CNL, CPL, CBT) — movement from right margin is meaningful, not clamped

## Anti-Patterns
- **Blanket PendingWrap=false in dispatcher epilogue** — this is the opt-out model that caused the original b5-ans01.ans bug. Each handler manages its own PendingWrap clearing.
- **Just clearing PendingWrap for forward movement** — clearing without resolution makes forward movement from the right margin a no-op.

## Examples
See `Writer.cs` CUF handler (case 'C'), CHT handler (case 'I'), and C0 HT in OnC0Control for the canonical implementations.

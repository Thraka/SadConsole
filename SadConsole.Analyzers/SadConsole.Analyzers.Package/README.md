This package contains analyzers and code fixes for SadConsole projects.

## Analyzers

| ID | Description |
|---|---|
| SADCON0001 | Detects `new` assignments to `ColoredGlyphBase.Decorators` |
| SADCON0002 | Detects `null` assignments to `ColoredGlyphBase.Decorators` |
| SADCON0003 | Detects `IsDirty = false` inside `ControlBase.UpdateAndRedraw` overrides |

## Code Fixes

- **SADCON0001**: Replaces `new` decorator assignments with the correct API usage.
- **SADCON0002**: Replaces `null` decorator assignments with the correct API usage.
- **SADCON0003**: Removes the `IsDirty = false` assignment; the renderer handles resetting IsDirty.

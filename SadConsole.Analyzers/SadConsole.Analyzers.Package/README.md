This package contains analyzers and code fixes for SadConsole projects.

## Analyzers

| ID | Description |
|---|---|
| SADCON0001 | Detects `new` assignments to `ColoredGlyphBase.Decorators` |
| SADCON0002 | Detects `null` assignments to `ColoredGlyphBase.Decorators` |

## Code Fixes

- **SADCON0001**: Replaces `new` decorator assignments with the correct API usage.
- **SADCON0002**: Replaces `null` decorator assignments with the correct API usage.

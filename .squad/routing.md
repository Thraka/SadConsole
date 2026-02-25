# Routing Rules

## By Domain

| Domain | Agent |
|--------|-------|
| Architecture, code review, PRD analysis, scope decisions, breaking changes | **Deckard** |
| SadConsole core library, object model, surfaces, entities, animations, string parsing, importers | **Roy** |
| SadConsole.Controls UI system, themes, UI components, control rendering | **Pris** |
| SadConsole.Extended (if UI-adjacent) | **Pris** |
| Host rendering (MonoGame, SFML, FNA, KNI, WPF, Blazor, Skia) | **Gaff** |
| Host shared code (SadConsole.Host.Shared/) | **Gaff** |
| Tests, quality, edge cases, demo/sample code | **Rachael** |
| Documentation accuracy review, spec verification against source code | **Holden** |
| Session logging, decisions merge, cross-agent context | **Scribe** |
| Work queue, GitHub issues, PR tracking | **Ralph** |

## By File Pattern

| Path Pattern | Agent |
|---|---|
| `SadConsole/**` (excluding Controls) | Roy |
| `SadConsole/Controls/**` | Pris |
| `SadConsole.Extended/**` | Pris |
| `SadConsole.Host.MonoGame/**` | Gaff |
| `SadConsole.Host.SFML/**` | Gaff |
| `SadConsole.Host.FNA/**` | Gaff |
| `SadConsole.Host.KNI/**` | Gaff |
| `SadConsole.Host.Shared/**` | Gaff |
| `SadConsole.Host.MonoGameWPF/**` | Gaff |
| `SadConsole.Host.Skia*/**` | Gaff |
| `SadConsole.Debug.MonoGame/**` | Gaff |
| `Tests/**` | Rachael |
| `PerformanceTests/**` | Rachael |
| `templates/**` | Rachael |
| `Samples/**` | Rachael |
| `.squad/**` | Scribe |

## Escalation

- Any change touching both core library and host → Deckard reviews first
- Any cross-host breaking change → Gaff + Deckard
- Any Controls + Core interaction → Pris + Roy

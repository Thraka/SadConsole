# Pris — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### My Domain
I own the Controls and UI system:
- `SadConsole/Controls/` — all interactive controls (buttons, listboxes, textboxes, scrollbars, radio buttons, checkboxes, etc.)
- `SadConsole/UI/` — Windows, panels, ControlsConsole surfaces, themes
- `SadConsole.Extended/` — extended UI components where applicable
- The Controls system is described as "pretty large and highly focused" by Thraka — it's a significant subsystem.

### Controls Architecture Notes
- Controls render on top of SadConsole surfaces
- Theme system controls visual appearance of all controls
- Controls have their own focus/navigation/input handling layer
- Controls interact with core via surface drawing APIs

### Team
- Roy — Core Dev (I coordinate with Roy when controls need core changes)
- Deckard — Lead (consult for any significant UI API changes)
- Rachael — tests my controls work

## Learnings

<!-- Append new entries here as work progresses -->

# SadConsole Squad

## Project Context

- **Project:** SadConsole
- **Description:** C#-based .NET cross-platform terminal/tile-based game engine. Simulates terminal/ASCII programs for modern platforms. The `SadConsole/` folder is the core library. Host libraries (`SadConsole.Host.MonoGame/`, `SadConsole.Host.SFML/`, `SadConsole.Host.FNA/`, `SadConsole.Host.KNI/`, etc.) provide all rendering. `SadConsole.Controls` is a large GUI system built on top of the core. `templates/template_code/SadConsole.Examples.Demo.CSharp/` contains demo/sample code.
- **Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI for rendering hosts
- **Lead developer:** Thraka

## Issue Source

- **Repository:** Thraka/SadConsole
- **Connected:** 2026-02-24
- **Note:** Issues are old — treat as low-confidence backlog. Verify relevance before working.

## Members

| Name | Role | Focus | Emoji |
|------|------|-------|-------|
| Deckard | Lead | Architecture, code review, scope decisions | 🏗️ |
| Roy | Core Dev | SadConsole main library, object model, interfaces to hosts | 🔧 |
| Pris | Controls Dev | SadConsole.Controls UI system (buttons, listboxes, text fields, themes) | 🎨 |
| Gaff | Host Dev | MonoGame/SFML/FNA/KNI rendering host implementations | ⚙️ |
| Rachael | Tester | Tests, quality, demo code validation | 🧪 |
| Holden | Doc Verifier | Fresh-eyes doc/spec accuracy review against source code | 🔍 |
| Scribe | Session Logger | Memory, decisions, session logs | 📋 |
| Ralph | Work Monitor | Work queue, backlog, keep-alive | 🔄 |

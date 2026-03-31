# Roy — Project History

## Core Context

**Project:** SadConsole — C#/.NET cross-platform terminal/tile-based game engine
**Stack:** C# / .NET 8, 9, 10; MonoGame, SFML, FNA, KNI rendering hosts
**Lead developer:** Thraka
**Hired:** 2026-02-24

### My Domain
I own `SadConsole/` — the core library. Key areas:
- Surface types (Console, AnimatedSurface, etc.) and the object model
- Entity system and components
- Animation/instruction system
- String parser (color codes, effects inline in strings)
- Importers: ANSI art, TheDraw fonts, RexPaint, Playscii
- Font/tileset definitions (core side — hosts do the GPU work)
- Host interface contracts — what hosts must implement
- Terminal parser and ANSI/ECMA-48 rendering

### Important: Rendering is in the Hosts
The core library does NOT render. It defines what needs to be rendered and the interfaces/contracts. All GPU/draw calls live in `SadConsole.Host.*/` (Gaff's territory).

### Team
- Deckard — Lead (consult before public API changes)
- Pris — Controls Dev (separate from core, but interacts with core surfaces)
- Gaff — Host Dev (implements my interfaces)
- Rachael — Tester

## Terminal System Architecture Summary

**Terminal parser:** ECMA-48 state machine (Ground/Escape/CSI/DCS/OSC), zero-allocation design, CP437/UTF-8 encoding. **Writer:** Direct cell manipulation, pending-wrap for ANSI art, scroll regions. **State:** Cursor position/attributes (SGR), tab stops. **Palette:** 256-entry RGB, OSC redefinition (X11 formats). **Rendering:** CellDecorator (underline 95, strikethrough 196), color resolution (reverse, dim, bold shifts).

**Phases 0–10 (662 tests):** Parser (87), Writer (160), Insert/Delete (24), Tabs (8), DEC modes (16), Decorators (18), Palette (12), Polish (18).

**PendingWrap architecture:** Opt-in clearing model (cursor-movers clear, SGR/decorators preserve). Root fix for ANSI art drift. CUF/CHT forward-resolution at margins. Bold+default foreground → Palette[15].

**Deckard alignment (2026-03-07):** Architecture design for injectable nullable Cursor, Writer with ITerminalOutput response channel, TerminalConsole subclass pattern. Zero disputes on design direction.

## Implementation History (2026-03-07 to 2026-03-09) — Cursor/Console/Encoder

**Phase 1 (Cursor):** TerminalCursor lightweight data class, CursorShape enum (6 modes), Writer.Cursor nullable injectable. Null-checks added to SyncCursorPosition/DECTCEM/DECSCUSR. 44 tests.

**Phase 2 (TerminalConsole):** Inherits ScreenSurface, wires Writer+TerminalCursor, Feed() delegation, focus toggling, ProcessKeyboard placeholder. GameHost.GetRendererStep() factory pattern. 33 tests.

**Phase 3 (KeyboardEncoder+ITerminalOutput):** KeyboardEncoder standalone encoder (arrows/F1-F12/navigation/modifiers), ITerminalOutput response channel (Write methods). Writer: Output property, DSR/DA1 handlers (null-guarded). TerminalConsole: Output delegation, KeyboardEncoder property, ProcessKeyboard DECCKM sync + encode/feed-back. 92 tests (70 encoder + 22 output). See history-archive.md for detailed patterns.

**Build:** 0 errors, 48 warnings (all pre-existing). **Tests:** 759/759 baseline pass, zero regressions.

## 2026-03-09 — Phase 3 Complete: KeyboardEncoder & ITerminalOutput

Delivered bidirectional input/output infrastructure for interactive terminal support. Validated via SadBBSClient sample (TelnetClient + BbsScreen demonstrating remote terminal integration). See history-archive.md for full details.

## Editor Addin System (2026-03-29)

**Task:** Implement the full addin infrastructure for the SadConsole Editor, as designed by Deckard. Cross-boundary work approved by Deckard.

**Key API findings before writing code:**
- `ImGuiList<T>.Objects` is `ObservableCollection<T>` — use `.Objects.Add()` to append to DocumentBuilders.
- `Core.ImGuiComponent.ImGuiRenderer.UIObjects` is a plain `List<ImGuiObjectBase>` — use `.Add()` directly (confirmed via `ResetUIList()` pattern in `Core.cs`).
- Editor.csproj has `ImplicitUsings=enable` — `System.Linq` (for `GroupBy`) is available without explicit using directive.
- `using SadConsole.Editor.Addins;` added to `GuiTopBar.cs` since it's in a different namespace.

**Files created:**
- `Editor/Addins/IEditorAddin.cs` — interface: Name, Version, Author, Initialize(), GetDocumentBuilders(), GetGuiPanels(), GetMenuItems()
- `Editor/Addins/EditorAddinAttribute.cs` — assembly attribute for fast type discovery, validates IEditorAddin assignability
- `Editor/Addins/AddinMenuItem.cs` — record(Menu, Label, OnClick) for top-bar menu contributions
- `Editor/Addins/AddinLoader.cs` — static loader: scans `addins/` dir, uses `AssemblyLoadContext.Default`, attribute-based discovery, registers builders/panels/menu items into Core state
- `Editor.Addin/ExampleAddin.cs` — demonstrates IEditorAddin implementation with one menu item under "Addins" menu

**Files modified:**
- `Editor/Core.State.cs` — added `public static List<Addins.AddinMenuItem> AddinMenuItems = new();`
- `Editor/GuiObjects/GuiTopBar.cs` — added `using SadConsole.Editor.Addins;`, added GroupBy-based addin menu rendering block after "Grid Guide" menu, before StatusItems loop
- `Editor/Program.cs` — added `using SadConsole.Editor.Addins;`, added `AddinLoader.LoadAndRegisterAddins()` call after `Core.Start()`
- `Editor.Addin/SadConsole.Editor.Addin.csproj` — rewrote to simple project reference to Editor.csproj with `<Private>false</Private>` and `<ExcludeAssets>runtime</ExcludeAssets>`

**Build result:** Both Editor.csproj and SadConsole.Editor.Addin.csproj: 0 errors, warnings all pre-existing.

**Startup flow:** `Core.Start()` → `AddinLoader.LoadAndRegisterAddins()` → scans `{AppContext.BaseDirectory}/addins/*.dll` → loads via `AssemblyLoadContext.Default` → reads `EditorAddinAttribute` → instantiates and calls `Initialize()` → registers builders/panels/menu items.

## Editor Addin Debug Workflow (2026-03-29)

**Task:** Configure Editor.Addin for debug-while-running-Editor in VS/Rider.

**TFM discovery:** Editor.csproj uses `<TargetFramework>net10.0</TargetFramework>` with `<OutputType>WinExe</OutputType>` and no `<RuntimeIdentifier>` — TFM is plain `net10.0` (not `net10.0-windows`).

**What was done:**
- Added `DeployAddinToEditor` MSBuild target (`AfterTargets="Build"`) to `SadConsole.Editor.Addin.csproj`. Copies DLL + PDB into `Editor\bin\$(Configuration)\net10.0\addins\` after each build.
- Created `Editor.Addin\Properties\launchSettings.json` with two profiles ("Editor (Debug)" and "Editor (Release)") using `commandName: "Executable"` pointing to `Editor.exe` in the respective bin output. launchSettings.json uses literal relative paths (no MSBuild variables).
- Build verified: `addins\SadConsole.Editor.Addin.dll` and `.pdb` deployed correctly on first build.

**Key facts:**
- `launchSettings.json` does NOT support MSBuild variables — use `..\\..\\Editor\\bin\\Debug\\net10.0\\Editor.exe` literal paths.
- `commandName: "Executable"` is the VS/Rider mechanism for launching an external process from a class library project.
- The Properties/ directory did not exist in Editor.Addin and had to be created.

## Architecture Decisions — Editor Addin System

### Assembly discovery: attribute-based over full reflection scan
`EditorAddinAttribute` on the assembly points directly to the concrete `IEditorAddin` type. No scanning of all exported types. Validation in the attribute constructor ensures the type implements `IEditorAddin` at load time rather than at `Activator.CreateInstance`.

### `AssemblyLoadContext.Default` for loading
Addins load into the default context so they can access the already-loaded Editor types (Core, Documents, ImGuiSystem) without isolation issues. Isolation-context loading would require assembly copy and interface bridging — not needed for first-party addins.

### `addins/` directory relative to `AppContext.BaseDirectory`
Not relative to `Directory.GetCurrentDirectory()` (which is `Core.State.RootFolder`). The working directory can be set by the user; the binary directory is stable.

### Menu contributions via `List<AddinMenuItem>` on Core.State
Follows the existing `StatusItems` pattern (additive per-frame list). `AddinMenuItems` is NOT cleared each frame — it's a registration list populated once at startup. `GroupBy(m => m.Menu)` in `GuiTopBar.BuildUI()` groups items under shared top-level menus.

### `ExcludeAssets>runtime` in Editor.Addin csproj
Prevents the Editor's transitive runtime dependencies (MonoGame, etc.) from being copied to the addin's output. The addin DLL only needs to contain its own code; the editor's runtime is already present when loading.

# SadConsole: Controls System Architecture

> **Scope:** `SadConsole/UI/` and `SadConsole/UI/Controls/` — the full GUI control library.  
> **Last updated:** 2026 (Deckard)

---

## Overview

SadConsole ships a self-contained GUI control system that sits entirely *above* the core surface and rendering layers. It uses the `IComponent` mechanism to attach to any `IScreenSurface`, so controls are not baked into the surface type hierarchy — they are opt-in.

Key architectural facts:

- Every interactive widget inherits from `ControlBase` (`SadConsole.UI.Controls`).
- A `ControlHost` component (`SadConsole.UI.ControlHost`) manages the control collection, input routing, focus, and tab order for a single surface.
- `ControlsConsole` and `Window` are convenience types — they are `Console` subclasses with a `ControlHost` pre-attached. You can also attach `ControlHost` to *any* `IScreenSurface` manually.
- Rendering is handled by a dedicated `IRenderStep` (`ControlHostRenderStep`, sort order 80) injected into the host surface's renderer pipeline when `ControlHost` is added.
- A `Colors` object (the "theme") carries per-state color values. A `ThemeStates` object maps those colors onto six `ColoredGlyphBase` appearances (Normal, Disabled, MouseOver, MouseDown, Selected, Focused). Each control calls `UpdateAndRedraw` to repaint its own `ICellSurface` every frame it is dirty.

---

## Control Hierarchy

### ControlBase

`SadConsole.UI.Controls.ControlBase` — the abstract root of every control.

```csharp
public abstract class ControlBase
{
    // Geometry
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public Point Position { get; set; }           // cell coordinates, relative to parent
    public Point AbsolutePosition { get; }        // Position + parent chain offset
    public Rectangle Bounds { get; }              // (Position.X, Position.Y, Width, Height)
    public Rectangle MouseArea { get; set; }      // hit-test region (default == Bounds)

    // Surface
    public ICellSurface Surface { get; set; }     // the cells this control paints

    // State
    public ControlStates State { get; protected set; }
    public bool IsEnabled { get; set; }
    public bool IsFocused { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDirty { get; set; }

    // Tabbing / focus
    public bool CanFocus { get; set; }
    public bool TabStop { get; set; }
    public int  TabIndex { get; set; }
    public bool FocusOnMouseClick { get; set; }

    // Theme
    public ThemeStates ThemeState { get; set; }
    public Colors? FindThemeColors();             // walks up: control → host → Colors.Default
    public void    SetThemeColors(Colors? value);

    // Input flags
    public bool UseMouse { get; set; }
    public bool UseKeyboard { get; set; }

    // Container linkage
    public IContainer? Parent { get; set; }

    // Core virtual methods
    public abstract void UpdateAndRedraw(TimeSpan time);
    public virtual  bool ProcessKeyboard(Keyboard state);
    public virtual  bool ProcessMouse(MouseScreenObjectState state);
    public virtual  void DetermineState();

    // Mouse virtual hooks
    protected virtual void OnMouseEnter(ControlMouseState state);
    protected virtual void OnMouseExit(ControlMouseState state);
    protected virtual void OnMouseIn(ControlMouseState state);
    protected virtual void OnLeftMouseClicked(ControlMouseState state);
    protected virtual void OnRightMouseClicked(ControlMouseState state);
}
```

Key design decisions in `ControlBase`:

- **Self-painting:** `UpdateAndRedraw` is abstract. Each control is responsible for writing its visual output into its own `Surface` (an `ICellSurface`). The renderer then blits that surface onto the host texture.
- **Dirty tracking:** `IsDirty` is forwarded to `Surface.IsDirty`. Any state change that should trigger a repaint sets `IsDirty = true`.
- **Focus delegation:** Setting `IsFocused = true` goes through `Parent.Host.FocusedControl`, ensuring only one control holds focus per host at a time.
- **Theme resolution:** `FindThemeColors()` walks: control-level override → host-level `ThemeColors` → static `Colors.Default`. This allows per-control, per-host, or global theming.
- **`PlaceRelativeTo`:** Helper for declarative layout — places this control adjacent to another in a cardinal or intercardinal direction.

### Standard Controls

Full inheritance tree:

```
ControlBase (abstract)
├── ButtonBase (abstract) — text, AutoSize, Click event, Space/Enter keyboard handling
│   ├── Button            — ShowEnds, LeftEndGlyph/RightEndGlyph decorators
│   ├── ButtonBox         — multi-line box-style button
│   ├── SelectionButton   — button variant used inside ListBox items
│   └── ToggleButtonBase (abstract) — adds IsSelected / IsSelectedChanged
│       ├── CheckBox      — toggles IsSelected on click; [ ] / [✓] visuals
│       └── RadioButton   — exclusive selection within a group
├── TextBox               — text input, caret, LeftDrawOffset scroll, Mask, Validator
├── NumberBox             — numeric input (TextBox subtype)
├── Label                 — display-only text; no interaction
├── ProgressBar           — value/max progress display
├── ScrollBar             — horizontal or vertical scrollbar
├── ListBox               — scrollable list with item selection
├── ComboBox              — dropdown combining TextBox + ListBox
├── DrawingArea           — interactive cell canvas
├── SurfaceViewer         — read-only surface viewport
├── TabControl            — tabbed container
├── TextEditor            — multi-line text editing surface
├── ToggleSwitch          — boolean toggle control
└── CompositeControl (abstract) — ControlBase + IContainer; hosts child controls
    ├── Panel             — generic child-control container; TabStop = false by default
    └── TabItem           — single tab page used by TabControl
```

All concrete controls are decorated with `[DataContract]` for serialization support.

### Interfaces

| Interface | Location | Purpose |
|-----------|----------|---------|
| `IContainer` | `UI/Controls/IContainer.cs` | Extended `IList<ControlBase>` with `AbsolutePosition`, `Host`, and named-control lookup |
| `IWindowData` | (Window) | Exposes window-specific state (title, modal, drag) |

`IContainer` is implemented by three types: `ControlHost`, `CompositeControl` (and its subclasses), and `Panel`.

---

## Container System

### ControlHost

`SadConsole.UI.ControlHost` — the core manager component. Implements `Components.IComponent`, `IList<ControlBase>`, and `IContainer`.

```csharp
public class ControlHost : IComponent, IList<ControlBase>, IContainer
{
    public IScreenSurface? ParentConsole { get; }      // the attached surface
    public ControlBase?    FocusedControl { get; set; }
    public ControlBase?    CapturedControl { get; }    // exclusive-mouse capture
    public Colors?         ThemeColors { get; set; }
    public bool            IsDirty { get; set; }

    // Configuration
    public bool ClearOnAdded { get; set; }             // default true
    public bool DisableCursorOnAdded { get; set; }     // default true
    public bool CanTabToNextConsole { get; set; }
    public bool DisableControlFocusing { get; set; }

    // Tab navigation helpers
    public IScreenSurface? NextTabConsole { get; set; }
    public IScreenSurface? PreviousTabConsole { get; set; }

    // Collection mutation
    public void Add(ControlBase control);
    public bool Remove(ControlBase control);
    public void Clear();
    public void Insert(int index, ControlBase control);
    public void ReOrderControls();           // sorts by TabIndex

    // Mouse capture
    public void CaptureControl(ControlBase control);
    public void ReleaseControl();
}
```

When attached to a surface via `IComponent.OnAdded`:

1. Gets a `ControlHostRenderStep` from `GameHost.Instance` and injects it into the surface's renderer pipeline (sort order 80, after surface/entity steps, before tint).
2. If `ClearOnAdded`, fills the surface with theme colors.
3. If `DisableCursorOnAdded` and host is a `Console`, hides the cursor.
4. Subscribes to `ParentConsole.MouseExit`, `.Focused`, `.FocusLost` to maintain control state.

### ControlsConsole / Window

`ControlsConsole` is a thin subclass of `Console` that creates and attaches a `ControlHost` in its constructor:

```csharp
public class ControlsConsole : Console
{
    public ControlHost Controls { get; }

    public ControlsConsole(int width, int height) : base(width, height)
    {
        Controls = new ControlHost();
        SadComponents.Add(Controls);
    }
}
```

`Window` (`UI.WindowConsole.cs`) does the same plus adds:
- Title, border drawing (`DrawBorder()`), and drag support.
- Modal overlay (darkens the background when `IsModal = true`).
- `Show()` / `Hide()` / `ShowModal()` static helper methods.
- `Closed` and `Shown` events.

Both expose their `ControlHost` as `Controls`. Controls are added via:

```csharp
myConsole.Controls.Add(new Button(10) { Text = "OK", Position = (1, 1) });
// or shorthand through the parent property:
new Button(10) { Text = "OK", Position = (1, 1), Parent = myConsole.Controls };
```

### Focus Management

Focus is managed cooperatively between `ControlBase` and `ControlHost`:

1. **Setting focus on a control:** `control.IsFocused = true` → property setter calls `Parent.Host.FocusedControl = this`.
2. `ControlHost.FocusedControl` setter calls `FocusedControlChanging(new, old)` (virtual, overridable) then `FocusedControlChanged(new, old)`.
3. `FocusedControlChanged` clears `IsFocused` on the old control and sets it on the new one. Each control's setter calls `DetermineState()` and raises `Focused`/`Unfocused`.
4. `DisableControlFocusing = true` on the host prevents all focus changes — useful during certain animations.
5. Surface-level focus propagates: when `ParentConsole` gains/loses focus (`Focused`/`FocusLost`), the `FocusedControl.DetermineState()` is called so the visual `Focused` state flag reflects the parent's focus too (`State` requires `ParentConsole.IsFocused` to be true for `ControlStates.Focused` to be set).

### Tab Order

Tab order is insertion-order by default, but is controlled by `ControlBase.TabIndex`:

- `ControlHost.Add(control)` assigns `TabIndex = ControlsList.Count - 1` then calls `ReOrderControls()`.
- `ReOrderControls()` sorts `ControlsList` ascending by `TabIndex`.
- Tab navigation (`Tab` key → `TabNextControl`, `Shift+Tab` → `TabPreviousControl`) walks the sorted list looking for controls where `TabStop && IsEnabled && CanFocus`.
- With `CanTabToNextConsole = true`, Tab wraps from the last control on this host to the first tabbable control on the next sibling surface that also has a `ControlHost`.
- `NextTabConsole` / `PreviousTabConsole` allow explicit cross-surface tab order overrides.
- Mouse capture (`CaptureControl` / `ReleaseControl`) routes all mouse events exclusively to the captured control and pushes the parent surface onto `GameHost.FocusedScreenObjects` to prevent event leakage.

---

## Rendering Pipeline

### How Controls Draw Themselves

Every frame, `ControlHost.Update` (called by `IComponent.Update`) iterates all controls and calls `control.UpdateAndRedraw(delta)`. Each control:

1. Checks `IsDirty`; if not dirty, returns immediately (cheap path).
2. Calls `FindThemeColors()` to resolve the current `Colors` object.
3. Calls `RefreshThemeStateColors(colors)` to rebuild `ThemeState` from those colors.
4. Calls `ThemeState.GetStateAppearance(State)` to select the right `ColoredGlyphBase` for the current flags.
5. Paints its own `Surface` using the standard `ICellSurface` drawing API (`Surface.Fill`, `Surface.Print`, `Surface.SetCellAppearance`, etc.).
6. Marks `IsDirty = false` at the end (the render step clears `control.IsDirty` after it blits).

Example from `Button.UpdateAndRedraw`:

```csharp
public override void UpdateAndRedraw(TimeSpan time)
{
    if (!IsDirty) return;

    Colors colors = FindThemeColors();
    RefreshThemeStateColors(colors);

    ColoredGlyphBase appearance = ThemeState.GetStateAppearance(State);
    ColoredGlyphBase endAppearance = EndsThemeState.GetStateAppearance(State);

    Surface.Fill(appearance.Foreground, appearance.Background, appearance.Glyph, null);

    if (ShowEnds && Width >= 3)
    {
        Surface.Print(1, middle, Text.Align(TextAlignment, Width - 2));
        Surface.SetCellAppearance(0, middle, endAppearance);
        Surface.SetCellAppearance(Width - 1, middle, endAppearance);
        Surface[Width - 1, middle].Glyph = RightEndGlyph;
        Surface[0, middle].Glyph = LeftEndGlyph;
    }
}
```

Controls with `AutoSize` recreate their `Surface` when `EstimateControlSurface()` returns a different size.

### ControlHostRenderStep

`ControlHostRenderStep` (sort order 80) owns the actual GPU blitting. Both the MonoGame and SFML hosts have identical implementations.

**Lifecycle:**

| Phase | Method | What happens |
|-------|--------|--------------|
| `Refresh` | `Refresh(renderer, screenObject, …)` | If `_controlsHost.IsDirty` or texture size changed: sets render target to a private `BackingTexture`, calls `ProcessContainer` to draw all visible controls, then clears `IsDirty`. Returns `true` (forces compositing). |
| `Composing` | `Composing(renderer, screenObject)` | Blits `BackingTexture` over the renderer's backing surface. |
| `Render` | `Render(renderer, screenObject)` | No-op — control visuals are fully composited in the earlier phase. |

`ProcessContainer` is recursive — it handles nested `IContainer` controls (e.g., `Panel` inside `ControlHost`) by recursing. For each visible control it calls `RenderControlCells`, then recurses if the control is also an `IContainer`.

`RenderControlCells` iterates the control's viewport, maps each cell to parent-surface coordinates, culls against the parent view rectangle, then issues two `SpriteBatch.Draw` calls per cell (background quad, then foreground glyph). Decorators get a third draw call each. Controls respect `AlternateFont` — a per-control font override.

### Theme System

The theme system is a three-layer stack:

```
Colors (color palette + pre-built Appearance_* ColoredGlyphBase objects)
  └── ThemeStates (6 per-state ColoredGlyphBase: Normal, Disabled, MouseOver, MouseDown, Selected, Focused)
        └── ControlBase.ThemeState (one ThemeStates instance per control)
```

**`Colors`** (`SadConsole.UI.Colors`):
- Holds named palette colors (`White`, `Black`, `Red`, …, `Gold`, `Silver`, `Bronze`, …).
- Holds `AdjustableColor` fields for every role (`ControlForegroundNormal`, `ControlBackgroundFocused`, `ControlHostBackground`, etc.).
- Holds `Appearance_Control*` `ColoredGlyphBase` properties pre-built from the adjustable colors.
- `RebuildAppearances()` regenerates the `Appearance_*` objects from the current adjustable colors.
- `Colors.Default` is the globally active palette. Changing it dirty-flags all controls.
- Predefined factory: `Colors.CreateAnsi()` (dark ANSI terminal palette).

**`AdjustableColor`** (`SadConsole.UI.AdjustableColor`):
- Wraps a `Color` with optional `Brightness` offset (Brightest / Bright / Normal / Dark / Darkest).
- Can be tied to a named `ColorNames` enum value so it updates when the `Colors` palette is swapped.
- Implicit cast to `Color` returns `ComputedColor` (base + brightness applied).

**`ThemeStates`** (`SadConsole.UI.ThemeStates`):
- Six `ColoredGlyphBase` properties: `Normal`, `Disabled`, `MouseOver`, `MouseDown`, `Selected`, `Focused`.
- `RefreshTheme(Colors colors)` — copies the `Colors.Appearance_Control*` values into the six slots.
- `GetStateAppearance(ControlStates state)` — priority-ordered lookup: Disabled → MouseDown → MouseOver → Focused → Selected → Normal.
- `Clone()` for deep copying.

Controls can have **multiple** `ThemeStates`. For example, `Button` has `ThemeState` (body) plus `EndsThemeState` (the `<` / `>` brackets). `CheckBox` and `RadioButton` have `BracketsThemeState` and `IconThemeState`.

### Control States and Appearance

`ControlStates` is a `[Flags]` enum:

```csharp
[Flags]
public enum ControlStates
{
    Normal             = 0,
    Disabled           = 1 << 0,
    Focused            = 1 << 1,
    Clicked            = 1 << 2,
    MouseOver          = 1 << 3,
    MouseLeftButtonDown  = 1 << 4,
    MouseRightButtonDown = 1 << 5,
    Selected           = 1 << 6
}
```

Multiple flags can be active simultaneously (e.g., `Focused | MouseOver`). `DetermineState()` recomputes the full flag set from the current boolean conditions:

```csharp
public virtual void DetermineState()
{
    State = !_isEnabled
        ? SetFlag(State, Disabled) : UnsetFlag(State, Disabled);
    State = MouseState_IsMouseOver
        ? SetFlag(State, MouseOver) : UnsetFlag(State, MouseOver);
    State = IsFocused && Parent?.Host?.ParentConsole?.IsFocused == true
        ? SetFlag(State, Focused) : UnsetFlag(State, Focused);
    State = MouseState_IsMouseLeftDown && IsMouseButtonStateClean
        ? SetFlag(State, MouseLeftButtonDown) : UnsetFlag(State, MouseLeftButtonDown);
    // … similarly for RightButtonDown

    if (oldState != State)
    {
        OnStateChanged(oldState, State);
        IsDirty = true;  // triggers repaint
    }
}
```

`ThemeStates.GetStateAppearance` uses a **priority order** — not all active flags are checked equally. Disabled wins, then mouse-down, then mouse-over, then focused, then selected, then normal.

---

## Input Handling

### Mouse Events

Mouse input flows: `GameHost` → `IScreenObject.ProcessMouse` → `IComponent.ProcessMouse` (ControlHost) → `ControlBase.ProcessMouse`.

**ControlHost** (`IComponent.ProcessMouse`):
- If a `CapturedControl` exists, routes all mouse events directly to it (exclusive capture).
- Otherwise, iterates `ControlsList` in **reverse order** (top-to-bottom draw order) and calls `control.ProcessMouse(state)` for each visible, direct-child control. Stops at the first control that returns `true`.
- Tracks `_controlWithMouse` — when the hot control changes, calls `LostMouse` on the previous one.
- On `Surface_MouseExit` (parent surface), calls `LostMouse` on all controls.

**ControlBase** (`ProcessMouse`):
```
state → ControlMouseState (wraps position relative to control + IsMouseOver check)
  if IsMouseOver:
    if first time → OnMouseEnter()   → MouseEnter event + DetermineState
    OnMouseIn()                       → MouseMove event + update button-down flags + DetermineState
    if left click  → OnLeftMouseClicked()  → MouseButtonClicked + optionally IsFocused = true
    if right click → OnRightMouseClicked() → MouseButtonClicked
  else if was over → OnMouseExit()   → MouseExit event + clear flags + DetermineState
```

`ControlMouseState` is an inner class on `ControlBase` that holds the original `MouseScreenObjectState` plus `IsMouseOver` (computed as `MouseArea.Contains(cellPosition - AbsolutePosition)`) and `MousePosition` relative to the control's origin.

**`IsMouseButtonStateClean`**: a guard that prevents a "button-down" state from registering if the mouse entered the control *while* the button was already down (drag-enter scenario).

### Keyboard Events

Keyboard input flows: `GameHost` → `IScreenObject.ProcessKeyboard` → `IComponent.ProcessKeyboard` (ControlHost) → `FocusedControl.ProcessKeyboard`.

**ControlHost** (`IComponent.ProcessKeyboard`):
1. If `FocusedControl != null && FocusedControl.IsEnabled && FocusedControl.UseKeyboard` → calls `FocusedControl.ProcessKeyboard(info)`.
2. If the focused control did **not** handle the keystroke:
   - `Shift+Tab` → `TabPreviousControl()`.
   - `Tab` → `TabNextControl()`.
   - (Returns `handled = false` so the surface can still process the key.)

**ControlBase** (`ProcessKeyboard`):
- Default implementation returns `false` (not handled).
- **ButtonBase** override: fires `Click` on `Space` or `Enter` release.
- **TextBox** override: full text-editing logic (character insertion, backspace, delete, Home, End, arrow keys, Ctrl+C/V, etc.).
- **ListBox** override: Up/Down arrows move selection.
- **ScrollBar** override: arrow keys scroll.

### Event Model

All events are standard C# `event EventHandler<TArgs>` patterns:

| Control | Key Events |
|---------|-----------|
| `ControlBase` | `IsDirtyChanged`, `Focused`, `Unfocused`, `PositionChanged`, `MouseEnter`, `MouseExit`, `MouseMove`, `MouseButtonClicked` |
| `ButtonBase` | `Click` |
| `ToggleButtonBase` | `IsSelectedChanged` |
| `TextBox` | `TextChanged`, `TextChangedPreview`, `KeyPressed`, `TextValidated` |
| `ListBox` | `SelectedItemChanged`, `SelectedItemExecuted` |
| `ScrollBar` | `ValueChanged` |
| `Window` | `Closed`, `Shown` |

---

## Extension Points

### Subclassing Controls

To create a custom control:

1. Inherit from `ControlBase` (or a more specific base like `ButtonBase`).
2. Override `UpdateAndRedraw(TimeSpan)` — paint `Surface` based on `State` and `ThemeState`.
3. Optionally override `DetermineState()` to add custom state flags.
4. Optionally override `ProcessKeyboard` / `ProcessMouse` / `OnLeftMouseClicked` etc.
5. Optionally override `RefreshThemeStateColors(Colors)` to apply custom color logic.
6. Optionally override `CreateControlSurface()` to return a differently-sized surface.

### CompositeControl

`CompositeControl` is the base for controls that host other controls internally (e.g., `TabControl` contains `TabItem` children, `ComboBox` contains a `ScrollBar` and a `ListBox`). It implements `IContainer` and forwards `ProcessMouse` / `UpdateAndRedraw` to its child list. Use `AddControl(ControlBase)` / `RemoveControl(ControlBase)` internally; child controls set their `Parent` to the composite.

### Custom ControlHost

Subclass `ControlHost` and override:
- `FocusedControlChanging(newControl, oldControl)` — veto focus changes.
- `FocusedControlChanged(newControl, oldControl)` — hook into focus transitions.
- `CanFocusControl(control)` — custom focusability logic.

### Custom Colors / Themes

```csharp
// 1. Create a new Colors palette
var myTheme = Colors.CreateAnsi();
myTheme.White = Color.AntiqueWhite;
myTheme.RebuildAppearances();

// 2. Apply to a specific host
myConsole.Controls.ThemeColors = myTheme;

// 3. Apply globally
Colors.Default = myTheme;

// 4. Per-control override
myButton.SetThemeColors(myTheme);
```

### SadConsole.Extended Controls

`SadConsole.Extended` adds additional controls that follow the same patterns:

| Control | Description |
|---------|-------------|
| `Table` | Grid/table control with cell theming |
| `ColorPicker` | RGB color picker |
| `ColorBar` | Color gradient selector |
| `HueBar` | Hue-only bar selector |
| `CharacterPicker` | Glyph font picker |
| `FileDirectoryListbox` | File-system list control |

These are additive and do not modify core controls.

---

## Key Patterns

### Dirty-then-Repaint

Controls use a **mark-dirty / repaint-on-demand** pattern rather than immediate repaint. Any state change sets `IsDirty = true` (which propagates to `Surface.IsDirty`). On the next frame, `UpdateAndRedraw` checks `IsDirty` and skips if clean. The render step also checks `ControlHost.IsDirty` before touching the GPU render target.

### Component Injection

`ControlHost` injects its render step into the parent surface's renderer pipeline on `OnAdded`. This means controls work with *any* `IScreenSurface` that has a renderer — no special surface type required. The step is removed cleanly on `OnRemoved`.

### Reverse-Order Mouse Hit Testing

`ControlHost.ProcessMouse` iterates the control list **in reverse** (last-added control is tested first). This gives "topmost" controls (painted last, visually on top) priority for mouse events — matching expected z-order semantics.

### State-Priority Appearance Lookup

`ThemeStates.GetStateAppearance` uses a deterministic priority order rather than "most recently set wins." This means that even if multiple flags are active, the displayed appearance is predictable: **Disabled > MouseDown > MouseOver > Focused > Selected > Normal**.

### Surface-as-Cell-Buffer

Each `ControlBase` owns an `ICellSurface` (`Surface`). It writes character/color data into this buffer in `UpdateAndRedraw`, and the render step reads it to produce pixels. This decouples control logic from rendering entirely — the control never knows it's running on MonoGame vs. SFML.

---

## Key File Paths

| File | Purpose |
|------|---------|
| `SadConsole/UI/Controls/ControlBase.cs` | Abstract root of all controls |
| `SadConsole/UI/Controls/ControlState.cs` | `ControlStates` flags enum |
| `SadConsole/UI/Controls/IContainer.cs` | Container interface |
| `SadConsole/UI/Controls/CompositeControl.cs` | Base for controls-within-controls |
| `SadConsole/UI/Controls/Panel.cs` | Generic child-control panel |
| `SadConsole/UI/Controls/ButtonBase.cs` + `Button.cs` | Button hierarchy |
| `SadConsole/UI/Controls/ToggleButtonBase.cs` | Base for CheckBox/RadioButton |
| `SadConsole/UI/Controls/TextBox.cs` | Text input control |
| `SadConsole/UI/ControlHost.cs` | The IComponent that manages controls |
| `SadConsole/UI/ControlsConsole.cs` | Convenience Console + ControlHost |
| `SadConsole/UI/WindowConsole.cs` | Window (modal, title, drag) |
| `SadConsole/UI/Colors.cs` | Color palette definition |
| `SadConsole/UI/AdjustableColor.cs` | Brightness-aware color reference |
| `SadConsole/UI/ThemeStates.cs` | Per-state appearance container |
| `SadConsole.Host.MonoGame/Renderers/Steps/ControlHostRenderStep.cs` | GPU rendering of controls (MonoGame) |
| `SadConsole.Host.SFML/Renderers/Steps/ControlHostRenderStep.cs` | GPU rendering of controls (SFML) |
| `SadConsole.Extended/UI/Controls/` | Extended control library |

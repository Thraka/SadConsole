using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using static SadConsole.UI.Controls.Table.Cell;

namespace SadConsole.Editor.Documents;

public partial class DocumentControlConsole : Document, IDocumentSimpleObjects, IDocumentZones
{
    public ImGuiList<SimpleObjectDefinition> SimpleObjects { get; } = new();

    public ImGuiList<ZoneSimplified> Zones { get; } = new();

    /// <summary>
    /// Gets the icon for control console documents.
    /// </summary>
    public override string DocumentIcon => "\uf11b"; // nf-fa-gamepad

    public ControlsConsole Console => (ControlsConsole)EditingSurface;

    public ControlBase? SelectedControl { get; set; }

    private ControlBase? _draggingControl;
    private Vector2 _dragOffset;
    private bool _wasDragging;

    private ControlBase? _resizingControl;
    private ResizeHandle _resizingHandle;
    private Rectangle _resizeStartBounds;
    private Vector2 _resizeStartMousePos;

    private bool _isControlMode = true;

    // Control name editing state
    private ControlBase? _lastSelectedControl;
    private string _editingControlName = string.Empty;
    private bool _controlNameConflict;

    private const float HandleSize = 6f;

    private enum ResizeHandle
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    public DocumentControlConsole(int width, int height)
    {
        EditingSurface = new ControlsConsole(width, height);
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        EditingSurface.IsDirty = true;

        Options.ToolsWindowShowToolsList = false;

        Redraw(true, true);
    }

    public DocumentControlConsole(ControlsConsole console)
    {
        EditingSurface = console;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        EditingSurface.IsDirty = true;

        Options.ToolsWindowShowToolsList = false;

        Redraw(true, true);
    }

    public override void ImGuiDrawSurfaceTextureAfter(ImGuiRenderer renderer, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!_isControlMode) return;

        // Draw list for custom rendering
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 startPos = ImGui.GetItemRectMin();

        // Handle mouse input for selection
        HandleControlInteraction(startPos, ImGui.GetMousePos(), isHovered);

        // Handle right-click context menu
        if (isHovered && ImGuiP.IsMouseClicked(ImGuiMouseButton.Right))
        {
            Point viewPos = EditingSurface.Surface.ViewPosition;
            Vector2 viewOffset = new Vector2(viewPos.X * EditorFontSize.X, viewPos.Y * EditorFontSize.Y);
            Vector2 consoleMousePos = (ImGui.GetMousePos() - startPos) + viewOffset;
            Point cellPos = new Point(
                (int)(consoleMousePos.X / EditorFontSize.X),
                (int)(consoleMousePos.Y / EditorFontSize.Y)
            );

            // Find control under mouse for right-click
            for (int i = Console.Controls.Count - 1; i >= 0; i--)
            {
                var control = Console.Controls[i];
                if (control.Bounds.Contains(cellPos))
                {
                    SelectedControl = control;
                    break;
                }
            }

            if (SelectedControl != null)
                ImGui.OpenPopup("control_context_menu"u8);
        }

        // Draw context menu
        if (ImGui.BeginPopup("control_context_menu"u8))
        {
            if (SelectedControl != null)
            {
                ImGui.Text(SelectedControl.Name ?? SelectedControl.GetType().Name);
                ImGui.Separator();

                if (ImGui.MenuItem("Bring to Front"u8))
                    BringToFront(SelectedControl);

                if (ImGui.MenuItem("Bring Forward"u8))
                    BringForward(SelectedControl);

                if (ImGui.MenuItem("Send Backward"u8))
                    SendBackward(SelectedControl);

                if (ImGui.MenuItem("Send to Back"u8))
                    SendToBack(SelectedControl);

                ImGui.Separator();

                if (ImGui.MenuItem("Delete"u8))
                {
                    Console.Controls.Remove(SelectedControl);
                    SelectedControl = null;
                }
            }
            ImGui.EndPopup();
        }

        // Draw selection box
        if (SelectedControl != null)
        {
            Point viewPos = EditingSurface.Surface.ViewPosition;
            Vector2 viewOffset = new Vector2(viewPos.X * EditorFontSize.X, viewPos.Y * EditorFontSize.Y);

            Vector2 min = startPos + new Vector2(
                SelectedControl.Position.X * EditorFontSize.X,
                SelectedControl.Position.Y * EditorFontSize.Y
            ) - viewOffset;

            Vector2 max = min + new Vector2(
                SelectedControl.Width * EditorFontSize.X,
                SelectedControl.Height * EditorFontSize.Y
            );

            drawList.AddRect(min, max, ImGui.GetColorU32(new Vector4(1, 1, 0, 1))); // Yellow box

            DrawSelectionHandles(drawList, min, max);
        }
    }

    private void HandleControlInteraction(Vector2 startPos, Vector2 mousePos, bool isHovered)
    {
        bool leftMouseDown = ImGuiP.IsMouseDown(ImGuiMouseButton.Left);
        bool leftMouseClicked = isHovered && leftMouseDown && !_wasDragging;

        Point viewPos = EditingSurface.Surface.ViewPosition;
        Vector2 viewOffset = new Vector2(viewPos.X * EditorFontSize.X, viewPos.Y * EditorFontSize.Y);
        
        // Mouse position in the console's coordinate space (pixels)
        Vector2 consoleMousePos = (mousePos - startPos) + viewOffset;

        if (_resizingControl != null)
        {
            if (leftMouseDown)
            {
                Vector2 mouseDelta = mousePos - _resizeStartMousePos;
                int cellDeltaX = (int)(mouseDelta.X / EditorFontSize.X);
                int cellDeltaY = (int)(mouseDelta.Y / EditorFontSize.Y);

                Rectangle newBounds = _resizeStartBounds;

                switch (_resizingHandle)
                {
                    case ResizeHandle.TopLeft:
                        newBounds = new Rectangle(_resizeStartBounds.X + cellDeltaX, _resizeStartBounds.Y + cellDeltaY, _resizeStartBounds.Width - cellDeltaX, _resizeStartBounds.Height - cellDeltaY);
                        break;
                    case ResizeHandle.Top:
                        newBounds = new Rectangle(_resizeStartBounds.X, _resizeStartBounds.Y + cellDeltaY, _resizeStartBounds.Width, _resizeStartBounds.Height - cellDeltaY);
                        break;
                    case ResizeHandle.TopRight:
                        newBounds = new Rectangle(_resizeStartBounds.X, _resizeStartBounds.Y + cellDeltaY, _resizeStartBounds.Width + cellDeltaX, _resizeStartBounds.Height - cellDeltaY);
                        break;
                    case ResizeHandle.Right:
                        newBounds = new Rectangle(_resizeStartBounds.X, _resizeStartBounds.Y, _resizeStartBounds.Width + cellDeltaX, _resizeStartBounds.Height);
                        break;
                    case ResizeHandle.BottomRight:
                        newBounds = new Rectangle(_resizeStartBounds.X, _resizeStartBounds.Y, _resizeStartBounds.Width + cellDeltaX, _resizeStartBounds.Height + cellDeltaY);
                        break;
                    case ResizeHandle.Bottom:
                        newBounds = new Rectangle(_resizeStartBounds.X, _resizeStartBounds.Y, _resizeStartBounds.Width, _resizeStartBounds.Height + cellDeltaY);
                        break;
                    case ResizeHandle.BottomLeft:
                        newBounds = new Rectangle(_resizeStartBounds.X + cellDeltaX, _resizeStartBounds.Y, _resizeStartBounds.Width - cellDeltaX, _resizeStartBounds.Height + cellDeltaY);
                        break;
                    case ResizeHandle.Left:
                        newBounds = new Rectangle(_resizeStartBounds.X + cellDeltaX, _resizeStartBounds.Y, _resizeStartBounds.Width - cellDeltaX, _resizeStartBounds.Height);
                        break;
                }

                if (newBounds.Width < 1) newBounds = new Rectangle(newBounds.X, newBounds.Y, 1, newBounds.Height);
                if (newBounds.Height < 1) newBounds = new Rectangle(newBounds.X, newBounds.Y, newBounds.Width, 1);

                if (_resizingControl.Position != newBounds.Position)
                    _resizingControl.Position = newBounds.Position;
                
                if (_resizingControl.Width != newBounds.Width || _resizingControl.Height != newBounds.Height)
                    _resizingControl.Resize(newBounds.Width, newBounds.Height);
            }
            else
            {
                _resizingControl = null;
                _resizingHandle = ResizeHandle.None;
            }
        }
        else if (_draggingControl != null)
        {
            if (leftMouseDown)
            {
                Vector2 newPixelPos = consoleMousePos - _dragOffset;
                int cellX = (int)(newPixelPos.X / EditorFontSize.X);
                int cellY = (int)(newPixelPos.Y / EditorFontSize.Y);
                
                _draggingControl.Position = new Point(cellX, cellY);
                _draggingControl.IsDirty = true;
            }
            else
            {
                _draggingControl = null;
            }
        }
        else if (leftMouseClicked)
        {
            // Check handles first
            if (SelectedControl != null)
            {
                Vector2 min = startPos + new Vector2(
                    SelectedControl.Position.X * EditorFontSize.X,
                    SelectedControl.Position.Y * EditorFontSize.Y
                ) - viewOffset;
                
                Vector2 max = min + new Vector2(
                    SelectedControl.Width * EditorFontSize.X,
                    SelectedControl.Height * EditorFontSize.Y
                );

                ResizeHandle handle = GetHandleAt(mousePos, min, max);
                if (handle != ResizeHandle.None)
                {
                    _resizingControl = SelectedControl;
                    _resizingHandle = handle;
                    _resizeStartBounds = new Rectangle(SelectedControl.Position.X, SelectedControl.Position.Y, SelectedControl.Width, SelectedControl.Height);
                    _resizeStartMousePos = mousePos;
                    _wasDragging = true;
                    return;
                }
            }

            // Check controls
            Point cellPos = new Point(
                (int)(consoleMousePos.X / EditorFontSize.X),
                (int)(consoleMousePos.Y / EditorFontSize.Y)
            );

            ControlBase? clickedControl = null;
            // Iterate in reverse to hit top-most first
            for (int i = Console.Controls.Count - 1; i >= 0; i--)
            {
                var control = Console.Controls[i];
                if (control.Bounds.Contains(cellPos))
                {
                    clickedControl = control;
                    break;
                }
            }

            SelectedControl = clickedControl;

            if (SelectedControl != null)
            {
                _draggingControl = SelectedControl;
                _dragOffset = consoleMousePos - new Vector2(SelectedControl.Position.X * EditorFontSize.X, SelectedControl.Position.Y * EditorFontSize.Y);
            }
        }

        _wasDragging = leftMouseDown;
    }

    private void DrawSelectionHandles(ImDrawListPtr drawList, Vector2 min, Vector2 max)
    {
        uint handleColor = ImGui.GetColorU32(new Vector4(1, 1, 1, 1));
        float halfHandle = HandleSize / 2;

        void DrawHandle(Vector2 center)
        {
            drawList.AddRectFilled(center - new Vector2(halfHandle), center + new Vector2(halfHandle), handleColor);
            drawList.AddRect(center - new Vector2(halfHandle), center + new Vector2(halfHandle), 0xFF000000);
        }

        DrawHandle(min); // TopLeft
        DrawHandle(new Vector2((min.X + max.X) / 2, min.Y)); // Top
        DrawHandle(new Vector2(max.X, min.Y)); // TopRight
        DrawHandle(new Vector2(max.X, (min.Y + max.Y) / 2)); // Right
        DrawHandle(max); // BottomRight
        DrawHandle(new Vector2((min.X + max.X) / 2, max.Y)); // Bottom
        DrawHandle(new Vector2(min.X, max.Y)); // BottomLeft
        DrawHandle(new Vector2(min.X, (min.Y + max.Y) / 2)); // Left
    }

    private ResizeHandle GetHandleAt(Vector2 mousePos, Vector2 min, Vector2 max)
    {
        float halfHandle = HandleSize / 2;
        bool IsOver(Vector2 center)
        {
            return mousePos.X >= center.X - halfHandle && mousePos.X <= center.X + halfHandle &&
                   mousePos.Y >= center.Y - halfHandle && mousePos.Y <= center.Y + halfHandle;
        }

        if (IsOver(min)) return ResizeHandle.TopLeft;
        if (IsOver(new Vector2((min.X + max.X) / 2, min.Y))) return ResizeHandle.Top;
        if (IsOver(new Vector2(max.X, min.Y))) return ResizeHandle.TopRight;
        if (IsOver(new Vector2(max.X, (min.Y + max.Y) / 2))) return ResizeHandle.Right;
        if (IsOver(max)) return ResizeHandle.BottomRight;
        if (IsOver(new Vector2((min.X + max.X) / 2, max.Y))) return ResizeHandle.Bottom;
        if (IsOver(new Vector2(min.X, max.Y))) return ResizeHandle.BottomLeft;
        if (IsOver(new Vector2(min.X, (min.Y + max.Y) / 2))) return ResizeHandle.Left;

        return ResizeHandle.None;
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        float width = ImGui.GetContentRegionAvail().X;
        if (ImGui.Button(_isControlMode ? "Switch to Drawing" : "Switch to Controls", new Vector2(width, 0)))
        {
            _isControlMode = !_isControlMode;
            Options.ToolsWindowShowToolsList = !_isControlMode;

            if (_isControlMode)
            {
                if (Core.State.Tools.IsItemSelected())
                {
                    Core.State.Tools.SelectedItem.OnDeselected(this);
                    Core.State.Tools.SelectedItemIndex = -1;
                }
            }
            else
            {
                if (!Core.State.Tools.IsItemSelected() && Core.State.Tools.Count > 0)
                {
                    Core.State.Tools.SelectedItemIndex = 0;
                    Core.State.Tools.SelectedItem.OnSelected(this);
                }
            }
        }

        BuildSurfaceSettings(renderer);
    }

    public override void ImGuiDrawInToolsPanel(ImGuiRenderer renderer)
    {
        if (_isControlMode)
        {
            BuildControlsSettings(renderer);
        }
    }

    private void BuildSurfaceSettings(ImGuiRenderer renderer)
    {
        ImGui.SeparatorText("Surface Settings"u8);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Name:"u8);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);

        string editTitle = Title;
        if (ImGui.InputText("##name"u8, ref editTitle, 50))
            Title = editTitle;

        ImGui.Separator();

        ImGui.BeginGroup();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Width: "u8);
        ImGui.SameLine();
        ImGui.Text(EditingSurface.Surface.Width.ToString());
        ImGui.Text("Height:"u8);
        ImGui.SameLine();
        ImGui.Text(EditingSurface.Surface.Height.ToString());
        ImGui.EndGroup();
        ImGui.SameLine(0, ImGui.GetFontSize());

        if (EditingSurface is ICellSurfaceResize)
        {
            if (ImGui.Button("Resize"u8))
            {
                _width = new(EditingSurface.Surface.Width);
                _height = new(EditingSurface.Surface.Height);
                ImGui.OpenPopup("resize_document");
            }

            if (ResizeSurfacePopup.Show("resize_document", ref _width.CurrentValue, ref _height.CurrentValue, out bool dialogResult))
            {
                if (dialogResult && (_width.IsChanged() || _height.IsChanged()))
                {
                    int viewWidth = Math.Min(EditingSurface.Surface.ViewWidth, _width.CurrentValue);
                    int viewHeight = Math.Min(EditingSurface.Surface.ViewHeight, _height.CurrentValue);

                    ((ICellSurfaceResize)EditingSurface).Resize(viewWidth, viewHeight, _width.CurrentValue, _height.CurrentValue, false);

                    _width = new(EditingSurface.Surface.Width);
                    _height = new(EditingSurface.Surface.Height);

                    EditingSurface.IsDirty = true;
                    if (Core.State.Tools.IsItemSelected())
                        Core.State.Tools.SelectedItem.Reset(this);
                }
            }
        }

        ImGui.Separator();

        var DefaultForeground = EditingSurface.Surface.DefaultForeground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Foreground: "u8);
        
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs))
            EditingSurface.Surface.DefaultForeground = DefaultForeground.ToColor();


        var DefaultBackground = EditingSurface.Surface.DefaultBackground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Background: "u8);
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs))
            EditingSurface.Surface.DefaultBackground = DefaultBackground.ToColor();

        ImGui.Separator();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font: "u8);
        ImGui.SameLine();
        if (ImGui.Button($"{EditingSurfaceFont.Name} | {EditingSurfaceFontSize}"))
            base.FontSelectionWindow_Popup();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Editor Font Size: "u8);
        ImGui.SameLine();
        if (ImGui.Button(EditorFontSize.ToString()))
        {
            ImGui.OpenPopup("editorfontsize_select");
        }
        ImGui.SameLine();
        if (ImGui.Button("Reset"u8))
        {
            EditorFontSize = EditingSurfaceFontSize;
        }

        if (base.FontSelectionWindow_BuildUI(renderer))
        {
            EditingSurfaceFont = (SadFont)FontSelectionWindow.SelectedFont;
            EditingSurfaceFontSize = FontSelectionWindow.SelectedFontSize;
            EditorFontSize = FontSelectionWindow.SelectedFontSize;
            EditingSurface.Font = EditingSurfaceFont;
            EditingSurface.FontSize = EditorFontSize;
            EditingSurface.IsDirty = true;
            VisualTool.Font = EditingSurfaceFont;
            VisualTool.IsDirty = true;
            base.FontSelectionWindow_Reset();
        }

        if (FontSizePopup.Show("editorfontsize_select", EditingSurfaceFont, ref EditorFontSize))
        {
            EditingSurface.IsDirty = true;
            VisualTool.IsDirty = true;
        }
    }

    private void BuildControlsSettings(ImGuiRenderer renderer)
    {
        if (ImGui.BeginTabBar("controls_tabbar"u8, ImGuiTabBarFlags.None))
        {
            if (ImGui.BeginTabItem("Toolbox"u8))
            {
                BuildToolboxPanel(renderer);
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Properties"u8))
            {
                BuildPropertiesPanel(renderer);
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    private void BuildToolboxPanel(ImGuiRenderer renderer)
    {
        ImGui.Text("Double-click to add:"u8);
        ImGui.Separator();

        DrawToolboxItem("Button"u8, () => new Button(10, 1) { Text = "Button" });
        DrawToolboxItem("CheckBox"u8, () => new CheckBox(15, 1) { Text = "CheckBox" });
        DrawToolboxItem("RadioButton"u8, () => new RadioButton(15, 1) { Text = "RadioButton" });
        DrawToolboxItem("TextBox"u8, () => new TextBox(10));
        DrawToolboxItem("Label"u8, () => new Label("Label"));
        DrawToolboxItem("ProgressBar"u8, () => new ProgressBar(10, 1, HorizontalAlignment.Left));
        DrawToolboxItem("ToggleSwitch"u8, () => new ToggleSwitch(5, 1));
        DrawToolboxItem("SelectionButton"u8, () => new SelectionButton(15, 1));
        DrawToolboxItem("ListBox"u8, () => new ListBox(15, 5));
        DrawToolboxItem("Panel"u8, () => new Panel(10, 5));
        DrawToolboxItem("ScrollBar (H)"u8, () => new ScrollBar(Orientation.Horizontal, 10, 10));
        DrawToolboxItem("ScrollBar (V)"u8, () => new ScrollBar(Orientation.Vertical, 10, 10));
    }

    private void DrawToolboxItem(ReadOnlySpan<byte> name, Func<ControlBase> createControl)
    {
        ImGui.Selectable(name);

        // Double-click to add at default position
        if (ImGui.IsItemHovered() && ImGuiP.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            ControlBase control = createControl();
            control.Position = new Point(1, 1);
            AddControl(control);
            SelectedControl = control;
        }
    }

    private void BuildPropertiesPanel(ImGuiRenderer renderer)
    {
        // Control list for selection
        ImGui.Text("Select Control:"u8);
        string currentControlName = SelectedControl != null 
            ? (string.IsNullOrEmpty(SelectedControl.Name) ? $"{SelectedControl.GetType().Name}#{SelectedControl.GetHashCode()}" : SelectedControl.Name)
            : "<none>";
        
        if (ImGui.BeginCombo("##selectcontrol"u8, currentControlName))
        {
            for (int i = 0; i < Console.Controls.Count; i++)
            {
                var control = Console.Controls[i];
                string name = string.IsNullOrEmpty(control.Name) ? $"{control.GetType().Name}#{control.GetHashCode()}" : control.Name;
                bool isSelected = SelectedControl == control;
                
                if (ImGui.Selectable($"{name}##{i}", isSelected))
                {
                    SelectedControl = control;
                    
                    // Load the new control's name for editing
                    _editingControlName = control.Name ?? "";
                    _lastSelectedControl = control;
                    _controlNameConflict = false;
                }
                
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        ImGui.Separator();

        if (SelectedControl == null)
        {
            ImGui.TextDisabled("No control selected"u8);
            return;
        }

        // Initialize or update editing state when selection changes
        if (_lastSelectedControl != SelectedControl)
        {
            _editingControlName = SelectedControl.Name ?? "";
            _lastSelectedControl = SelectedControl;
            _controlNameConflict = false;
        }

        ImGui.Text($"Type: {SelectedControl.GetType().Name}");
        ImGui.Separator();

        // Name editing with validation
        ImGui.Text("Name:"u8);
        ImGui.SetNextItemWidth(-1);
        if (ImGui.InputText("##control_name", ref _editingControlName, 256))
        {
            // Check for duplicate name on each change
            _controlNameConflict = IsControlNameDuplicate(_editingControlName, SelectedControl);
        }

        if (_controlNameConflict)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f));
            ImGui.TextWrapped("Name already exists. Please use a unique name.");
            ImGui.PopStyleColor();
        }
        else if (_editingControlName != (SelectedControl.Name ?? ""))
        {
            // Name is different and valid - show apply button
            if (ImGui.Button("Apply Name"u8))
            {
                ApplyControlName(SelectedControl, _editingControlName);
            }
            ImGui.SameLine();
            ImGui.TextDisabled("(recreates control)"u8);
        }

        ImGui.Separator();

        // Common properties
        if (SettingsTable.BeginTable("control_props"))
        {
            // Position
            int x = SelectedControl.Position.X;
            int y = SelectedControl.Position.Y;
            bool posChanged = false;

            if (SettingsTable.DrawInt("X", "##control.x", ref x, -1000, 1000)) posChanged = true;
            if (SettingsTable.DrawInt("Y", "##control.y", ref y, -1000, 1000)) posChanged = true;

            if (posChanged)
            {
                SelectedControl.Position = new Point(x, y);
                SelectedControl.IsDirty = true;
            }

            // Size
            int width = SelectedControl.Width;
            int height = SelectedControl.Height;
            bool sizeChanged = false;

            if (SettingsTable.DrawInt("Width", "##control.width", ref width, 1, 1000)) sizeChanged = true;
            if (SettingsTable.DrawInt("Height", "##control.height", ref height, 1, 1000)) sizeChanged = true;

            if (sizeChanged && SelectedControl.CanResize)
                SelectedControl.Resize(width, height);

            // Common ControlBase properties
            bool isVisible = SelectedControl.IsVisible;
            if (SettingsTable.DrawCheckbox("IsVisible", "##control.visible", ref isVisible))
                SelectedControl.IsVisible = isVisible;

            bool isEnabled = SelectedControl.IsEnabled;
            if (SettingsTable.DrawCheckbox("IsEnabled", "##control.enabled", ref isEnabled))
                SelectedControl.IsEnabled = isEnabled;

            bool tabStop = SelectedControl.TabStop;
            if (SettingsTable.DrawCheckbox("TabStop", "##control.tabstop", ref tabStop))
                SelectedControl.TabStop = tabStop;

            int tabIndex = SelectedControl.TabIndex;
            if (SettingsTable.DrawInt("TabIndex", "##control.tabindex", ref tabIndex, 0, 1000))
                SelectedControl.TabIndex = tabIndex;

            bool useMouse = SelectedControl.UseMouse;
            if (SettingsTable.DrawCheckbox("UseMouse", "##control.usemouse", ref useMouse))
                SelectedControl.UseMouse = useMouse;

            bool useKeyboard = SelectedControl.UseKeyboard;
            if (SettingsTable.DrawCheckbox("UseKeyboard", "##control.usekeyboard", ref useKeyboard))
                SelectedControl.UseKeyboard = useKeyboard;

            bool canFocus = SelectedControl.CanFocus;
            if (SettingsTable.DrawCheckbox("CanFocus", "##control.canfocus", ref canFocus))
                SelectedControl.CanFocus = canFocus;

            SettingsTable.EndTable();
        }

        // Type-specific properties
        ImGui.Separator();
        BuildTypeSpecificProperties(renderer);

        // Delete button
        ImGui.Separator();
        if (ImGui.Button("Delete Control"u8, new Vector2(-1, 0)))
        {
            Console.Controls.Remove(SelectedControl);
            SelectedControl = null;
            _lastSelectedControl = null;
            _editingControlName = "";
        }
    }

    /// <summary>
    /// Applies a new name to a control by recreating it with the new name.
    /// </summary>
    private void ApplyControlName(ControlBase oldControl, string newName)
    {
        // Get the index of the old control to maintain z-order
        int index = Console.Controls.IndexOf(oldControl);
        if (index < 0) return;

        // Create a new control of the same type with the new name
        ControlBase? newControl = RecreateControlWithName(oldControl, newName);
        if (newControl == null) return;

        // Remove old and insert new at same position
        Console.Controls.Remove(oldControl);
        Console.Controls.Insert(index, newControl);

        // Update selection
        SelectedControl = newControl;
        _lastSelectedControl = newControl;
        _editingControlName = newName;

        Console.Controls.IsDirty = true;
    }

    /// <summary>
    /// Recreates a control with a new name, copying all properties from the original.
    /// </summary>
    private ControlBase? RecreateControlWithName(ControlBase original, string newName)
    {
        // Note: Order matters - more specific types must come before base types (e.g., SelectionButton before Button)
        ControlBase? newControl = original switch
        {
            SelectionButton selBtn => new SelectionButton(selBtn.Width, selBtn.Height) { Name = newName, Text = selBtn.Text },
            RadioButton rad => new RadioButton(rad.Width, rad.Height) { Name = newName, Text = rad.Text, IsSelected = rad.IsSelected },
            CheckBox chk => new CheckBox(chk.Width, chk.Height) { Name = newName, Text = chk.Text, IsSelected = chk.IsSelected },
            Button btn => new Button(btn.Width, btn.Height) { Name = newName, Text = btn.Text, ShowEnds = btn.ShowEnds },
            TextBox txt => new TextBox(txt.Width) { Name = newName, Text = txt.Text, MaxLength = txt.MaxLength },
            Label lbl => new Label(lbl.Width) { Name = newName, DisplayText = lbl.DisplayText, ShowUnderline = lbl.ShowUnderline, ShowStrikethrough = lbl.ShowStrikethrough },
            ProgressBar prog => new ProgressBar(prog.Width, prog.Height, prog.HorizontalAlignment) { Name = newName, Progress = prog.Progress },
            ToggleSwitch toggle => new ToggleSwitch(toggle.Width, toggle.Height) { Name = newName, IsSelected = toggle.IsSelected },
            ListBox listBox => new ListBox(listBox.Width, listBox.Height) { Name = newName },
            Panel panel => new Panel(panel.Width, panel.Height) { Name = newName },
            ScrollBar scrollBar => new ScrollBar(scrollBar.Orientation, scrollBar.Width, scrollBar.Height) { Name = newName, MaximumValue = scrollBar.MaximumValue, Value = scrollBar.Value },
            _ => null
        };

        if (newControl == null) return null;

        // Copy common properties
        newControl.Position = original.Position;
        newControl.IsVisible = original.IsVisible;
        newControl.IsEnabled = original.IsEnabled;
        newControl.TabStop = original.TabStop;
        newControl.TabIndex = original.TabIndex;
        newControl.UseMouse = original.UseMouse;
        newControl.UseKeyboard = original.UseKeyboard;
        newControl.CanFocus = original.CanFocus;

        return newControl;
    }

    /// <summary>
    /// Checks if the specified control name already exists in another control.
    /// </summary>
    private bool IsControlNameDuplicate(string name, ControlBase excludeControl)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        foreach (var control in Console.Controls)
        {
            if (control != excludeControl && string.Equals(control.Name, name, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private void BuildTypeSpecificProperties(ImGuiRenderer renderer)
    {
        if (SelectedControl is Button btn)
        {
            string text = btn.Text ?? "";
            if (ImGui.InputText("Text"u8, ref text, 100)) btn.Text = text;
            
            bool showEnds = btn.ShowEnds;
            if (ImGui.Checkbox("ShowEnds"u8, ref showEnds)) btn.ShowEnds = showEnds;
        }
        else if (SelectedControl is CheckBox chk)
        {
            string text = chk.Text ?? "";
            if (ImGui.InputText("Text"u8, ref text, 100)) chk.Text = text;
            
            bool isSelected = chk.IsSelected;
            if (ImGui.Checkbox("IsSelected"u8, ref isSelected)) chk.IsSelected = isSelected;
        }
        else if (SelectedControl is RadioButton rad)
        {
            string text = rad.Text ?? "";
            if (ImGui.InputText("Text"u8, ref text, 100)) rad.Text = text;
            
            bool isSelected = rad.IsSelected;
            if (ImGui.Checkbox("IsSelected"u8, ref isSelected)) rad.IsSelected = isSelected;
        }
        else if (SelectedControl is Label lbl)
        {
            string text = lbl.DisplayText ?? "";
            if (ImGui.InputText("Text"u8, ref text, 100)) lbl.DisplayText = text;

            bool showUnderline = lbl.ShowUnderline;
            if (ImGui.Checkbox("ShowUnderline"u8, ref showUnderline)) lbl.ShowUnderline = showUnderline;

            bool showStrikethrough = lbl.ShowStrikethrough;
            if (ImGui.Checkbox("ShowStrikethrough"u8, ref showStrikethrough)) lbl.ShowStrikethrough = showStrikethrough;
        }
        else if (SelectedControl is TextBox txt)
        {
            string text = txt.Text ?? "";
            if (ImGui.InputText("Text"u8, ref text, 500)) txt.Text = text;

            int maxLength = txt.MaxLength;
            if (ImGui.InputInt("MaxLength"u8, ref maxLength))
            {
                if (maxLength > 0) txt.MaxLength = maxLength;
            }
        }
        else if (SelectedControl is ProgressBar prog)
        {
            float progress = prog.Progress;
            if (ImGui.SliderFloat("Progress"u8, ref progress, 0f, 1f))
                prog.Progress = progress;
        }
        else if (SelectedControl is ToggleSwitch toggle)
        {
            bool isSelected = toggle.IsSelected;
            if (ImGui.Checkbox("IsSelected"u8, ref isSelected)) toggle.IsSelected = isSelected;
        }
        else if (SelectedControl is SelectionButton selBtn)
        {
            string text = selBtn.Text ?? "";
            if (ImGui.InputText("Text"u8, ref text, 100)) selBtn.Text = text;
        }
    }

    private void AddControl(ControlBase control)
    {
        Console.Controls.Add(control);
        Redraw(true, true);
    }

    public override void ImGuiDrawTopBar(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMenu("Document Options"u8))
        {
            bool optionChanged = false;

            ImGui.PushItemFlag(ImGuiItemFlags.AutoClosePopups, false);

            bool enableSimpleObjs = Options.UseSimpleObjects;
            optionChanged |= ImGui.MenuItem("Enable Simple Objs", "", ref enableSimpleObjs);
            Options.UseSimpleObjects = enableSimpleObjs;

            bool enableZones = Options.UseZones;
            optionChanged |= ImGui.MenuItem("Enable Zones", "", ref enableZones);
            Options.UseZones = enableZones;

            if (optionChanged)
                SyncToolModes();

            ImGui.PopItemFlag();

            if (enableSimpleObjs || enableZones)
            {
                if (enableSimpleObjs)
                {
                    ImGui.SeparatorText("Simple Objects");

                    ImGui.PushID("objects_menu");

                    if (ImGui.MenuItem("Manage"u8))
                        new Windows.SimpleObjectEditor(SimpleObjects, EditingSurface.Surface.DefaultForeground.ToVector4(), EditingSurface.Surface.DefaultBackground.ToVector4(), EditingSurfaceFont).Open();

                    bool doImport = false;
                    bool replace = false;

                    if (ImGui.MenuItem("Import (add)"u8))
                    {
                        doImport = true;
                        replace = false;
                    }

                    if (ImGui.MenuItem("Import (replace)"u8))
                    {
                        doImport = true;
                        replace = true;
                    }

                    if (doImport)
                    {
                        Windows.OpenFile window = new([new SimpleObjectsHandler()]);

                        window.Closed += (s, e) =>
                        {
                            if (window.DialogResult)
                            {
                                if (window.SelectedLoader.Load(window.SelectedFile.FullName) is SimpleObjectDefinition[] objects)
                                {
                                    if (replace)
                                        SimpleObjects.Objects.Clear();

                                    foreach (SimpleObjectDefinition obj in objects)
                                    {
                                        if (!SimpleObjects.Objects.Where(o => o.Name.Equals(obj.Name, StringComparison.OrdinalIgnoreCase)).Any())
                                            SimpleObjects.Objects.Add(obj);
                                    }
                                }
                            }
                        };
                        window.Open();
                    }

                    if (ImGui.MenuItem("Export"u8))
                        new SaveFile(SimpleObjects.Objects.ToArray(), [new SimpleObjectsHandler()]).Open();

                    ImGui.PopID();
                }

                if (enableZones)
                {
                    ImGui.SeparatorText("Zones");

                    bool doImport = false;
                    bool replace = false;

                    ImGui.PushID("zones_menu");

                    if (ImGui.MenuItem("Import (add)"u8))
                    {
                        doImport = true;
                        replace = false;
                    }

                    if (ImGui.MenuItem("Import (replace)"u8))
                    {
                        doImport = true;
                        replace = true;
                    }

                    if (doImport)
                    {
                        Windows.OpenFile window = new([new ZonesHandler()]);

                        window.Closed += (s, e) =>
                        {
                            if (window.DialogResult)
                            {
                                if (window.SelectedLoader.Load(window.SelectedFile.FullName) is ZoneSimplified[] zones)
                                {
                                    if (replace)
                                        Zones.Objects.Clear();

                                    foreach (ZoneSimplified zone in zones)
                                    {
                                        if (!Zones.Objects.Where(z => z.Name.Equals(zone.Name, StringComparison.OrdinalIgnoreCase)).Any())
                                            Zones.Objects.Add(zone);
                                    }

                                    Core.State.Tools.SelectedItem?.DocumentViewChanged(this);
                                }
                            }
                        };
                        window.Open();
                    }

                    if (ImGui.MenuItem("Export"u8))
                        new SaveFile(Zones.Objects.ToArray(), [new ZonesHandler()]).Open();

                    ImGui.PopID();
                }
            }

            ImGui.EndMenu();
        }
    }

    public override void SetSurfaceView(int x, int y, int width, int height)
    {
        base.SetSurfaceView(x, y, width, height);

        Console.Controls.IsDirty = true;
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        []; // Placeholder

    private void BringToFront(ControlBase control)
    {
        // Find the highest TabIndex among all controls
        int maxTabIndex = 0;
        foreach (var c in Console.Controls)
        {
            if (c.TabIndex > maxTabIndex)
                maxTabIndex = c.TabIndex;
        }

        // Set this control's TabIndex higher than all others
        control.TabIndex = maxTabIndex + 1;
        Console.Controls.ReOrderControls();
        Console.Controls.IsDirty = true;
    }

    private void SendToBack(ControlBase control)
    {
        // Find the lowest TabIndex among all controls
        int minTabIndex = int.MaxValue;
        foreach (var c in Console.Controls)
        {
            if (c.TabIndex < minTabIndex)
                minTabIndex = c.TabIndex;
        }

        // Set this control's TabIndex lower than all others
        control.TabIndex = minTabIndex > 0 ? minTabIndex - 1 : 0;

        // If we hit 0, normalize all TabIndex values
        if (control.TabIndex == 0)
            NormalizeTabIndexes();

        Console.Controls.ReOrderControls();
        Console.Controls.IsDirty = true;
    }

    private void BringForward(ControlBase control)
    {
        // Find the control with the next higher TabIndex
        int currentTabIndex = control.TabIndex;
        ControlBase? nextControl = null;
        int nextTabIndex = int.MaxValue;

        foreach (var c in Console.Controls)
        {
            if (c != control && c.TabIndex > currentTabIndex && c.TabIndex < nextTabIndex)
            {
                nextControl = c;
                nextTabIndex = c.TabIndex;
            }
        }

        if (nextControl != null)
        {
            // Swap TabIndex values
            control.TabIndex = nextTabIndex;
            nextControl.TabIndex = currentTabIndex;
            Console.Controls.ReOrderControls();
            Console.Controls.IsDirty = true;
        }
    }

    private void SendBackward(ControlBase control)
    {
        // Find the control with the next lower TabIndex
        int currentTabIndex = control.TabIndex;
        ControlBase? prevControl = null;
        int prevTabIndex = -1;

        foreach (var c in Console.Controls)
        {
            if (c != control && c.TabIndex < currentTabIndex && c.TabIndex > prevTabIndex)
            {
                prevControl = c;
                prevTabIndex = c.TabIndex;
            }
        }

        if (prevControl != null)
        {
            // Swap TabIndex values
            control.TabIndex = prevTabIndex;
            prevControl.TabIndex = currentTabIndex;
            Console.Controls.ReOrderControls();
            Console.Controls.IsDirty = true;
        }
    }

    private void NormalizeTabIndexes()
    {
        // Sort controls by current TabIndex and reassign sequential values
        var sortedControls = Console.Controls.OrderBy(c => c.TabIndex).ToList();
        for (int i = 0; i < sortedControls.Count; i++)
        {
            sortedControls[i].TabIndex = i;
        }
        Console.Controls.ReOrderControls();
    }
}

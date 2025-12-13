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
        ImGui.SeparatorText("Controls"u8);
        
        if (ImGui.Button("Add Control"u8))
        {
            ImGui.OpenPopup("add_control_popup"u8);
        }

        if (ImGui.BeginPopup("add_control_popup"u8))
        {
            if (ImGui.MenuItem("Button"u8)) AddControl(new Button(10, 1) { Text = "Button" });
            if (ImGui.MenuItem("CheckBox"u8)) AddControl(new CheckBox(15, 1) { Text = "CheckBox" });
            if (ImGui.MenuItem("RadioButton"u8)) AddControl(new RadioButton(15, 1) { Text = "RadioButton" });
            if (ImGui.MenuItem("TextBox"u8)) AddControl(new TextBox(10));
            if (ImGui.MenuItem("Label"u8)) AddControl(new Label("Label"));
            if (ImGui.MenuItem("ProgressBar"u8)) AddControl(new ProgressBar(10, 1, HorizontalAlignment.Left));
            if (ImGui.MenuItem("ToggleSwitch"u8)) AddControl(new ToggleSwitch(5, 1));
            if (ImGui.MenuItem("SelectionButton"u8)) AddControl(new SelectionButton(15, 1));
            ImGui.EndPopup();
        }

        ImGui.BeginChild("controls_list"u8, new Vector2(0, 150), ImGuiChildFlags.Borders);
        foreach (var control in Console.Controls)
        {
            string name = string.IsNullOrEmpty(control.Name) ? control.GetType().Name : control.Name;
            if (ImGui.Selectable($"{name}##{control.GetHashCode()}", SelectedControl == control))
            {
                SelectedControl = control;
            }
        }
        ImGui.EndChild();

        if (SelectedControl != null)
        {
            ImGui.SeparatorText("Selected Control Properties"u8);
            
            ImGui.Text($"Name: {SelectedControl.Name ?? "<unnamed>"}");

            if (SettingsTable.BeginTable("control_props_possize"))
            {
                int x = SelectedControl.Position.X;
                int y = SelectedControl.Position.Y;
                bool posChanged = false;

                if (SettingsTable.DrawInt("X", "control.x",ref x, -100)) posChanged = true;
                if (SettingsTable.DrawInt("Y", "control.y", ref y, -100)) posChanged = true;

                if (posChanged)
                {
                    SelectedControl.Position = new Point(x, y);
                    SelectedControl.IsDirty = true;
                }

                int width = SelectedControl.Width;
                int height = SelectedControl.Height;

                bool sizeChanged = false;

                if (SettingsTable.DrawInt("Width", "control.width", ref width, 1, Console.Surface.Width)) sizeChanged = true;
                if (SettingsTable.DrawInt("Height", "control.height", ref height, 1, Console.Surface.Height)) sizeChanged = true;

                if (sizeChanged) SelectedControl.Resize(width, height);

                SettingsTable.EndTable();
            }

            if (SelectedControl is Button btn)
            {
                string text = btn.Text;
                if (ImGui.InputText("Text"u8, ref text, 100)) btn.Text = text;
            }
            else if (SelectedControl is CheckBox chk)
            {
                string text = chk.Text;
                if (ImGui.InputText("Text"u8, ref text, 100)) chk.Text = text;
            }
            else if (SelectedControl is RadioButton rad)
            {
                string text = rad.Text;
                if (ImGui.InputText("Text"u8, ref text, 100)) rad.Text = text;
            }
            else if (SelectedControl is Label lbl)
            {
                string text = lbl.DisplayText;
                if (ImGui.InputText("Text"u8, ref text, 100)) lbl.DisplayText = text;
            }
            
            if (ImGui.Button("Remove Control"u8))
            {
                Console.Controls.Remove(SelectedControl);
                SelectedControl = null;
            }
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
}

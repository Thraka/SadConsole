using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentLayeredSurface : Document, IDocumentSimpleObjects, IDocumentZones
{
    public ImGuiList<SimpleObjectDefinition> SimpleObjects { get; } = new();

    public ImGuiList<ZoneSimplified> Zones { get; } = new();

    private int _selectedLayerIndex;

    public LayeredScreenSurface LayeredEditingSurface => (LayeredScreenSurface)EditingSurface;

    /// <summary>
    /// Gets the icon for layered surface documents.
    /// </summary>
    public override string DocumentIcon => "\ue257"; // layer-group icon

    public DocumentLayeredSurface(LayeredScreenSurface editingSurface)
    {
        EditingSurface = editingSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        EditingSurface.IsDirty = true;

        Redraw(true, true);
    }

    public void SetActiveLayer(int layerIndex)
    {
        if (layerIndex >= 0 && layerIndex < LayeredEditingSurface.Layers.Count)
        {
            _selectedLayerIndex = layerIndex;
            ((ISurfaceSettable)EditingSurface).Surface = LayeredEditingSurface.Layers[layerIndex];
            EditingSurface.IsDirty = true;

            if (Core.State.Tools.IsItemSelected())
                Core.State.Tools.SelectedItem.Reset(this);

            Redraw(true, true);
        }
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        ImGui.SeparatorText("Layered Surface Settings"u8);

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

                    LayeredEditingSurface.Resize(viewWidth, viewHeight, _width.CurrentValue, _height.CurrentValue, false);

                    _width = new(EditingSurface.Surface.Width);
                    _height = new(EditingSurface.Surface.Height);

                    EditingSurface.IsDirty = true;
                    if (Core.State.Tools.IsItemSelected())
                        Core.State.Tools.SelectedItem.Reset(this);
                }
            }
        }

        ImGui.Separator();

        // Layer management UI
        ImGui.SeparatorText("Layers"u8);

        int layerCount = LayeredEditingSurface.Layers.Count;

        // Layer list
        if (ImGui.BeginChild("layer_list"u8, new Vector2(-1, ImGui.GetTextLineHeightWithSpacing() * Math.Min(8, layerCount + 1)), ImGuiChildFlags.Borders))
        {
            Vector2 iconSize = ImGui.CalcTextSize("\uf06e\uf06e"u8);

            for (int i = layerCount - 1; i >= 0; i--)
            {
                ImGui.PushID(i);

                bool isVisible = LayeredEditingSurface.Layers.GetLayerVisibility(i);
                char icon = isVisible ? '\uf06e' : '\uf070';
                bool isSelected = _selectedLayerIndex == i;
                Vector2 pos = ImGui.GetCursorScreenPos();
                
                if (ImGui.Selectable($"{icon}  Layer {i + 1}", isSelected, ImGuiSelectableFlags.AllowOverlap))
                    SetActiveLayer(i);

                ImGui.SetCursorScreenPos(pos);
                if (ImGui.InvisibleButton("##vis", iconSize))
                    LayeredEditingSurface.Layers.SetLayerVisibility(i, !isVisible);

                ImGui.PopID();
            }
        }
        ImGui.EndChild();

        // Layer management buttons
        if (ImGui.Button("Add Layer"u8))
        {
            var newLayer = LayeredEditingSurface.Layers.Create();
            newLayer.DefaultForeground = EditingSurface.Surface.DefaultForeground;
            newLayer.DefaultBackground = Color.Transparent;
            newLayer.Clear();
            SetActiveLayer(LayeredEditingSurface.Layers.Count - 1);
        }

        ImGui.SameLine();

        ImGui.BeginDisabled(layerCount <= 1);
        if (ImGui.Button("Remove Layer"u8))
        {
            if (_selectedLayerIndex >= 0 && _selectedLayerIndex < layerCount)
            {
                LayeredEditingSurface.Layers.RemoveAt(_selectedLayerIndex);

                if (_selectedLayerIndex >= LayeredEditingSurface.Layers.Count)
                    _selectedLayerIndex = LayeredEditingSurface.Layers.Count - 1;

                SetActiveLayer(_selectedLayerIndex);
            }
        }
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(_selectedLayerIndex <= 0);
        if (ImGui.Button("\uf063"u8)) // Down arrow - move layer down (lower in render order)
        {
            var layer = LayeredEditingSurface.Layers[_selectedLayerIndex];
            LayeredEditingSurface.Layers.RemoveAt(_selectedLayerIndex);
            LayeredEditingSurface.Layers.Insert(_selectedLayerIndex - 1, layer);

            _selectedLayerIndex--;
            EditingSurface.IsDirty = true;
        }
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(_selectedLayerIndex >= layerCount - 1);
        if (ImGui.Button("\uf062"u8)) // Up arrow - move layer up (higher in render order)
        {
            var layer = LayeredEditingSurface.Layers[_selectedLayerIndex];
            LayeredEditingSurface.Layers.RemoveAt(_selectedLayerIndex);
            LayeredEditingSurface.Layers.Insert(_selectedLayerIndex + 1, layer);

            _selectedLayerIndex++;
            EditingSurface.IsDirty = true;
        }
        ImGui.EndDisabled();

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

    public override void ImGuiDrawTopBar(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMenu("Document Options"))
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
        LayeredEditingSurface.Layers.View = new(x, y, width, height);
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new LayeredSurfaceDocument(), new LayeredSurfaceFile()];

    public override string ToString() =>
        Title;
}

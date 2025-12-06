using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentSurface: Document, IDocumentSimpleObjects, IDocumentZones
{
    public ImGuiList<SimpleObjectDefinition> SimpleObjects { get; } = new();
    public ImGuiList<Serialization.ZoneSerialized> Zones { get; } = new();

    public DocumentSurface()
    {
        Tools = Tools.Append(new Tools.Zones()).ToArray();
        Zones.Objects.Add(new Serialization.ZoneSerialized()
        {
            Name = "Default Zone",
            ZoneArea = new(new SadRogue.Primitives.Rectangle(1, 1, 10, 10).Positions()),
            Appearance = new ColoredGlyph()
            {
                Foreground = SadRogue.Primitives.Color.Yellow,
                Background = SadRogue.Primitives.Color.DarkGray,
                Glyph = '@',
                Mirror = SadConsole.Mirror.None
            }
        });
        SyncToolModes();
    }

    public DocumentSurface(CellSurface editingSurface): this()
    {
        EditingSurface = new ScreenSurface(editingSurface);
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        EditingSurface.IsDirty = true;

        Redraw(true, true);
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
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
        ImGui.Text(EditingSurface.Width.ToString());
        ImGui.Text("Height:"u8);
        ImGui.SameLine();
        ImGui.Text(EditingSurface.Height.ToString());
        ImGui.EndGroup();
        ImGui.SameLine(0, ImGui.GetFontSize());

        if (EditingSurface is ICellSurfaceResize)
        {
            if (ImGui.Button("Resize"u8))
            {
                _width = new(EditingSurface.Width);
                _height = new(EditingSurface.Height);
                ImGui.OpenPopup("resize_document");
            }

            if (ResizeSurfacePopup.Show("resize_document", ref _width.CurrentValue, ref _height.CurrentValue, out bool dialogResult))
            {
                if (dialogResult && (_width.IsChanged() || _height.IsChanged()))
                {
                    // Don't sync just the view because the view should be set by the drawing of the surface based on available screen space
                    int viewWidth = Math.Min(EditingSurface.ViewWidth, _width.CurrentValue);
                    int viewHeight = Math.Min(EditingSurface.ViewHeight, _height.CurrentValue);

                    // Resize
                    ((ICellSurfaceResize)EditingSurface).Resize(viewWidth, viewHeight, _width.CurrentValue, _height.CurrentValue, false);

                    // Resync width/height
                    _width = new(EditingSurface.Width);
                    _height = new(EditingSurface.Height);

                    // Redraw
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

            // Force overlay to update and match surface
            //ComposeVisual();
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
            // Force overlay to update and match surface
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

                    if (ImGui.MenuItem("Manage Simple Objects"u8))
                        new Windows.SimpleObjectEditor(SimpleObjects, EditingSurface.Surface.DefaultForeground.ToVector4(), EditingSurface.Surface.DefaultBackground.ToVector4(), EditingSurfaceFont).Open();

                    if (ImGui.MenuItem("Import"u8))
                        throw new NotImplementedException();
                }

                if (enableZones)
                {
                    ImGui.SeparatorText("Zones");

                    if (ImGui.MenuItem("Edit Zones"u8))
                        throw new NotImplementedException();
                }
            }

            ImGui.EndMenu();
        }
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SurfaceDocument(), new SurfaceFile()];

    public override string ToString() =>
        Title;

}

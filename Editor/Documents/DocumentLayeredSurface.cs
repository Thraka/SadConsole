using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentLayeredSurface : Document, IDocumentSimpleObjects
{
    public ImGuiList<SimpleObjectDefinition> SimpleObjects { get; } = new();

    public DocumentLayeredSurface()
    {
    }

    public DocumentLayeredSurface(CellSurface editingSurface)
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
        ImGui.SeparatorText("Surface Settings");

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Name:");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);

        string editTitle = Title;
        if (ImGui.InputText("##name", ref editTitle, 50))
            Title = editTitle;

        ImGui.Separator();

        ImGui.BeginGroup();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Width: ");
        ImGui.SameLine();
        ImGui.Text(EditingSurface.Width.ToString());
        ImGui.Text("Height:");
        ImGui.SameLine();
        ImGui.Text(EditingSurface.Height.ToString());
        ImGui.EndGroup();
        ImGui.SameLine(0, ImGui.GetFontSize());

        if (EditingSurface is ICellSurfaceResize)
        {
            if (ImGui.Button("Resize"))
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
        ImGui.Text("Def. Foreground: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs))
            EditingSurface.Surface.DefaultForeground = DefaultForeground.ToColor();


        var DefaultBackground = EditingSurface.Surface.DefaultBackground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Background: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs))
            EditingSurface.Surface.DefaultBackground = DefaultBackground.ToColor();

        ImGui.Separator();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font: ");
        ImGui.SameLine();
        if (ImGui.Button($"{EditingSurfaceFont.Name} | {EditingSurfaceFontSize}"))
            base.FontSelectionWindow_Popup();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Editor Font Size: ");
        ImGui.SameLine();
        if (ImGui.Button(EditorFontSize.ToString()))
        {
            ImGui.OpenPopup("editorfontsize_select");
        }
        ImGui.SameLine();
        if (ImGui.Button("Reset"))
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

        ImGui.SeparatorText("Simple Game Objects"u8);
        if (ImGui.Button("Manage"u8))
            new Windows.SimpleObjectEditor(SimpleObjects, DefaultForeground, DefaultBackground, EditingSurfaceFont).Open();

        ImGui.SameLine();
        ImGui.Button("Import"u8);
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SurfaceDocument(), new SurfaceFile()];

    public override string ToString() =>
        Title;

}

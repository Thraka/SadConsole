using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentAnimated: Document
{
    private AnimatedScreenObject _baseAnimation;

    public DocumentAnimated()
    {
    }

    public DocumentAnimated(AnimatedScreenObject animationObject)
    {
        _baseAnimation = animationObject;

        EditingSurface = new ScreenSurface(animationObject.CurrentFrame);
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;
        EditingSurface.IsDirty = true;

        Redraw(true, true);
    }

    public void SetFrameIndex(int frameIndex)
    {
        _baseAnimation.CurrentFrameIndex = frameIndex;

        EditingSurface.Surface = _baseAnimation.CurrentFrame;
        EditingSurface.IsDirty = true;

        Redraw(true, true);
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        int currentFrameIndex = _baseAnimation.CurrentFrameIndex;

        // ========================
        // Frames
        // ========================
        ImGui.SeparatorText("Frames");

        float frameButtonArea = ImGui.GetStyle().ItemSpacing.X * 2 + ImGui.GetStyle().FramePadding.X * 4 + ImGui.CalcTextSize("\uf049 "u8).X + ImGui.CalcTextSize("\uf04a "u8).X;

        ImGui.BeginDisabled(_baseAnimation.CurrentFrameIndex == 0);
        if (ImGui.Button("\uf049 "u8)) _baseAnimation.MoveStart(); ImGui.EndDisabled(); ImGui.SameLine();
        ImGui.BeginDisabled(_baseAnimation.CurrentFrameIndex == 0);
        if (ImGui.Button("\uf04a "u8)) _baseAnimation.MovePrevious(); ImGui.EndDisabled(); ImGui.SameLine();

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - frameButtonArea);
        int frameIndex = _baseAnimation.CurrentFrameIndex + 1;
        if (ImGui.SliderInt("##frameslider", ref frameIndex, 1, _baseAnimation.Frames.Count))
            _baseAnimation.CurrentFrameIndex = frameIndex - 1;

        ImGui.SameLine();

        ImGui.BeginDisabled(_baseAnimation.CurrentFrameIndex == _baseAnimation.Frames.Count - 1);
        if (ImGui.Button("\uf04e "u8)) _baseAnimation.MoveNext(); ImGui.EndDisabled(); ImGui.SameLine();
        ImGui.BeginDisabled(_baseAnimation.CurrentFrameIndex == _baseAnimation.Frames.Count - 1);
        if (ImGui.Button("\uf050 "u8)) _baseAnimation.MoveEnd(); ImGui.EndDisabled();

        // If animation frame changed, sync surface
        if (currentFrameIndex != _baseAnimation.CurrentFrameIndex)
            SetFrameIndex(_baseAnimation.CurrentFrameIndex);

        // ========================
        // Animation Settings
        // ========================
        ImGui.SeparatorText("Animation Settings");

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
        {
            //FontPicker popup = new(VisualDocument.Font, SurfaceFontSize);
            //popup.Closed += FontPicker_Closed;
            //popup.Show();
        }
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










        if (FontSizePopup.Show("editorfontsize_select", EditingSurfaceFont, ref EditorFontSize))
        {
            // Force overlay to update and match surface
            EditingSurface.IsDirty = true;
            VisualTool.IsDirty = true;
        }
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SurfaceDocument(), new SurfaceFile()];

    public override string ToString() =>
        Title;

}

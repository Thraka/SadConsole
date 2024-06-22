using System.Numerics;
using System.Runtime.Serialization;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.Model;

internal partial class AnimationDocument : Document, IDocumentTools, IFileHandler
{
    private int _sliderValueY;
    private int _sliderValueX;

    private bool _showPreviousFrame;
    private int _currentFrameCounter = 1;
    private byte _previousFrameOpacity = 50;

    public int Width = 40;
    public int Height = 20;
    public int FrameCount;

    public int ViewX;
    public int ViewY;

    public Vector4 DefaultForeground = Color.White.ToVector4();
    public Vector4 DefaultBackground = Color.Transparent.ToVector4();

    [DataMember]
    public Point SurfaceFontSize;

    [DataMember]
    public Point EditorFontSize;

    [DataMember]
    AnimatedScreenObject _animatedScreenObject;

    Overlay _previousFrameOverlay = new();

    public AnimationDocument()
    {
        //DocumentType = Types.Surface;
        
        Options.UseToolsWindow = true;
        Options.ToolsWindowShowToolsList = true;

        ((IDocumentTools)this).ShowToolsList = true;
        ((IDocumentTools)this).State.ToolObjects = [ new Tools.Info(), new Tools.Empty(), new Tools.Pencil(), new Tools.Recolor(),
                                                     new Tools.Fill(), new Tools.Box(), new Tools.Circle(), new Tools.Line(),
                                                     new Tools.Text(), new Tools.Selection(), new Tools.Operations() ];
        ((IDocumentTools)this).State.ToolNames = ((IDocumentTools)this).State.ToolObjects.Select(t => t.Name).ToArray();

        Name = GenerateName("Animation");
    }

    public override void BuildUINew(ImGuiRenderer renderer)
    {
        float paddingX = ImGui.GetStyle().WindowPadding.X;
        float windowWidth = ImGui.GetWindowWidth();

        ImGui.Text("Name");
        ImGui.InputText("##name", ref Name, 50);

        ImGui.Separator();

        ImGui.Text("Width: ");
        ImGui.SameLine(ImGui.CalcTextSize("Frames: ").X + (ImGui.GetStyle().ItemSpacing.X * 2));
        ImGui.InputInt("##docwidth", ref Width);

        ImGui.Text("Height: ");
        ImGui.SameLine();
        ImGui.InputInt("##docheight", ref Height);

        ImGui.Text("Frames: ");
        ImGui.SameLine();
        ImGui.InputInt("##frames", ref FrameCount, 1);
        FrameCount = Math.Clamp(FrameCount, 2, 25);

        ImGui.Text("Def. Foreground: ");
        ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
        ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs);
        ImGuiCore.State.CheckSetPopupOpen("##forepicker");
        ImGuiCore.State.LayoutInfo.ColorEditBoxWidth = ImGui.GetItemRectSize().X;

        ImGui.Text("Def. Background: ");
        ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
        ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs);
        ImGuiCore.State.CheckSetPopupOpen("##backpicker");
    }

    public override void BuildUIEdit(ImGuiRenderer renderer, bool readOnly)
    {
        ImGuiWidgets.BeginGroupPanel("Animation Settings");

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Name:");
        ImGui.SameLine();
        ImGui.InputText("##name", ref Name, 50);

        ImGui.Separator();

        if (ImGui.BeginTable("table1", 3))
        {
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("two", ImGuiTableColumnFlags.WidthFixed);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Width: ");
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(Width.ToString());
            ImGui.TableSetColumnIndex(2);

            if (ImGui.Button("Resize"))
                ImGuiCore.State.OpenPopup("resize_document");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Height: ");
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(Height.ToString());

            if (ResizeSurfacePopup.Show("resize_document", ref Width, ref Height, out bool dialogResult))
            {
                if (dialogResult)
                {
                    if (Surface.Surface.ViewWidth > Width)
                        Surface.Surface.ViewWidth = Width;
                    if (Surface.Surface.ViewHeight > Height)
                        Surface.Surface.ViewHeight = Height;

                    //TODO: TEST
                    ((ICellSurfaceResize)Surface.Surface).Resize(Surface.Surface.ViewWidth, Surface.Surface.ViewHeight, Width, Height, false);
                    Surface.IsDirty = true;
                }
                else
                {
                    Width = Surface.Surface.Width;
                    Height = Surface.Surface.Height;
                }
            }

            ImGui.EndTable();
        }

        ImGui.Separator();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font: ");
        ImGui.SameLine();
        if (ImGui.Button($"{Surface.Font.Name} | {SurfaceFontSize}"))
        {
            FontPicker popup = new(Surface.Font, SurfaceFontSize);
            popup.Closed += FontPicker_Closed;
            popup.Show();
        }
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Editor Font Size: ");
        ImGui.SameLine();
        if (ImGui.Button(EditorFontSize.ToString()))
        {
            ImGuiCore.State.OpenPopup("editorfontsize_select");
        }
        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            EditorFontSize = SurfaceFontSize;
            _animatedScreenObject.FontSize = EditorFontSize;
            Surface.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            foreach (Overlay overlay in Surface.GetSadComponents<Components.Overlay>())
                overlay.Update(Surface, TimeSpan.Zero);
        }

        if (FontSizePopup.Show(renderer, "editorfontsize_select", Surface.Font, ref EditorFontSize))
        {
            _animatedScreenObject.FontSize = EditorFontSize;
            Surface.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            foreach (Overlay overlay in Surface.GetSadComponents<Components.Overlay>())
                overlay.Update(Surface, TimeSpan.Zero);
        }
        ImGuiWidgets.EndGroupPanel();

        ImGuiWidgets.BeginGroupPanel("Animation Editor");
        {
            ImGui.AlignTextToFramePadding();

            // Current Frame
            // =======================
            //ImGui.Text($"Current Frame: {_currentFrameCounter}\\{_animatedScreenObject.Frames.Count}");

            //ImGui.SameLine();
            if (ImGui.Button("<<"))
                Frames_GoStart();

            ImGui.SameLine();
            if (ImGui.Button("<"))
                Frames_GoPrevious();

            float textSize = ImGui.CalcTextSize(">>").X + ImGui.CalcTextSize(">").X;
            Vector2 spacing = ImGui.GetStyle().ItemSpacing;
            Vector2 padding = ImGui.GetStyle().FramePadding;

            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGui.GetItemRectSize().X - (spacing.X * 3) - (textSize * 2) - padding.X);
            if (ImGui.SliderInt("##currentframe", ref _currentFrameCounter, 1, _animatedScreenObject.Frames.Count))
                Frames_GoCurrent();

            ImGui.SameLine();
            if (ImGui.Button(">"))
                Frames_GoNext();

            ImGui.SameLine();
            if (ImGui.Button(">>"))
                Frames_GoEnd();

            // Adding Frames
            // =======================
            ImGui.Text("Adding Frames: ");

            ImGui.SameLine();
            if (ImGui.SmallButton("Add To End"))
            {
                Frames_AddToEnd();

                if (_currentFrameCounter == _animatedScreenObject.Frames.Count - 1)
                    Frames_GoNext();

            }

            ImGui.SameLine();
            if (ImGui.SmallButton("Insert Here"))
                Frames_Insert();

            // Editing Frames
            // =======================
            ImGui.Text("Editing Frames: ");

            ImGui.SameLine();
            if (ImGui.SmallButton("Copy Prev."))
                Frames_CopyPrevious();

            ImGui.SameLine();
            if (ImGui.SmallButton("Clear This"))
                Frames_Reset(_animatedScreenObject.CurrentFrame);

            ImGui.Text("Delete Frames: ");
            ImGui.SameLine();
            if (ImGui.SmallButton("Delete This"))
                Frames_Delete();

            // Moving Frames
            // =======================
            ImGui.Text("Editing Frames: ");
            ImGui.SameLine();
            if (ImGui.SmallButton("<<##movestart"))
                Frames_MoveStart();

            ImGui.SameLine();
            if (ImGui.SmallButton("<##moveback"))
                Frames_MoveBack();

            ImGui.SameLine();
            if (ImGui.SmallButton(">##moveforward"))
                Frames_MoveForward();

            ImGui.SameLine();
            if (ImGui.SmallButton(">>##moveend"))
                Frames_MoveEnd();

            ImGui.Separator();

            if (ImGui.Checkbox("Show Previous Frame Trace", ref _showPreviousFrame))
                SetupTraceOverlay();

            if (_showPreviousFrame)
            {
                float value = (_previousFrameOpacity / 255f) * 100;
                ImGui.Text("Opacity: ");
                ImGui.SameLine();
                if (ImGui.SliderFloat("##opacity", ref value, 0f, 100f, "%.0f%%"))//, ImGuiSliderFlags.None))
                {
                    _previousFrameOpacity = (byte)((value / 100) * 255);
                    ((Renderers.SurfaceRenderStep)_previousFrameOverlay.RenderStep!).ComposeTint = Color.White.SetAlpha(_previousFrameOpacity).ToMonoColor();
                    _previousFrameOverlay.Surface!.IsDirty = true;
                }
            }
        }
        ImGuiWidgets.EndGroupPanel();
    }

    private void FontPicker_Closed(object? sender, EventArgs e)
    {
        FontPicker window = (FontPicker)sender!;
        if (window.DialogResult)
        {
            Surface.Font = window.Font;
            Surface.FontSize = window.FontSize;
            EditorFontSize = window.FontSize;
            SurfaceFontSize = window.FontSize;

            _animatedScreenObject.FontSize = SurfaceFontSize;
            Surface.FontSize = SurfaceFontSize;
        }
    }

    public override void BuildUIDocument(ImGuiRenderer renderer)
    {
        BuildUIDocumentStandard(renderer, Surface);
    }

    public override void OnShow(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = Surface;
    }

    public override void OnHide(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = null;
    }

    public override IEnumerable<IFileHandler> GetLoadHandlers() =>
        [this, new SurfaceFile()];

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [this, new SurfaceFile(), new SurfaceFileCompressed()];

    public override bool HydrateFromFileHandler(IFileHandler handler, string file)
    {
        return false;
    }

    public override object DehydrateToFileHandler(IFileHandler handler, string file)
    {
        return this;
    }

    public override void Create()
    {
        _animatedScreenObject = new(Name, Width, Height);
        
        for (int i = 0; i < FrameCount; i++)
            _animatedScreenObject.CreateFrame();

        foreach (ICellSurface frame in _animatedScreenObject.Frames)
            Frames_Reset(frame);

        EditorFontSize = _animatedScreenObject.FontSize;
        SurfaceFontSize = EditorFontSize;
        CreateSurfaceFromFrame();
    }

    private void CreateSurfaceFromFrame()
    {
        if (Surface is ScreenSurface surface)
        {
            if (surface.SadComponents.Contains(_previousFrameOverlay))
                surface.SadComponents.Remove(_previousFrameOverlay);

            surface.Dispose();
        }

        ICellSurface newSurface = _animatedScreenObject.CurrentFrame.GetSubSurface();
        Surface = new ScreenSurface(newSurface, null, EditorFontSize);
        Surface.Render(TimeSpan.Zero);
        SadConsole.Game.Instance.Screen = Surface;

        SetupTraceOverlay();
    }

    private void SetupTraceOverlay()
    {
        if (_showPreviousFrame)
        {
            if (!Surface.SadComponents.Contains(_previousFrameOverlay))
            {
                Surface.SadComponents.Add(_previousFrameOverlay);
                ((Renderers.SurfaceRenderStep)_previousFrameOverlay.RenderStep!).ComposeTint = Color.White.SetAlpha(_previousFrameOpacity).ToMonoColor();

                if (_animatedScreenObject.CurrentFrameIndex == 0)
                    _animatedScreenObject.Frames[^1].Copy(_previousFrameOverlay.Surface!.Surface);
                else
                    _animatedScreenObject.Frames[_animatedScreenObject.CurrentFrameIndex - 1].Copy(_previousFrameOverlay.Surface!.Surface);
            }
        }
        else
        {
            if (Surface.SadComponents.Contains(_previousFrameOverlay))
                Surface.SadComponents.Remove(_previousFrameOverlay);
        }
    }

    private void Frames_Reset(ICellSurface frame)
    {
        frame.DefaultBackground = DefaultBackground.ToColor();
        frame.DefaultForeground = DefaultForeground.ToColor();
        frame.Clear();
    }

    private void Frames_GoCurrent()
    {
        _animatedScreenObject.CurrentFrameIndex = _currentFrameCounter - 1;
        CreateSurfaceFromFrame();
    }

    private void Frames_GoNext()
    {
        _currentFrameCounter++;
        if (_currentFrameCounter > _animatedScreenObject.Frames.Count)
            _currentFrameCounter = 1;
        _animatedScreenObject.CurrentFrameIndex = _currentFrameCounter - 1;
        CreateSurfaceFromFrame();
    }

    private void Frames_GoStart()
    {
        _currentFrameCounter = 1;
        _animatedScreenObject.CurrentFrameIndex = _currentFrameCounter - 1;
        CreateSurfaceFromFrame();
    }

    private void Frames_GoEnd()
    {
        _currentFrameCounter = _animatedScreenObject.Frames.Count;
        _animatedScreenObject.CurrentFrameIndex = _currentFrameCounter - 1;
        CreateSurfaceFromFrame();
    }

    private void Frames_GoPrevious()
    {
        _currentFrameCounter--;
        if (_currentFrameCounter == 0)
            _currentFrameCounter = _animatedScreenObject.Frames.Count;
        _animatedScreenObject.CurrentFrameIndex = _currentFrameCounter - 1;
        CreateSurfaceFromFrame();
    }

    private void Frames_Insert()
    {
        Frames_Reset(_animatedScreenObject.CreateFrame());

        ICellSurface frame = _animatedScreenObject.Frames[^1];
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex, frame);

        CreateSurfaceFromFrame();
    }

    private void Frames_CopyPrevious()
    {
        int targetIndex = _currentFrameCounter - 2;
        if (targetIndex == -1)
            targetIndex = _animatedScreenObject.Frames.Count - 1;

        _animatedScreenObject.Frames[targetIndex].Copy(_animatedScreenObject.CurrentFrame);
        Surface.IsDirty = true;
    }

    private void Frames_Delete()
    {
        throw new NotImplementedException();
    }

    private void Frames_AddToEnd() =>
        Frames_Reset(_animatedScreenObject.CreateFrame());

    private void Frames_MoveEnd()
    {
        if (_currentFrameCounter != _animatedScreenObject.Frames.Count)
        {
            ICellSurface frame = _animatedScreenObject.CurrentFrame;
            _animatedScreenObject.Frames.Remove(frame);
            _animatedScreenObject.Frames.Add(frame);
            Frames_GoEnd();
        }
    }

    private void Frames_MoveForward()
    {
        if (_currentFrameCounter != _animatedScreenObject.Frames.Count)
        {
            ICellSurface frame = _animatedScreenObject.CurrentFrame;
            _animatedScreenObject.Frames.Remove(frame);
            _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex + 1, frame);
            Frames_GoNext();
        }
    }

    private void Frames_MoveStart()
    {
        if (_currentFrameCounter != 1)
        {
            ICellSurface frame = _animatedScreenObject.CurrentFrame;
            _animatedScreenObject.Frames.Remove(frame);
            _animatedScreenObject.Frames.Insert(0, frame);
            Frames_GoStart();
        }
    }

    private void Frames_MoveBack()
    {
        if (_currentFrameCounter != 1)
        {
            ICellSurface frame = _animatedScreenObject.CurrentFrame;
            _animatedScreenObject.Frames.Remove(frame);
            _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex - 1, frame);
            Frames_GoPrevious();
        }
    }
}

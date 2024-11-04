using System.Numerics;
using System.Runtime.Serialization;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal partial class AnimationDocument : Document, IDocumentTools, IFileHandler
{
    private int _sliderValueY;
    private int _sliderValueX;

    private bool _showPreviousFrame;
    private byte _previousFrameOpacity = 50;

    public int Width = 40;
    public int Height = 20;
    public int FrameCount;

    [DataMember]
    public int ViewX;
    [DataMember]
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
        if (ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs));
        {
            if (VisualDocument != null)
                VisualDocument.Surface.DefaultForeground = DefaultForeground.ToColor();
        }
        ImGuiCore.State.CheckSetPopupOpen("##forepicker");
        ImGuiCore.State.LayoutInfo.ColorEditBoxWidth = ImGui.GetItemRectSize().X;

        ImGui.Text("Def. Background: ");
        ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
        if (ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs))
        {
            if (VisualDocument != null)
                VisualDocument.Surface.DefaultBackground = DefaultBackground.ToColor();
        }
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
                    foreach (ICellSurfaceResize frame in _animatedScreenObject.Frames)
                    {
                        frame.Resize(Width, Height, false);
                    }

                    Frames_GoCurrent();
                }
                else
                {
                    Width = base.VisualDocument.Surface.Width;
                    Height = base.VisualDocument.Surface.Height;
                }
            }

            ImGui.EndTable();
        }

        ImGui.Separator();

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font: ");
        ImGui.SameLine();
        if (ImGui.Button($"{base.VisualDocument.Font.Name} | {SurfaceFontSize}"))
        {
            FontPicker popup = new(base.VisualDocument.Font, SurfaceFontSize);
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
            VisualDocument.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            foreach (Overlay overlay in VisualDocument.GetSadComponents<Components.Overlay>())
                overlay.Update(VisualDocument, TimeSpan.Zero);
        }

        if (FontSizePopup.Show(renderer, "editorfontsize_select", VisualDocument.Font, ref EditorFontSize))
        {
            _animatedScreenObject.FontSize = EditorFontSize;
            VisualDocument.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            foreach (Overlay overlay in VisualDocument.GetSadComponents<Components.Overlay>())
                overlay.Update(VisualDocument, TimeSpan.Zero);
        }
        ImGuiWidgets.EndGroupPanel();

        ImGuiWidgets.BeginGroupPanel("Animation Editor");
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Frames"); ImGui.SameLine();
            // Current Frame
            // =======================
            //ImGui.Text($"Current Frame: {_currentFrameCounter}\\{_animatedScreenObject.Frames.Count}");

            //ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == 0);
            if (ImGui.Button("<<"))
                Frames_GoStart();
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == 0);
            if (ImGui.Button("<"))
                Frames_GoPrevious();
            ImGui.EndDisabled();

            float textSize = ImGui.CalcTextSize(">>").X + ImGui.CalcTextSize(">").X;
            Vector2 spacing = ImGui.GetStyle().ItemSpacing;
            Vector2 padding = ImGui.GetStyle().FramePadding;

            ImGui.SameLine();
            ImGui.SetNextItemWidth(ImGui.GetItemRectSize().X - (spacing.X * 3) - (textSize * 2) - padding.X);
            ImGui.BeginDisabled(_animatedScreenObject.Frames.Count == 1);
            int sliderFrame = _animatedScreenObject.CurrentFrameIndex + 1;
            if (ImGui.SliderInt("##currentframe", ref sliderFrame, 1, _animatedScreenObject.Frames.Count))
            {
                _animatedScreenObject.CurrentFrameIndex = sliderFrame - 1;
                Frames_GoCurrent();
            }
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count - 1);
            if (ImGui.Button(">"))
                Frames_GoNext();
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count - 1);
            if (ImGui.Button(">>"))
                Frames_GoEnd();
            ImGui.EndDisabled();

            // Adding Frames
            // =======================
            ImGui.Text("Add: ");

            ImGui.SameLine();
            if (ImGui.SmallButton("Add To End"))
            {
                Frames_AddToEnd();

                if (_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count - 2)
                    Frames_GoNext();

            }

            ImGui.SameLine();
            if (ImGui.SmallButton("Insert Here"))
                Frames_Insert();

            // Editing Frames
            // =======================
            ImGui.Text("Edit: ");

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.Frames.Count == 1);
            if (ImGui.SmallButton("Copy Prev."))
                Frames_CopyPrevious();
            ImGui.EndDisabled();

            ImGui.SameLine();
            if (ImGui.SmallButton("Clear This"))
                Frames_Reset(_animatedScreenObject.CurrentFrame);

            // Delete Frames
            // =======================
            ImGui.Text("Delete: ");

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.Frames.Count == 1);
            if (ImGui.SmallButton("Delete This"))
                Frames_Delete();
            ImGui.EndDisabled();

            // Moving Frames
            // =======================
            ImGui.Text("Move: ");
            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == 0);
            if (ImGui.SmallButton("<<##movestart"))
                Frames_MoveStart();
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == 0);
            if (ImGui.SmallButton("<##moveback"))
                Frames_MoveBack();
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count - 1);
            if (ImGui.SmallButton(">##moveforward"))
                Frames_MoveForward();
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count - 1);
            if (ImGui.SmallButton(">>##moveend"))
                Frames_MoveEnd();
            ImGui.EndDisabled();

            ImGui.Separator();

            if (ImGui.Checkbox("Show Previous Frame Trace", ref _showPreviousFrame))
                // Regardless of if it's on/off, setup and compose
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
            VisualDocument.Font = window.Font;
            VisualDocument.FontSize = window.FontSize;
            EditorFontSize = window.FontSize;
            SurfaceFontSize = window.FontSize;

            _animatedScreenObject.FontSize = SurfaceFontSize;
            VisualDocument.FontSize = SurfaceFontSize;
            ComposeVisual();
        }
    }

    public override void BuildUIDocument(ImGuiRenderer renderer)
    {
        BuildUIDocumentStandard(renderer);
    }

    public override IEnumerable<IFileHandler> GetLoadHandlers() =>
        [this, new AnimationFile()];

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [this, new AnimationFile()];

    public override bool HydrateFromFileHandler(IFileHandler handler, string file)
    {
        if (handler is AnimationDocument documentHandler)
        {
            documentHandler = (AnimationDocument)handler.Load(file);
            _animatedScreenObject = documentHandler._animatedScreenObject;
            FrameCount = _animatedScreenObject.Frames.Count;
            DocumentType = documentHandler.DocumentType;
            Height = documentHandler.Height;
            Width = documentHandler.Width;
            Name = documentHandler.Name;
            Options = documentHandler.Options;
            ViewX = 0;
            ViewY = 0;
            EditorFontSize = documentHandler.EditorFontSize;
            SurfaceFontSize = documentHandler.SurfaceFontSize;
            CreateSurfaceFromFrame();
            VisualDocument.FontSize = EditorFontSize;
            VisualDocument.Render(TimeSpan.Zero);
            LoadPaletteIfExist(file + ".pal");
            return true;
        }
        else if (handler is AnimationFile)
        {
            _animatedScreenObject = (AnimatedScreenObject)handler.Load(file);
            FrameCount = _animatedScreenObject.Frames.Count;
            Width = _animatedScreenObject.Frames[0].Width;
            Height = _animatedScreenObject.Frames[0].Height;
            DefaultBackground = _animatedScreenObject.Frames[0].DefaultBackground.ToVector4();
            DefaultForeground = _animatedScreenObject.Frames[0].DefaultForeground.ToVector4();
            EditorFontSize = _animatedScreenObject.FontSize;
            SurfaceFontSize = _animatedScreenObject.FontSize;
            CreateSurfaceFromFrame();
            LoadPaletteIfExist(file + ".pal");
            return true;
        }

        return false;
    }

    public override object DehydrateToFileHandler(IFileHandler handler, string file)
    {
        if (handler is AnimationFile)
        {
            AnimatedScreenObject output = new(_animatedScreenObject.Name, _animatedScreenObject.Frames);
            output.Center = _animatedScreenObject.Center;
            output.Repeat = _animatedScreenObject.Repeat;
            output.AnimationDuration = _animatedScreenObject.AnimationDuration;
            output.FontSize = SurfaceFontSize;

            return output;
        }

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

    private void ClearTool() =>
        ((IDocumentTools)this).State.SelectedTool?.DocumentViewChanged(this);

    private void CreateSurfaceFromFrame()
    {
        //Rectangle previousView = default;

        VisualDocument ??= new ScreenSurface(1, 1);

        VisualDocument.FontSize = EditorFontSize;
        //((ICellSurfaceSettable)VisualDocument.Surface).SetSurface(_animatedScreenObject.CurrentFrame.GetSubSurface(), previousView);
        ((ScreenSurface)VisualDocument).Surface = _animatedScreenObject.CurrentFrame;
        DefaultBackground = VisualDocument.Surface.DefaultBackground.ToVector4();
        DefaultForeground = VisualDocument.Surface.DefaultForeground.ToVector4();

        VisualDocument.Render(TimeSpan.Zero);

        SetupTraceOverlay();
    }

    private void SetupTraceOverlay()
    {
        ClearTool();

        if (_showPreviousFrame)
        {
            if (!VisualDocument.SadComponents.Contains(_previousFrameOverlay))
            {
                VisualDocument.SadComponents.Add(_previousFrameOverlay);
                ((Renderers.SurfaceRenderStep)_previousFrameOverlay.RenderStep!).ComposeTint = Color.White.SetAlpha(_previousFrameOpacity).ToMonoColor();

                if (_animatedScreenObject.CurrentFrameIndex == 0)
                    _animatedScreenObject.Frames[^1].Copy(_previousFrameOverlay.Surface!.Surface);
                else
                    _animatedScreenObject.Frames[_animatedScreenObject.CurrentFrameIndex - 1].Copy(_previousFrameOverlay.Surface!.Surface);
            }
        }
        else
        {
            if (VisualDocument.SadComponents.Contains(_previousFrameOverlay))
                VisualDocument.SadComponents.Remove(_previousFrameOverlay);
        }
        ComposeVisual();
    }

    private void Frames_Reset(ICellSurface frame)
    {
        ClearTool();
        frame.DefaultBackground = DefaultBackground.ToColor();
        frame.DefaultForeground = DefaultForeground.ToColor();
        frame.Clear();

        if (VisualDocument != null)
            VisualDocument.IsDirty = true;
    }

    private void Frames_GoCurrent()
    {
        ClearTool();
        CreateSurfaceFromFrame();
    }

    private void Frames_GoNext()
    {
        ClearTool();
        _animatedScreenObject.MoveNext(true);
        CreateSurfaceFromFrame();
    }

    private void Frames_GoStart()
    {
        ClearTool();
        _animatedScreenObject.MoveStart();
        CreateSurfaceFromFrame();
    }

    private void Frames_GoEnd()
    {
        ClearTool();
        _animatedScreenObject.MoveEnd();
        CreateSurfaceFromFrame();
    }

    private void Frames_GoPrevious()
    {
        ClearTool();
        _animatedScreenObject.MovePrevious(true);
        CreateSurfaceFromFrame();
    }

    private void Frames_Insert()
    {
        ClearTool();
        Frames_Reset(_animatedScreenObject.CreateFrame());

        ICellSurface frame = _animatedScreenObject.Frames[^1];
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex, frame);

        CreateSurfaceFromFrame();
    }

    private void Frames_CopyPrevious()
    {
        ClearTool();
        int targetIndex = _animatedScreenObject.CurrentFrameIndex - 1;
        if (targetIndex == -1)
            targetIndex = _animatedScreenObject.Frames.Count - 1;

        _animatedScreenObject.Frames[targetIndex].Copy(_animatedScreenObject.CurrentFrame);
        VisualDocument.IsDirty = true;
    }

    private void Frames_Delete()
    {
        ClearTool();
        _animatedScreenObject.Frames.Remove(_animatedScreenObject.CurrentFrame);

        if (_animatedScreenObject.CurrentFrameIndex == _animatedScreenObject.Frames.Count)
            _animatedScreenObject.CurrentFrameIndex--;

        Frames_GoCurrent();
    }

    private void Frames_AddToEnd() =>
        Frames_Reset(_animatedScreenObject.CreateFrame());

    private void Frames_MoveEnd()
    {
        ClearTool();
        ICellSurface frame = _animatedScreenObject.CurrentFrame;
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Add(frame);
        Frames_GoEnd();
    }

    private void Frames_MoveForward()
    {
        ClearTool();
        ICellSurface frame = _animatedScreenObject.CurrentFrame;
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex + 1, frame);
        Frames_GoNext();
    }

    private void Frames_MoveStart()
    {
        ClearTool();
        ICellSurface frame = _animatedScreenObject.CurrentFrame;
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Insert(0, frame);
        Frames_GoStart();
    }

    private void Frames_MoveBack()
    {
        ClearTool();
        ICellSurface frame = _animatedScreenObject.CurrentFrame;
        _animatedScreenObject.Frames.Remove(frame);
        _animatedScreenObject.Frames.Insert(_animatedScreenObject.CurrentFrameIndex - 1, frame);
        Frames_GoPrevious();
    }
}

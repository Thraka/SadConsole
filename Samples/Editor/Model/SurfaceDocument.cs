using System.Numerics;
using System.Runtime.Serialization;
using ImGuiNET;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

internal partial class SurfaceDocument : Document, IDocumentTools, IFileHandler
{
    private int _sliderValueY;
    private int _sliderValueX;

    public int Width = 80;
    public int Height = 25;

    public int ViewX;
    public int ViewY;

    public Vector4 DefaultForeground = Color.White.ToVector4();
    public Vector4 DefaultBackground = Color.Black.ToVector4();

    [DataMember]
    public Point SurfaceFontSize;
    [DataMember]
    public Point EditorFontSize;

    [DataMember]
    public IScreenSurface Surface { get => base.Surface; set => base.Surface = value; }

    public SurfaceDocument()
    {
        //DocumentType = Types.Surface;

        Options.UseToolsWindow = true;
        Options.ToolsWindowShowToolsList = true;

        ((IDocumentTools)this).ShowToolsList = true;
        ((IDocumentTools)this).State.ToolObjects = [ new Tools.Info(), new Tools.Empty(), new Tools.Pencil(), new Tools.Recolor(),
                                                     new Tools.Fill(), new Tools.Box(), new Tools.Circle(), new Tools.Line(),
                                                     new Tools.Text(), new Tools.Selection(), new Tools.Operations() ];
        ((IDocumentTools)this).State.ToolNames = ((IDocumentTools)this).State.ToolObjects.Select(t => t.Name).ToArray();
    }

    public override void BuildUINew(ImGuiRenderer renderer)
    {
        float paddingX = ImGui.GetStyle().WindowPadding.X;
        float windowWidth = ImGui.GetWindowWidth();

        ImGui.Text("Name");
        ImGui.InputText("##name", ref Name, 50);

        ImGui.Separator();

        ImGui.Text("Width: ");
        ImGui.SameLine(ImGui.CalcTextSize("Height: ").X + (ImGui.GetStyle().ItemSpacing.X * 2));
        ImGui.InputInt("##docwidth", ref Width);

        ImGui.Text("Height: ");
        ImGui.SameLine();
        ImGui.InputInt("##docheight", ref Height);

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
        ImGuiWidgets.BeginGroupPanel("Document Settings");

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

        DefaultForeground = Surface.Surface.DefaultForeground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Foreground: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs))
            Surface.Surface.DefaultForeground = DefaultForeground.ToColor();
        ImGuiCore.State.CheckSetPopupOpen("##forepicker");

        DefaultBackground = Surface.Surface.DefaultBackground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Background: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs))
            Surface.Surface.DefaultBackground = DefaultBackground.ToColor();
        ImGuiCore.State.CheckSetPopupOpen("##backpicker");

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
            Surface.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            Components.Overlay? overlay = Surface.GetSadComponent<Components.Overlay>();
            if (overlay != null)
                overlay.Update(Surface, TimeSpan.Zero);
        }

        if (FontSizePopup.Show(renderer, "editorfontsize_select", Surface.Font, ref EditorFontSize))
        {
            Surface.FontSize = EditorFontSize;

            // Force overlay to update and match surface
            Components.Overlay? overlay = Surface.GetSadComponent<Components.Overlay>();
            if (overlay != null)
                overlay.Update(Surface, TimeSpan.Zero);
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
        if (handler is SurfaceDocument documentHandler)
        {
            documentHandler = (SurfaceDocument)handler.Load(file);
            //TODO: Should most of the these ... = documentHandler.value calls just go to the surface directly??
            Surface = documentHandler.Surface;
            DefaultBackground = Surface.Surface.DefaultBackground.ToVector4();
            DefaultForeground = Surface.Surface.DefaultForeground.ToVector4();
            DocumentType = documentHandler.DocumentType;
            Height = documentHandler.Height;
            Width = documentHandler.Width;
            Name = documentHandler.Name;
            Options = documentHandler.Options;
            ViewX = 0;
            ViewY = 0;
            EditorFontSize = documentHandler.EditorFontSize;
            SurfaceFontSize = documentHandler.SurfaceFontSize;
            Surface.FontSize = EditorFontSize;
            Surface.Render(TimeSpan.Zero);
            return true;
        }
        else if (handler is SurfaceFileCompressed compressedHandler)
        {
            Surface = (ScreenSurface)compressedHandler.Load(file);
            Width = Surface.Surface.Width;
            Height = Surface.Surface.Height;
            DefaultBackground = Surface.Surface.DefaultBackground.ToVector4();
            DefaultForeground = Surface.Surface.DefaultForeground.ToVector4();
            EditorFontSize = Surface.FontSize;
            SurfaceFontSize = Surface.FontSize;
            Surface.Render(TimeSpan.Zero);
            return true;
        }
        else if (handler is SurfaceFile surfaceHandler)
        {
            Surface = (ScreenSurface)surfaceHandler.Load(file);
            Width = Surface.Surface.Width;
            Height = Surface.Surface.Height;
            DefaultBackground = Surface.Surface.DefaultBackground.ToVector4();
            DefaultForeground = Surface.Surface.DefaultForeground.ToVector4();
            EditorFontSize = Surface.FontSize;
            SurfaceFontSize = Surface.FontSize;
            Surface.Render(TimeSpan.Zero);
            return true;
        }
        else
            return false;
    }

    public override object DehydrateToFileHandler(IFileHandler handler, string file)
    {
        if (handler is SurfaceFileCompressed || handler is SurfaceFile)
        {
            ScreenSurface newSurface = new(Surface.Surface, Surface.Font, SurfaceFontSize);
            return newSurface;
        }

        return this;
    }


    public override void Create()
    {
        Surface = new ScreenSurface(Width, Height);
        Surface.Surface.DefaultBackground = DefaultBackground.ToColor();
        Surface.Surface.DefaultForeground = DefaultForeground.ToColor();
        Surface.Surface.Clear();
        EditorFontSize = Surface.FontSize;
        SurfaceFontSize = Surface.FontSize;
        Surface.Render(TimeSpan.Zero);
    }

    public static SurfaceDocument FromSettings(int width, int height, Color foreground, Color background)
    {
        SurfaceDocument doc = new SurfaceDocument()
        {
            Width = width,
            Height = height,
            DefaultForeground = foreground.ToVector4(),
            DefaultBackground = background.ToVector4(),
        };

        doc.Create();

        return doc;
    }
}

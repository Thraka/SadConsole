using System.Numerics;
using System.Runtime.Serialization;
using ImGuiNET;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model;

public abstract class Document
{
    private static int _id;
    private string _uniqueIdentifier = GenerateCharacterId();

    public DocumentOptions Options = new DocumentOptions();

    public string UniqueIdentifier => _uniqueIdentifier;

    public string Name = GenerateName("Document");

    public DocumentTypes DocumentType;

    public IScreenSurface Surface;

    public virtual void OnShow(ImGuiRenderer renderer)
    {

    }

    public virtual void OnHide(ImGuiRenderer renderer)
    {

    }

    public abstract void Create();

    public abstract void BuildUIEdit(ImGuiRenderer renderer, bool readOnly);

    public abstract void BuildUINew(ImGuiRenderer renderer);

    public abstract void BuildUIDocument(ImGuiRenderer renderer);

    public abstract IEnumerable<IFileHandler> GetLoadHandlers();
    public abstract IEnumerable<IFileHandler> GetSaveHandlers();

    public abstract bool HydrateFromFileHandler(IFileHandler handler, string file);

    public abstract object DehydrateToFileHandler(IFileHandler handler, string file);

    protected static string GenerateName(string prefix) =>
        $"{prefix}|{GenerateCharacterId()}";

    protected static string GenerateCharacterId()
    {
        char[] characters = new char[6];
        foreach (var index in Enumerable.Range(1, 6))
        {
            characters[index - 1] = (char)Random.Shared.Next((int)'a', ((int)'z') + 1);
        }
        return new string(characters);
    }

    protected void BuildUIDocumentStandard(ImGuiRenderer renderer, IScreenSurface Surface)
    {
        Vector2 topLeft = ImGui.GetCursorPos();
        Vector2 region = ImGui.GetContentRegionAvail();
        Vector2 imageSize = new Vector2(Surface.Renderer!.Output.Width, Surface.Renderer.Output.Height);
        int barSize = 15;
        Vector2 padding = ImGui.GetStyle().FramePadding;
        bool bigX = false;

        int newViewWidth = (int)(region.X - barSize - (padding.X * 2)) / Surface.FontSize.X;
        int newViewHeight = (int)(region.Y - barSize - 2 - (padding.Y * 2)) / Surface.FontSize.Y; // minus 2 is here because of button height

        newViewWidth = Math.Max(newViewWidth, 1);
        newViewHeight = Math.Max(newViewHeight, 1);

        if (Surface.Surface.Width < newViewWidth && Surface.Surface.Width != Surface.Surface.ViewWidth)
            Surface.Surface.ViewWidth = Surface.Surface.Width;
        else if (Surface.Surface.Width > newViewWidth)
            Surface.Surface.ViewWidth = newViewWidth;

        if (Surface.Surface.Height < newViewHeight && Surface.Surface.Height != Surface.Surface.ViewHeight)
            Surface.Surface.ViewHeight = Surface.Surface.Height;
        else if (Surface.Surface.Height > newViewHeight)
            Surface.Surface.ViewHeight = newViewHeight;

        // Print stats
        ImGuiCore.GuiTopBar.StatusItems.Add((Vector4.Zero, "| ViewPort:"));
        ImGuiCore.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), Surface.Surface.View.ToString()));

        // Force overlay to update and match surface
        Components.Overlay? overlay = Surface.GetSadComponent<Components.Overlay>();
        overlay?.Update(Surface, TimeSpan.Zero);

        Point hoveredCellPosition;

        ImGui.BeginChild("doc_surface");
        //isActive = ImGui.IsItemActive();

        //if (idother != 0 && idvalue != idother) System.Diagnostics.Debugger.Break();

        ImGuiExt.DrawTextureChild("output_preview_surface1", true, ImGuiExt.ZoomNormal, ((Host.GameTexture)Surface.Renderer.Output).Texture, imageSize, renderer, out bool isActive, out bool isHovered);

        Tools.ITool? tool = ((IDocumentTools)this).State.SelectedTool;
        tool?.DrawOverDocument();

        Vector2 mousePosition = ImGui.GetMousePos();
        Vector2 pos = mousePosition - ImGui.GetItemRectMin();
        if (Surface.AbsoluteArea.WithPosition((0, 0)).Contains(new Point((int)pos.X, (int)pos.Y)))
        {
            if (isHovered)
            {
                hoveredCellPosition = new Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(Surface.FontSize) + Surface.Surface.ViewPosition;
                tool?.MouseOver(Surface, hoveredCellPosition, isActive, renderer);
                ImGuiCore.GuiTopBar.StatusItems.Add((Vector4.Zero, "| Mouse:"));
                ImGuiCore.GuiTopBar.StatusItems.Add((Color.Yellow.ToVector4(), hoveredCellPosition.ToString()));
            }
        }

        Rectangle view = Surface.Surface.View;

        if (view.Height != Surface.Surface.Height)
        {
            ImGui.SameLine();

            int _sliderValueY = view.Position.Y;

            ImGui.BeginDisabled(Options.DisableScrolling);

            if (ImGuiExt.VSliderIntNudges("##height", new Vector2(barSize, imageSize.Y), ref _sliderValueY, Surface.Surface.Height - view.Height, 0, ImGuiSliderFlags.AlwaysClamp))
            {
                Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithY(_sliderValueY);
                tool?.DocumentViewChanged();
            }

            ImGui.EndDisabled();
        }

        if (view.Width != Surface.Surface.Width)
        {
            int _sliderValueX = view.Position.X;

            ImGui.BeginDisabled(Options.DisableScrolling);

            if (ImGuiExt.SliderIntNudges("##width", (int)imageSize.X, ref _sliderValueX, 0, Surface.Surface.Width - view.Width, bigX ? "BIG" : "%d", ImGuiSliderFlags.AlwaysClamp))
            {
                Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithX(_sliderValueX);
                tool?.DocumentViewChanged();
            }

            ImGui.EndDisabled();
        }

        ImGui.EndChild();
    }

}

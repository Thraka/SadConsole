using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using ImGuiInternal = ImGuiNET.Internal.ImGui;

namespace SadConsole.Editor.Model;

internal partial class SurfaceDocument : Document, IDocumentTools, IDocumentSurface
{
    private int _sliderValueY;
    private int _sliderValueX;

    public int Width = 80;
    public int Height = 25;

    public int ViewX;
    public int ViewY;
    public int ViewWidth;
    public int ViewHeight;

    public Vector4 DefaultForeground = Color.White.ToVector4();
    public Vector4 DefaultBackground = Color.Black.ToVector4();

    public IScreenSurface Surface;

    IScreenSurface IDocumentSurface.Surface => Surface;

    public SurfaceDocument()
    {
        DocumentType = Types.Surface;

        Options.UseToolsWindow = true;
        Options.ToolsWindowShowToolsList = true;

        ((IDocumentTools)this).ShowToolsList = true;
        ((IDocumentTools)this).State.ToolNames = new string[] { "Pencil", "Fill" };
        ((IDocumentTools)this).State.ToolObjects = new Tools.ITool[] { new Tools.Pencil(), new Tools.Fill() };
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
        ImGuiCore.State.LayoutInfo.ColorEditBoxWidth = ImGui.GetItemRectSize().X;

        ImGui.Text("Def. Background: ");
        ImGui.SameLine(windowWidth - paddingX - ImGuiCore.State.LayoutInfo.ColorEditBoxWidth);
        ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs);
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
            ImGui.Button("Resize");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Height: ");
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(Height.ToString());

            ImGui.EndTable();
        }

        ImGui.Separator();

        DefaultForeground = Surface.Surface.DefaultForeground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Foreground: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##fore", ref DefaultForeground, ImGuiColorEditFlags.NoInputs))
            Surface.Surface.DefaultForeground = DefaultForeground.ToColor();

        DefaultBackground = Surface.Surface.DefaultBackground.ToVector4();
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Def. Background: ");
        ImGui.SameLine();
        if (ImGui.ColorEdit4("##back", ref DefaultBackground, ImGuiColorEditFlags.NoInputs))
            Surface.Surface.DefaultBackground = DefaultBackground.ToColor();

        ImGuiWidgets.EndGroupPanel();
    }

    public override void BuildUIDocument(ImGuiRenderer renderer)
    {
        //ImGui.BeginTabBar("document_host_tabbar", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);

        //if (ImGui.BeginTabItem($"{Name}##{UniqueIdentifier}"))
        //{
        var topLeft = ImGui.GetCursorPos();
        if (ImGuiExt.ScrollableSurface("doc_surface", Surface, out Point hoveredCellPosition, renderer))
        {
            Tools.ITool tool = ((IDocumentTools)this).State.SelectedTool;
            if (tool != null)
            {
                tool.MouseOver(Surface, hoveredCellPosition, renderer);
            }
        }
            //ImGui.EndTabItem();
        //}

        //ImGui.EndTabBar();
    }

    public override void OnShow(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = Surface;
    }

    public override void OnHide(ImGuiRenderer renderer)
    {
        SadConsole.Game.Instance.Screen = null;
    }

    public override void Create()
    {
        Surface = new ScreenSurface(Width, Height);
        Surface.Surface.DefaultBackground = DefaultBackground.ToColor();
        Surface.Surface.DefaultForeground = DefaultForeground.ToColor();
        Surface.Surface.Clear();
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

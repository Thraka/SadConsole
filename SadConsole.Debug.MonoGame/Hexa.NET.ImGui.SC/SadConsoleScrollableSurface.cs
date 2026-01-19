using System;
using System.Numerics;
using SadConsole;
using SadConsole.ImGuiSystem;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static bool ScrollableSurface(string id, IScreenSurface Surface, out SadRogue.Primitives.Point hoveredCellPosition, ImGuiRenderer renderer)
    {
        Vector2 region = ImGui.GetContentRegionAvail();
        Vector2 imageSize = new Vector2(Surface.Renderer.Output.Width, Surface.Renderer.Output.Height);
        int barSize = 15;
        Vector2 padding = ImGui.GetStyle().FramePadding;
        bool bigX = false;
        bool returnValue = false;


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

        hoveredCellPosition = new SadRogue.Primitives.Point();

        if (ImGui.BeginChild(id))
        {
            //isActive = ImGui.IsItemActive();

            //if (idother != 0 && idvalue != idother) System.Diagnostics.Debugger.Break();

            DrawTexture("output_preview_surface1", true, ImGuiSC.ZoomNormal, ((SadConsole.Host.GameTexture)Surface.Renderer.Output).Texture, imageSize, renderer, out var isActive, out var isHovered);

            Vector2 mousePosition = ImGui.GetMousePos();
            Vector2 pos = mousePosition - ImGui.GetItemRectMin();
            if (Surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
            {
                if (isActive && isHovered)
                {
                    hoveredCellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(Surface.FontSize) + Surface.Surface.ViewPosition;
                    returnValue = true;
                }
            }

            SadRogue.Primitives.Rectangle view = Surface.Surface.View;

            if (view.Height != Surface.Surface.Height)
            {
                ImGui.SameLine();

                int _sliderValueY = view.Position.Y;

                if (ImGuiSC.VSliderIntNudges("##height", new Vector2(barSize, imageSize.Y), ref _sliderValueY, Surface.Surface.Height - view.Height, 0, ImGuiSliderFlags.AlwaysClamp))
                    Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithY(_sliderValueY);
            }

            if (view.Width != Surface.Surface.Width)
            {
                int _sliderValueX = view.Position.X;

                if (ImGuiSC.SliderIntNudges("##width", (int)imageSize.X, ref _sliderValueX, 0, Surface.Surface.Width - view.Width, bigX ? "BIG" : "%d", ImGuiSliderFlags.AlwaysClamp))
                    Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithX(_sliderValueX);
            }
        }
        ImGui.EndChild();

        return returnValue;
    }
}

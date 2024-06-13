using System.Numerics;
using ImGuiNET;

namespace SadConsole.Editor.GuiParts.Tools;

internal static class ToolHelpers
{
    public static Vector2 TransformCellCenter(Point cellPosition, Point viewOffset, Point fontSize)
    {
        Vector2 topleft = ImGui.GetItemRectMin();

        Point pixelCellPosition = (cellPosition - viewOffset) * fontSize;

        return new Vector2(topleft.X + pixelCellPosition.X + (fontSize.X / 2), topleft.Y + pixelCellPosition.Y + (fontSize.Y / 2));
    }

    public static Vector2 TransformMouseCellCenter(Vector2 imGuiMouse, Point fontSize)
    {
        Vector2 topleft = ImGui.GetItemRectMin();
        imGuiMouse = imGuiMouse - topleft;
        imGuiMouse = new(imGuiMouse.X / fontSize.X, imGuiMouse.Y / fontSize.Y);

        Point pixelCellPosition = new((int)imGuiMouse.X * fontSize.X, (int)imGuiMouse.Y * fontSize.Y);

        return new Vector2(topleft.X + pixelCellPosition.X + (fontSize.X / 2), topleft.Y + pixelCellPosition.Y + (fontSize.Y / 2));
    }

    public static void HighlightCell(Point cellPosition, Point viewOffset, Point fontSize, Color color)
    {
        Vector2 topleft = ImGui.GetItemRectMin();

        Point pixelCellPosition = (cellPosition - viewOffset) * fontSize;

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 boxTopLeft = new(topleft.X + pixelCellPosition.X, topleft.Y + pixelCellPosition.Y);
        Vector2 boxBottomRight = boxTopLeft + new Vector2(fontSize.X, fontSize.Y);

        drawList.AddRect(boxTopLeft, boxBottomRight, color.PackedValue);
    }
}

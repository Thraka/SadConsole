using System.Numerics;
using Hexa.NET.ImGui;

namespace SadConsole.Editor.Tools;

internal static class ToolHelpers
{
    public static Vector2 TransformCellCenter(Point cellPosition, Point viewOffset, Point fontSize)
    {
        Vector2 topLeft = ImGui.GetItemRectMin();

        Point pixelCellPosition = (cellPosition - viewOffset) * fontSize;

        return new Vector2(topLeft.X + pixelCellPosition.X + (fontSize.X / 2), topLeft.Y + pixelCellPosition.Y + (fontSize.Y / 2));
    }

    public static Vector2 TransformMouseCellCenter(Vector2 imGuiMouse, Point fontSize)
    {
        Vector2 topLeft = ImGui.GetItemRectMin();
        imGuiMouse = imGuiMouse - topLeft;
        imGuiMouse = new(imGuiMouse.X / fontSize.X, imGuiMouse.Y / fontSize.Y);

        Point pixelCellPosition = new((int)imGuiMouse.X * fontSize.X, (int)imGuiMouse.Y * fontSize.Y);

        return new Vector2(topLeft.X + pixelCellPosition.X + (fontSize.X / 2), topLeft.Y + pixelCellPosition.Y + (fontSize.Y / 2));
    }

    public static void HighlightCell(Point cellPosition, Point viewOffset, Point fontSize, Color color)
    {
        Vector2 topLeft = ImGui.GetItemRectMin();

        Point pixelCellPosition = (cellPosition - viewOffset) * fontSize;

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();

        Vector2 boxTopLeft = new(topLeft.X + pixelCellPosition.X, topLeft.Y + pixelCellPosition.Y);
        Vector2 boxBottomRight = boxTopLeft + new Vector2(fontSize.X, fontSize.Y);

        drawList.AddRect(boxTopLeft, boxBottomRight, color.PackedValue);
    }
}

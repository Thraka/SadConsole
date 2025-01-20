using System.Numerics;
using SadRogue.Primitives;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static void SeparatorText(string label, Vector4 color)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.SeparatorText(label);
        ImGui.PopStyleColor();
    }

    public static void SeparatorLabel(string text)
    {
        Color color = ImGui.GetStyle().Colors[(int)ImGuiCol.Text].ToColor();
        SeparatorLabel(text, color, color);
    }

    public static void SeparatorLabel(string text, Vector4 barColor) =>
        SeparatorLabel(text, barColor.ToColor(), ImGui.GetStyle().Colors[(int)ImGuiCol.Text].ToColor());

    public static void SeparatorLabel(string text, Color barColor) =>
        SeparatorLabel(text, barColor, ImGui.GetStyle().Colors[(int)ImGuiCol.Text].ToColor());

    public static void SeparatorLabel(string text, Vector4 barColor, Vector4 textColor) =>
        SeparatorLabel(text, barColor.ToColor(), textColor.ToColor());

    public static void SeparatorLabel(string text, Color barColor, Color textColor)
    {
        Vector2 spacing = ImGui.GetStyle().ItemSpacing;
        Vector2 padding = ImGui.GetStyle().FramePadding;
        
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 pos = ImGui.GetCursorScreenPos();
        Vector2 textSize = ImGui.CalcTextSize(text);
        float height = textSize.Y / 4;
        float top = textSize.Y / 2 - height / 2 + 1f;
        Vector2 rectRegionStart = new(pos.X - padding.X, pos.Y + top);
        ImGui.Indent();
        pos = ImGui.GetCursorScreenPos();
        Vector2 rectRegionEnd = new(pos.X - spacing.X / 2, pos.Y + top + height);
        drawList.AddRectFilled(rectRegionStart, rectRegionEnd, barColor.PackedValue);
        float width = ImGui.GetContentRegionAvail().X;
        ImGui.TextColored(textColor.ToVector4(), text);

        ImGui.Unindent();

        rectRegionStart = pos + textSize;
        rectRegionStart.X += spacing.X / 2;
        rectRegionStart.Y = pos.Y + top;
        width -= textSize.X;
        rectRegionEnd = rectRegionStart + new Vector2(width, height);
        drawList.AddRectFilled(rectRegionStart, rectRegionEnd, barColor.PackedValue);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
    }

}

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static void TextWrappedDisabled(string text)
    {
        unsafe
        {
            ImGui.PushStyleColor(ImGuiCol.Text, *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled));
            ImGui.TextWrapped(text);
            ImGui.PopStyleColor();
        }
    }

    public static void TextWrappedDisabled(ReadOnlySpan<byte> text)
    {
        unsafe
        {
            ImGui.PushStyleColor(ImGuiCol.Text, *ImGui.GetStyleColorVec4(ImGuiCol.TextDisabled));
            ImGui.TextWrapped(text);
            ImGui.PopStyleColor();
        }
    }
}

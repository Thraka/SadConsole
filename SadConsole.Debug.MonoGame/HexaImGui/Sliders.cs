using System;
using System.Numerics;

namespace Hexa.NET.ImGui;

public static partial class ImGui2
{
    public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max) =>
        VSliderIntNudges(label, size, ref v, v_min, v_max, "%d", ImGuiSliderFlags.None);

    public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max, ImGuiSliderFlags flags) =>
        VSliderIntNudges(label, size, ref v, v_min, v_max, "%d", flags);

    public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max, string fmt, ImGuiSliderFlags flags)
    {
        bool returnValue = false;

        // Calculate button size, taken from widgets.cpp in imgui lib
        var style = ImGui.GetStyle();
        Vector2 label_size = ImGui.CalcTextSize("+", true) + (style.FramePadding * 2f);
        ImGui.BeginChild(label, size);

        if (ImGui.Button($"-", new Vector2(size.X, label_size.Y)))
        {
            returnValue = true;
            v = Math.Clamp(v - 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
        }

        returnValue = returnValue | ImGui.VSliderInt("##vslider", new Vector2(size.X, size.Y - (style.FramePadding.Y * 4f) - (label_size.Y * 2)), ref v, v_min, v_max, fmt, flags);

        if (ImGui.Button("+", new Vector2(size.X, label_size.Y)))
        {
            returnValue = true;
            v = Math.Clamp(v + 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
        }

        ImGui.EndChild();

        return returnValue;
    }

    public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max) =>
        SliderIntNudges(label, width, ref v, v_min, v_max, "%d", ImGuiSliderFlags.None);

    public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max, ImGuiSliderFlags flags) =>
        SliderIntNudges(label, width, ref v, v_min, v_max, "%d", flags);

    public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max, string fmt, ImGuiSliderFlags flags)
    {
        bool returnValue = false;

        // Calculate button size, taken from widgets.cpp in imgui lib
        var style = ImGui.GetStyle();

        ImGui.BeginChild(label, new Vector2(width, 0));

        if (ImGui.Button("-"))
        {
            returnValue = true;
            v = Math.Clamp(v - 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
        }

        ImGui.SameLine();

        var buttonWidth = ImGui.GetCursorPosX();

        ImGui.SetNextItemWidth(width - (buttonWidth * 2));

        returnValue = returnValue | ImGui.SliderInt("##width", ref v, v_min, v_max, fmt, flags);

        ImGui.SameLine();

        if (ImGui.Button("+", new Vector2()))
        {
            returnValue = true;
            v = Math.Clamp(v + 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
        }

        ImGui.EndChild();

        return returnValue;
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using ImRect = ImGuiNET.Internal.ImRect;
using ImGuiInternal = ImGuiNET.Internal.ImGui;

namespace ImGuiNET;

public static partial class ImGuiWidgets
{
    // Taken from https://github.com/ocornut/imgui/issues/1496#issuecomment-655048353

    private static Stack<ImRect> _stack = new Stack<ImRect>(2);

    public static void BeginGroupPanel(string name) =>
        BeginGroupPanel(name, Vector2.Zero);

    public static void BeginGroupPanel(string name, Vector2 size)
    {
        ImGui.BeginGroup();

        var cursorPos = ImGui.GetCursorScreenPos();
        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        var framePadding = ImGui.GetStyle().FramePadding;
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0.0f, 0.0f));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 0.0f));

        var frameHeight = ImGui.GetFrameHeight();
        ImGui.BeginGroup();

        Vector2 effectiveSize = size;
        if (size.X <= 0.0f)
            effectiveSize.X = ImGui.GetContentRegionAvail().X;
        else
            effectiveSize.X = size.X;

        ImGui.Dummy(new Vector2(effectiveSize.X, 0.0f));
        ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
        ImGui.SameLine(0.0f, 0.0f);
        ImGui.BeginGroup();
        ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
        ImGui.SameLine(0.0f, 0.0f);
        ImGui.TextUnformatted(name);
        var labelMin = ImGui.GetItemRectMin();
        var labelMax = ImGui.GetItemRectMax();
        ImGui.SameLine(0.0f, 0.0f);
        ImGui.Dummy(new Vector2(0.0f, frameHeight + itemSpacing.Y));
        ImGui.BeginGroup();

        //ImGui.GetWindowDrawList()->AddRect(labelMin, labelMax, IM_COL32(255, 0, 255, 255));

        ImGui.PopStyleVar(2);

        var window = ImGuiInternal.GetCurrentWindow();

        window.ContentRegionRect.Max.X -= frameHeight * 0.5f;
        window.WorkRect.Max.X -= frameHeight * 0.5f;
        window.InnerRect.Max.X -= frameHeight * 0.5f;
        window.Size.X -= frameHeight;

        var itemWidth = ImGui.CalcItemWidth();
        ImGui.PushItemWidth(MathF.Max(0.0f, itemWidth));

        _stack.Push(new ImRect() {  Min = labelMin, Max = labelMax });
    }

    public unsafe static void EndGroupPanel()
    {
        ImGui.PopItemWidth();

        var itemSpacing = ImGui.GetStyle().ItemSpacing;

        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0.0f, 0.0f));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 0.0f));

        var frameHeight = ImGui.GetFrameHeight();

        ImGui.EndGroup();

        //ImGui.GetWindowDrawList()->AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), IM_COL32(0, 255, 0, 64), 4.0f);

        ImGui.EndGroup();

        ImGui.SameLine(0.0f, 0.0f);
        ImGui.Dummy(new Vector2(frameHeight * 0.5f, 0.0f));
        ImGui.Dummy(new Vector2(0.0f, frameHeight - frameHeight * 0.5f));

        ImGui.EndGroup();

        var itemMin = ImGui.GetItemRectMin();
        var itemMax = ImGui.GetItemRectMax();
        //ImGui.GetWindowDrawList()->AddRectFilled(itemMin, itemMax, IM_COL32(255, 0, 0, 64), 4.0f);

        var labelRect = _stack.Pop();

        Vector2 halfFrame = new Vector2(frameHeight * 0.25f, frameHeight) * 0.5f;
        ImRect frameRect = new ImRect() { Min = itemMin + halfFrame, Max = itemMax - new Vector2(halfFrame.X, 0.0f) };
        labelRect.Min.X -= itemSpacing.X;
        labelRect.Max.X += itemSpacing.X;
        for (int i = 0; i < 4; ++i)
        {
            switch (i)
            {
                // left half-plane
                case 0: ImGui.PushClipRect(new Vector2(-float.MaxValue, -float.MaxValue), new Vector2(labelRect.Min.X, float.MaxValue), true); break;
                // right half-plane
                case 1: ImGui.PushClipRect(new Vector2(labelRect.Max.X, -float.MaxValue), new Vector2(float.MaxValue, float.MaxValue), true); break;
                // top
                case 2: ImGui.PushClipRect(new Vector2(labelRect.Min.X, -float.MaxValue), new Vector2(labelRect.Max.X, labelRect.Min.Y), true); break;
                // bottom
                case 3: ImGui.PushClipRect(new Vector2(labelRect.Min.X, labelRect.Max.Y), new Vector2(labelRect.Max.X, float.MaxValue), true); break;
            }

            ImGui.GetWindowDrawList().AddRect(
                frameRect.Min, frameRect.Max,
                (*ImGui.GetStyleColorVec4(ImGuiCol.Border)).ToColor().PackedValue,
                halfFrame.X);

            ImGui.PopClipRect();
        }

        ImGui.PopStyleVar(2);

        var window = ImGuiInternal.GetCurrentWindow();
        window.ContentRegionRect.Max.X += frameHeight * 0.5f;
        window.WorkRect.Max.X += frameHeight * 0.5f;
        window.InnerRect.Max.X += frameHeight * 0.5f;
        window.Size.X += frameHeight;

        ImGui.Dummy(new Vector2(0.0f, 0.0f));

        ImGui.EndGroup();
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ImGuiNET
{
    public static partial class ImGuiEx
    {
        // Taken from https://github.com/ocornut/imgui/issues/1496#issuecomment-655048353
        private static void BeginGroupPanel(string name, Vector2 size)
        {
            ImGui.BeginGroup();
            var cursorPos = ImGui.GetCursorScreenPos();
            var itemSpacing = ImGui.GetStyle().ItemSpacing;

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, 0f);

            var frameHeight = ImGui.GetFrameHeight();
            ImGui.BeginGroup();

            Vector2 effectiveSize = size;
            if (size.X < 0.0f)
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

            ImGui.PopStyleVar(2);


        }
    }
}

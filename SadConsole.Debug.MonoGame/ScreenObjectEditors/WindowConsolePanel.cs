﻿using System.Numerics;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame.ScreenObjectEditors;

internal class WindowConsolePanel : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Window!!!!");
        //ImGui.SameLine();
        //ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        //if (ImGui.ColorEdit4("##tint", ref GuiState._selectedScreenObjectState.Tint))
        //{
        //    ((IScreenSurface)GuiState._selectedScreenObjectState.Object).Tint = GuiState._selectedScreenObjectState.Tint.ToColor();
        //    GuiState._selectedScreenObjectState.Refresh();
        //}
    }
}

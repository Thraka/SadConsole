using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.SadComponentEditors;

internal class WindowConsolePanel : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        //ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Window!!!!");
        //ImGui.SameLine();
        //ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        //if (ImGui.ColorEdit4("##tint", ref GuiState._selectedScreenObjectState.Tint))
        //{
        //    ((IScreenSurface)GuiState._selectedScreenObjectState.Object).Tint = GuiState._selectedScreenObjectState.Tint.ToColor();
        //    GuiState._selectedScreenObjectState.Refresh();
        //}
    }
}

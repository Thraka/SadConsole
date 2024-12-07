using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.Editors;

internal class WindowConsolePanel : IScreenObjectPanel
{
    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        var window = (UI.Window)state.Object;

        ImGui2.SeparatorText("Window Settings", Debugger.Settings.Color_PanelHeader);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        window.Title = ImGui2.InputText("##window_title", window.Title);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title Alignment: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.CalcTextSize("Stretch").X * 2 + ImGui.GetStyle().FramePadding.X * 2.0f);
        if (ImGui.Combo("##window_title_alignment", ref state.WindowState.TitleAlignment,
                Enums<HorizontalAlignment>.Names,
                Enums<HorizontalAlignment>.Count))
        {
            window.TitleAlignment = (HorizontalAlignment)state.WindowState.TitleAlignment;
        }
    }
}

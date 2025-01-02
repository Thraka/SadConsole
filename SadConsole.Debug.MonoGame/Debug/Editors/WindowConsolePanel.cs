using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

internal class WindowConsolePanel : IScreenObjectPanel
{
    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        var window = (UI.Window)state.Object;

        ImGuiSC.SeparatorText("Window Settings", Debugger.Settings.Color_PanelHeader);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        window.Title = ImGuiSC.InputText("##window_title", window.Title);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title Alignment: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.CalcTextSize("Stretch").X * 2 + ImGui.GetStyle().FramePadding.X * 2.0f);
        if (ImGui.Combo("##window_title_alignment", ref state.WindowState.TitleAlignment,
                ImGuiListEnum<HorizontalAlignment>.Names,
                ImGuiListEnum<HorizontalAlignment>.Count))
        {
            window.TitleAlignment = (HorizontalAlignment)state.WindowState.TitleAlignment;
        }
    }
}

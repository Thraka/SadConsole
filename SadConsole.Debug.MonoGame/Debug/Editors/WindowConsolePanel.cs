using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

public class ScreenObjectEditorWindowConsole : IScreenObjectPanel
{
    public void BuildTabItem(ImGuiRenderer renderer, ScreenObjectState state)
    {
        var window = (UI.Window)state.Object;

        if (ImGui.BeginTabItem("Window", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
        {
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

            ImGui.EndTabItem();
        }
    }
}

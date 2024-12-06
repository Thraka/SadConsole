using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.ScreenObjectEditors;

internal class WindowConsolePanel : IDetailsPanel
{
    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState CurrentScreenObject)
    {
        var window = (UI.Window)CurrentScreenObject.Object;

        ImGui.SeparatorText("Window Settings");

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        window.Title = ImGui2.InputText("##window_title", window.Title);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Title Alignment: ");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.CalcTextSize("Stretch").X * 2 + ImGui.GetStyle().FramePadding.X * 2.0f);
        if (ImGui.Combo("##window_title_alignment", ref CurrentScreenObject.WindowState.TitleAlignment,
                Enums<HorizontalAlignment>.Names,
                Enums<HorizontalAlignment>.Count))
        {
            window.TitleAlignment = (HorizontalAlignment)CurrentScreenObject.WindowState.TitleAlignment;
        }
    }
}

using ImGuiNET;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public static class ResizeSurfacePopup
{
    public static int MaxWidth = 2000;
    public static int MaxHeight = 2000;

    public static bool Show(string popupId, ref int width, ref int height, out bool dialogResult)
    {
        bool returnValue = false;
        dialogResult = false;

        ImGuiCore.State.CheckSetPopupOpen(popupId);
        if (ImGui.BeginPopup(popupId))
        {
            SettingsTable.BeginTable("resize_surface_table");

            SettingsTable.DrawInt("Width:", "width", ref width, 1, MaxWidth);
            SettingsTable.DrawInt("Height:", "height", ref height, 1, MaxHeight);

            SettingsTable.EndTable();

            if (ImGuiWindow.DrawButtons(out dialogResult))
            {
                returnValue = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        return returnValue;
    }
}

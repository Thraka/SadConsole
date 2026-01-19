using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
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

        ImGui.SetNextWindowSize(new System.Numerics.Vector2(200, -1));
        if (ImGui.BeginPopup(popupId))
        {
            if (SettingsTable.BeginTable("resize_surface_table", column1Flags: ImGuiTableColumnFlags.WidthFixed))
            {
                SettingsTable.DrawInt("Width:", "##width", ref width, 1, MaxWidth);
                SettingsTable.DrawInt("Height:", "##height", ref height, 1, MaxHeight);

                SettingsTable.EndTable();
            }

            if (ImGuiWindowBase.DrawButtons(out dialogResult))
            {
                returnValue = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        return returnValue;
    }
}

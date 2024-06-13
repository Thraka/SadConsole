using ImGuiNET;

namespace SadConsole.Editor.GuiParts.Tools;

internal static partial class SettingsTable
{
    public static void BeginTable(string id)
    {
        if (ImGui.BeginTable(id, 2))
        {
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("two");
        }
    }

    public static void EndTable()
    {
        ImGui.EndTable();
    }
}

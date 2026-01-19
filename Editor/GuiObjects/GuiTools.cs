using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;
public class GuiToolsList: ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (!Core.State.HasSelectedDocument || !Core.State.SelectedDocument!.Options.UseToolsWindow) return;

        ImGui.Begin(GuiDockSpace.ID_RIGHT_PANEL);
        if (Core.State.SelectedDocument!.Options.ToolsWindowShowToolsList)
        {
            ImGuiGuardedValue<int> _itemIndex = new(Core.State.Tools.SelectedItemIndex);
            ImGui.SetNextItemWidth(-1);
            if (ImGui.ListBox("##tools", ref _itemIndex.CurrentValue,
                    Core.State.Tools.Names, Core.State.Tools.Count, Core.State.Tools.Count))
            {
                if (_itemIndex.IsChanged() && _itemIndex.OriginalValue != -1)
                    Core.State.Tools.Objects[_itemIndex.OriginalValue].OnDeselected(Core.State.SelectedDocument);

                Core.State.Tools.Objects[_itemIndex.CurrentValue].OnSelected(Core.State.SelectedDocument);

                Core.State.Tools.SelectedItemIndex = _itemIndex.CurrentValue;
            }

            if (Core.State.Tools.IsItemSelected())
            {
                if (ImGui.BeginChild("settings_panel"u8))
                {
                    ImGui.SeparatorText(Core.State.Tools.SelectedItem.Title);

                    Core.State.Tools.SelectedItem.BuildSettingsPanel(Core.State.SelectedDocument);

                }
                ImGui.EndChild();
            }
        }

        Core.State.SelectedDocument.ImGuiDrawInToolsPanel(renderer);

        ImGui.End();
    }
}

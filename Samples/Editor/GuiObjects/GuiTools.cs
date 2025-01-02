using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;
public class GuiToolsList: ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (!Core.State.Documents.IsItemSelected() || !Core.State.Documents.SelectedItem.Options.UseToolsWindow) return;

        ImGui.Begin(GuiDockSpace.ID_RIGHT_PANEL);
        ImGuiGuardedValue<int> _itemIndex = new(Core.State.Tools.SelectedItemIndex);
        ImGui.SetNextItemWidth(-1);
        if (ImGui.ListBox("##tools", ref _itemIndex.CurrentValue,
                Core.State.Tools.Names, Core.State.Tools.Count, 8))
        {
            if (_itemIndex.IsChanged() && _itemIndex.OriginalValue != -1)
                Core.State.Tools.Objects[_itemIndex.OriginalValue].OnDeselected(Core.State.Documents.SelectedItem);

            Core.State.Tools.Objects[_itemIndex.CurrentValue].OnSelected(Core.State.Documents.SelectedItem);

            Core.State.Tools.SelectedItemIndex = _itemIndex.CurrentValue;
        }

        if (Core.State.Tools.IsItemSelected())
        {
            Core.State.Tools.SelectedItem.BuildSettingsPanel(Core.State.Documents.SelectedItem);
        }

        ImGui.End();
    }
}

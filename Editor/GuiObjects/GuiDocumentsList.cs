using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;
public class GuiDocumentsList: ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin(GuiDockSpace.ID_LEFT_PANEL);
        ImGui.SetNextItemWidth(-1);

        Documents.Document? oldDocument = Core.State.Documents.SelectedItem;

        if (ImGui.ListBox("##docs", ref Core.State.Documents.SelectedItemIndex,
                                    Core.State.Documents.Names, Core.State.Documents.Count, 5))
        {
            if (oldDocument != null && oldDocument != Core.State.Documents.SelectedItem)
            {
                oldDocument.OnDeselected();
                Core.State.Tools.Objects.Clear();
            }

            if (Core.State.Documents.IsItemSelected())
            {
                Core.State.Documents.SelectedItem.OnSelected();
            }
        }

        if (Core.State.Documents.IsItemSelected())
        {
            Core.State.Documents.SelectedItem.BuildUiDocumentSettings(renderer);
        }

        ImGui.End();
    }
}

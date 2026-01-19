using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class NewDocument : ImGuiWindowBase
{
    public NewDocument()
    {
        Title = "New file";
    }

    public void Show()
    {
        IsOpen = true;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }

    protected override void OnClosed()
    {
        if (RemoveOnClose)
            ImGuiCore.GuiComponents.Remove(this);

        if (DialogResult)
        {
            if (Core.State.DocumentBuilders.IsItemSelected())
                Core.State.Documents.Add(Core.State.DocumentBuilders.SelectedItem.CreateDocument());
        }

        Core.State.DocumentBuilders.SelectedItem = null;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                bool isDocValid = false;

                ImGui.Text("Document Type:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##doctypes", ref Core.State.DocumentBuilders.SelectedItemIndex, Core.State.DocumentBuilders.Names,
                                  Core.State.DocumentBuilders.Count))
                {
                    Core.State.DocumentBuilders.SelectedItem!.ResetBuilder();
                }

                if (Core.State.DocumentBuilders.IsItemSelected())
                {
                    ImGui.Separator();
                    Core.State.DocumentBuilders.SelectedItem.ImGuiNewDocument(renderer);
                    isDocValid = Core.State.DocumentBuilders.SelectedItem.IsDocumentValid();
                }

                ImGui.Separator();

                if (DrawButtons(out DialogResult, !isDocValid, acceptButtonText: "Create"))
                    Close();

                ImGui.EndPopup();
            }
        }
    }
}

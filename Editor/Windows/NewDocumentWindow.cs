using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class NewDocumentWindow
{
    public static void Show(ImGuiRenderer renderer)
    {
        Instance instance = new();
        renderer.UIObjects.Add(instance);
    }

    private class Instance : ImGuiObjectBase
    {
        private bool _firstShow = true;

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("New file"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));

            if (ImGui.BeginPopupModal("New file"u8, ImGuiWindowFlags.NoResize))
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

                bool dialogResult;
                if (ImGuiSC.WindowDrawButtons(out dialogResult, !isDocValid, acceptButtonText: "Create"))
                {
                    if (dialogResult && Core.State.DocumentBuilders.IsItemSelected())
                        Core.State.Documents.Add(Core.State.DocumentBuilders.SelectedItem.CreateDocument());

                    Core.State.DocumentBuilders.SelectedItem = null;
                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                }
                ImGui.EndPopup();
            }
        }
    }
}

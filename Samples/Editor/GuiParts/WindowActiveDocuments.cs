using System.Numerics;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts;

public class WindowActiveDocuments : ImGuiObjectBase
{
    private Model.Document _selectedDocument = null;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin("Active Documents");
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGuiCore.State.OpenDocuments.Length > 0)
            {
                ImGui.ListBox("##documentslist", ref ImGuiCore.State.SelectedDocumentIndex, ImGuiCore.State.OpenDocumentTitles, ImGuiCore.State.OpenDocuments.Length, ImGuiCore.State.OpenDocuments.Length <= 4 ? 4 : 6);

                // Some document is selected
                if (ImGuiCore.State.SelectedDocumentIndex != -1)
                {
                    if (_selectedDocument != ImGuiCore.State.OpenDocuments[ImGuiCore.State.SelectedDocumentIndex])
                    {
                        _selectedDocument?.OnHide(renderer);
                        _selectedDocument = ImGuiCore.State.OpenDocuments[ImGuiCore.State.SelectedDocumentIndex];
                        _selectedDocument.OnShow(renderer);
                    }

                    _selectedDocument?.BuildUIEdit(renderer, true);
                }
            }
            else
            {
                ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No documents loaded");
            }
        }
        ImGui.End();
    }
}

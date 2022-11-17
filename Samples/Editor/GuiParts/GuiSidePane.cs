using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.GuiParts;

public class GuiSidePane : ImGuiObjectBase
{
    private int _selectedDocIndex = -1;
    private Model.Document _selectedDocument = null;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.SetNextWindowSize(new Vector2(300, ImGui.GetIO().DisplaySize.Y - ImGui.GetFrameHeight()), ImGuiCond.Always);
        ImGui.SetNextWindowPos(new Vector2(0, ImGui.GetFrameHeight()));
        ImGui.Begin("##side_pane", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking);
        {
            ImGui.Text("Documents");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGuiCore.State.OpenDocuments.Length > 0)
            {
                if (ImGui.ListBox("##documentslist", ref _selectedDocIndex, ImGuiCore.State.OpenDocumentTitles, ImGuiCore.State.OpenDocuments.Length))
                {
                    bool sameDocument = false;

                    ImGuiCore.State.SelectedDocumentIndex = _selectedDocIndex;

                    if (_selectedDocument != null)
                    {
                        if (_selectedDocument == ImGuiCore.State.OpenDocuments[_selectedDocIndex])
                            sameDocument = true;
                        else
                            _selectedDocument.OnHide(renderer);
                    }

                    if (!sameDocument)
                    {
                        _selectedDocument = ImGuiCore.State.OpenDocuments[_selectedDocIndex];
                        _selectedDocument.OnShow(renderer);
                    }
                }

                if (_selectedDocIndex != -1)
                {
                    ImGui.Separator();
                    _selectedDocument.BuildUIEdit(renderer, true);
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

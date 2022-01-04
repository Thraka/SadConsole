using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts
{
    public class GuiSidePane : ImGuiObjectBase
    {
        private int _selectedDocIndex;

        public override void BuildUI(ImGuiRenderer renderer)
        {
            ImGui.SetNextWindowSize(new Vector2(200, ImGui.GetIO().DisplaySize.Y / 3), ImGuiCond.Appearing);
            
            ImGui.Begin("Documents", ImGuiWindowFlags.NoCollapse);
            {
                
                ImGui.ListBox("##documentslist", ref _selectedDocIndex, ImGuiCore.State.OpenDocumentTitles, ImGuiCore.State.OpenDocuments.Length);

                if (ImGuiCore.State.OpenDocuments.Length > 0)
                {
                    ImGuiCore.State.OpenDocuments[ImGuiCore.State.SelectedDocumentIndex].Settings.BuildUIEdit(renderer, true);
                }
            }
            ImGui.End();
        }
    }
}

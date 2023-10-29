using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.GuiParts;

public class WindowTools : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin("Tools");

        if (ImGuiCore.State.SelectedDocumentIndex != -1)
        {
            var doc = ImGuiCore.State.GetOpenDocument();

            // If the document supports the tools window
            if (doc is IDocumentTools docTools)
            {
                // Display the default tools list
                if (docTools.ShowToolsList)
                {
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    if (docTools.State.ToolObjects.Length != 0)
                    {
                        ImGui.ListBox("##toolsList", ref docTools.State.SelectedToolIndex, docTools.State.ToolNames, docTools.State.ToolObjects.Length, docTools.State.ToolObjects.Length <= 4 ? 4 : 10);
                        ImGui.Separator();
                        docTools.State.SelectedTool.BuildSettingsPanel(renderer);
                    }
                    else
                    {
                        docTools.State.SelectedToolIndex = -1;
                        ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No tools associated with this document");
                    }
                }

                // Build any custom rendering afterwards
                docTools.BuildUI(renderer);
            }
        }
        else
        {
            ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No document selected");
        }


        ImGui.End();
    }
}

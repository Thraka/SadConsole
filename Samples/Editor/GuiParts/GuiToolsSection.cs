using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Editor.GuiParts;

public class GuiToolsSection : ImGuiObjectBase
{
    private int _selectedToolIndex;

    public string[] ToolNames;
    public Tools.ITool[] ToolObjects;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Separator();
        ImGui.PushID("tools_panel");

        ImGui.Text("Tools");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ToolObjects.Length > 0)
        {
            ImGui.ListBox("##documentslist", ref _selectedToolIndex, ToolNames, ToolNames.Length, 10);
            ImGui.Separator();
            ToolObjects[_selectedToolIndex].BuildSettingsPanel(renderer);
        }
        else
        {
            ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No tools associated with this document");
        }

        ImGui.PopID();
    }
}

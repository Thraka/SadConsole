
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts;

public class GuiDocumentsHost : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.SetNextWindowSize(new Vector2(ImGui.GetIO().DisplaySize.X - 300, ImGui.GetIO().DisplaySize.Y - ImGui.GetFrameHeight()), ImGuiCond.Always);
        ImGui.SetNextWindowPos(new Vector2(300, ImGui.GetFrameHeight()));

        ImGui.Begin("##document_host", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDocking);

        if (ImGuiCore.State.SelectedDocumentIndex != -1)
        {
            ImGuiCore.State.OpenDocuments[ImGuiCore.State.SelectedDocumentIndex].BuildUIDocument(renderer);
        }
        ImGui.End();
    }
}

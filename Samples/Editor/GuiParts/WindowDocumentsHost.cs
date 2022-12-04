
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts;

public class WindowDocumentsHost : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.SetNextWindowClass(ImGuiCore.State.NoTabBarWindowClass);
        ImGui.Begin("Open Document");

        if (ImGuiCore.State.SelectedDocumentIndex != -1)
            ImGuiCore.State.OpenDocuments[ImGuiCore.State.SelectedDocumentIndex].BuildUIDocument(renderer);

        ImGui.End();
    }
}

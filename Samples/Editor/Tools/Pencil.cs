using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;
internal class Pencil : ITool
{
    public string Name => throw new NotImplementedException();

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGui.Text("You're using the pencil!");
    }

    public void MouseOver(ImGuiRenderer renderer)
    {

    }
}

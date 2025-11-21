using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;

public class GuiFinalDrawDocument : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (Core.State.Documents.IsItemSelected())
            Core.State.Documents.SelectedItem.Redraw(false, false);
    }
}

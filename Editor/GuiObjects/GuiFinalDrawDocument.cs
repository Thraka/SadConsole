using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;

public class GuiFinalDrawDocument : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (Core.State.HasSelectedDocument)
        {
            Core.State.SelectedDocument!.Redraw(false, false);
            Core.State.SelectedDocument.ImGuiDrawAfterEverything(renderer);
        }
    }
}

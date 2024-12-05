using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

class GuiTopBar : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.MenuItem("Close", "c"))
                renderer.HideRequested = true;

            ImGui.EndMainMenuBar();
        }
    }
}

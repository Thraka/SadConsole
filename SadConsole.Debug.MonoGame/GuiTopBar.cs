using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

class GuiTopBar : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Close", "c"))
                    renderer.HideRequested = true;

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Options"))
            {
                //if (ImGui.MenuItem("Show SadConsole game", "s", ref GuiState.ShowSadConsoleRendering, true))
                //    GuiState.RaiseShowSadConsoleRenderingChanged();

                ImGui.MenuItem("Show surface preview", "p", ref GuiState.ShowSurfacePreview, true);
                ImGui.MenuItem("Show final output", "o", ref GuiState.GuiFinalOutputWindow.IsOpen, true);
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}

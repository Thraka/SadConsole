using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;
using Hexa.NET.ImGui;
using System;

namespace SadConsole.Debug;

public class GuiTopBar : ImGuiObjectBase
{
    /// <summary>
    /// Occurs when a request to close the component is made.
    /// </summary>
    public event Action? Closed;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.MenuItem("Close", "c"))
                Closed?.Invoke();

            ImGui.EndMainMenuBar();
        }
    }
}

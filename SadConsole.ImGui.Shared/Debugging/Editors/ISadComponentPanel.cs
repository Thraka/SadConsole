using SadConsole.Components;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Debug.Editors;

public interface ISadComponentPanel
{
    void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component);
}

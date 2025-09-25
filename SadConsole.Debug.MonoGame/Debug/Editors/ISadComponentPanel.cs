using SadConsole.ImGuiSystem;
using SadConsole.Components;

namespace SadConsole.Debug.Editors;

public interface ISadComponentPanel
{
    void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component);
}

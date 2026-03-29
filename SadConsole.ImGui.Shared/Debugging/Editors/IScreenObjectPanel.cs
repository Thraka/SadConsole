using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Debug.Editors;

public interface IScreenObjectPanel
{
    void BuildTabItem(ImGuiRenderer renderer, ScreenObjectState state);
}

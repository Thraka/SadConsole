using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

public interface IScreenObjectPanel
{
    void BuildTabItem(ImGuiRenderer renderer, ScreenObjectState state);
}

using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

public interface IScreenObjectPanel
{
    void BuildUI(ImGuiRenderer renderer, ScreenObjectState state);
}

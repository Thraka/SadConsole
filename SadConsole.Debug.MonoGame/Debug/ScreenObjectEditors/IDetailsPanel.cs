using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.ScreenObjectEditors;

public interface IDetailsPanel
{
    void BuildUI(ImGuiRenderer renderer, ScreenObjectState state);
}

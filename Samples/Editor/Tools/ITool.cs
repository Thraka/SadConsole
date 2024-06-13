using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

public interface ITool
{
    string Name { get; }

    string Description { get; }

    void BuildSettingsPanel(ImGuiRenderer renderer);

    void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer);

    void OnSelected();

    void OnDeselected();

    void DocumentViewChanged();

    void DrawOverDocument();
}

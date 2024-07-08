using SadConsole.ImGuiSystem;
using SadConsole.Editor.Model;

namespace SadConsole.Editor.Tools;

public interface ITool
{
    string Name { get; }

    string Description { get; }

    void BuildSettingsPanel(ImGuiRenderer renderer);

    void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer);

    void OnSelected();

    void OnDeselected();

    void DocumentViewChanged(Document document);

    void DrawOverDocument(Document document, ImGuiRenderer renderer);
}

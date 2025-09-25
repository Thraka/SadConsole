using SadConsole.ImGuiSystem;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.Tools;

public interface ITool: ITitle
{
    string Description { get; }

    void BuildSettingsPanel(Document document);

    void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive);

    void OnSelected(Document document);

    void OnDeselected(Document document);

    void Reset(Document document);

    void DocumentViewChanged(Document document);

    void DrawOverDocument(Document document);
}

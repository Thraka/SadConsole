using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Documents;

public interface IBuilder: ITitle
{
    void ImGuiNewDocument(ImGuiRenderer renderer);

    bool IsDocumentValid();

    void ResetBuilder();

    Document CreateDocument();

    IEnumerable<IFileHandler> GetLoadHandlers();
}

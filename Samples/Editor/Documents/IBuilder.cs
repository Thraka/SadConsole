using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public interface IBuilder: ITitle
{
    void ImGuiNewDocument(ImGuiRenderer renderer);

    bool IsDocumentValid();

    void ResetBuilder();

    Document CreateDocument();

    IEnumerable<IFileHandler> GetLoadHandlers();
}

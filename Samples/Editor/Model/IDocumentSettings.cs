using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Model
{
    public interface IDocumentSettings
    {
        void BuildUIEdit(ImGuiRenderer renderer, bool readOnly);
        void BuildUINew(ImGuiRenderer renderer);
    }
}

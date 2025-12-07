using Hexa.NET.ImGui;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentScene
{
    public class Builder : IBuilder
    {
        public string Name;

        public string Title => "Scene";

        public Builder() =>
            ResetBuilder();

        public void ImGuiNewDocument(ImGuiRenderer renderer)
        {
            ImGui.Text("Name"u8);
            ImGui.InputText("##name"u8, ref Name, 50);

            ImGui.Separator();

            ImGui.TextWrapped("A scene is a container for other documents. It allows you to position multiple surfaces, layered surfaces, and animations together.");
            
            ImGui.Spacing();
            
            ImGui.TextDisabled("After creating the scene, use 'Document Options > Import document from list' to add existing documents to the scene.");
        }

        public bool IsDocumentValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public Document CreateDocument()
        {
            return new DocumentScene() { Title = Name };
        }

        public void ResetBuilder()
        {
            Name = Document.GenerateName("Scene");
        }

        public IEnumerable<IFileHandler> GetLoadHandlers() =>
            [new SceneDocument()];

        public override string ToString() =>
            Title;
    }
}

using SadConsole.Editor.Addin.SlicedBorder;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class SlicedDocumentFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, true);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, true);

    public string Title => "Sliced Border Document";
    public string[] ExtensionsLoading => ["border"];

    public string[] ExtensionsSaving => ["border"];

    public string HelpInformation => "Saves just the surface, without any other metadata, such as the document title.";

    public object? Load(string file)
    {
        // Try loading uncompressed first.
        if (!Serializer.TryLoad(file, false, out SlicedBorder? border))
        {
            if (!Serializer.TryLoad(file, true, out border))
            {
                SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to load file.\r\n\r\nIs it the wrong type?", "Error");
                return null;
            }
        }

        return new SlicedDocument(border) { Title = Document.GenerateName("SlicedBorder") };
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is SlicedDocument doc)
        {
            try
            {
                Serializer.Save(doc.Border, file, compress);

                return true;
            }
            catch (Exception e)
            {
                SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to save file.\r\n\r\n{e.Message}", "Error");
                return false;
            }
        }

        SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}", "Error");

        return false;
    }
}

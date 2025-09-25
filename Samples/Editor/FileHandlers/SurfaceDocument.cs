using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class SurfaceDocument: IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Editor Document";

    public string[] ExtensionsLoading => ["document"];

    public string[] ExtensionsSaving => ["document"];

    public string HelpInformation => "Saves or loads the document.";

    public object? Load(string file)
    {
        DocumentSurface doc;
        try
        {
            // Try to load the file as uncompressed first
            try
            {
                doc = Serializer.Load<DocumentSurface>(file, false);
            }
            catch (Exception e)
            {
                // Try to load the file as compressed next
                doc = Serializer.Load<DocumentSurface>(file, true);
            }
        }
        catch (Exception e)
        {
            MessageWindow.Show($"Unable to load file.\r\n\r\n{e.Message}", "Error");
            return null;
        }

        doc.TryLoadPalette(file + ".pal");

        if (doc is IDocumentSimpleObjects docObjs)
            docObjs.TryLoadObjects(file + ".objs");

        return doc;
    }

    public bool Save(object instance, string file, bool compress)
    {
        if (!file.EndsWith(((IFileHandler)this).ExtensionsSaving[0], StringComparison.InvariantCulture))
            file += "." + ((IFileHandler)this).ExtensionsSaving[0];

        if (instance is Document doc)
        {
            try
            {
                Serializer.Save(doc, file, file.EndsWith('z'));

                if (doc.HasPalette)
                    doc.SavePalette(file + ".pal");

                if (doc is IDocumentSimpleObjects docObjs)
                    docObjs.SaveObjects(file + ".objs");

                return true;
            }
            catch (Exception e)
            {
                MessageWindow.Show($"Unable to save file.\r\n\r\n{e.Message}", "Error");
                return false;
            }
        }

        MessageWindow.Show($"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}", "Error");

        return false;
    }
}

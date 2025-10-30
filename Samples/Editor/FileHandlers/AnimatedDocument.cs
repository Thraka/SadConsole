using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class AnimatedDocument: IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Animation Document";

    public string[] ExtensionsLoading => ["animdoc"];

    public string[] ExtensionsSaving => ["animdoc"];

    public string HelpInformation => "Saves or loads the animation.";

    public object? Load(string file)
    {
        DocumentAnimated doc;
        try
        {
            // Try to load the file as uncompressed first
            try
            {
                doc = Serializer.Load<DocumentAnimated>(file, false);
            }
            catch (Exception e)
            {
                // Try to load the file as compressed next
                doc = Serializer.Load<DocumentAnimated>(file, true);
            }

            doc.RefreshDuration();
            doc.SetFrameIndex(0);
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
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentAnimated doc)
        {
            try
            {
                Serializer.Save(doc, file, compress);

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

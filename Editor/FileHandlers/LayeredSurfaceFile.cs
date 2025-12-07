using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class LayeredSurfaceFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, true);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, true);

    public string Title => "Layered Surface";

    public string[] ExtensionsLoading => ["layeredsurface"];

    public string[] ExtensionsSaving => ["layeredsurface"];

    public string HelpInformation => "Saves just the layered surface layers, without any other metadata, such as the document title.";

    public object? Load(string file)
    {
        // Try loading uncompressed first.
        if (!Serializer.TryLoad<CellSurface[]>(file, false, out CellSurface[]? layers))
        {
            if (!Serializer.TryLoad<CellSurface[]>(file, true, out layers))
            {
                MessageWindow.Show($"Unable to load file.\r\n\r\nIs it the wrong type?", "Error");
                return null;
            }
        }

        if (layers == null || layers.Length == 0)
        {
            MessageWindow.Show($"Unable to load file.\r\n\r\nNo layers found.", "Error");
            return null;
        }

        LayeredScreenSurface layeredSurface = new(layers[0]);

        for (int i = 1; i < layers.Length; i++)
            layeredSurface.Layers.Add(layers[i]);

        return new DocumentLayeredSurface(layeredSurface);
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentLayeredSurface doc)
        {
            try
            {
                CellSurface[] layers = new CellSurface[doc.LayeredEditingSurface.Layers.Count];
                for (int i = 0; i < doc.LayeredEditingSurface.Layers.Count; i++)
                    layers[i] = (CellSurface)doc.LayeredEditingSurface.Layers[i];

                Serializer.Save(layers, file, compress);

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

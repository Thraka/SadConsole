using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class SurfaceFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, true);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, true);

    public string Title => "Cell Surface";

    public string[] ExtensionsLoading => ["surface"];

    public string[] ExtensionsSaving => ["surface"];

    public string HelpInformation => "Saves just the surface, without any other metadata, such as the document title.";

    public object? Load(string file)
    {
        // Try loading uncompressed first.
        if (!Serializer.TryLoad<CellSurface>(file, false, out CellSurface? surface))
        {
            if (!Serializer.TryLoad<CellSurface>(file, true, out surface))
            {
                MessageWindow.Show($"Unable to load file.\r\n\r\nIs it the wrong type?", "Error");
                return null;
            }
        }

        return new DocumentSurface(surface);
    }

    public bool Save(object instance, string file, bool compress)
    {
        if (!file.EndsWith(ExtensionsSaving[0], StringComparison.InvariantCultureIgnoreCase))
            file += "." + ExtensionsSaving[0];

        if (instance is Document doc)
        {
            ScreenSurface surface = doc.EditingSurface;

            try
            {
                Serializer.Save(surface.Surface, file, compress);

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

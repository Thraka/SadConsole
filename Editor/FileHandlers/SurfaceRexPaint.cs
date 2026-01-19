using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class SurfaceRexPaint : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => false;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, true);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, false);

    public string Title => "REXPaint";

    public string[] ExtensionsLoading => ["xp"];

    public string[] ExtensionsSaving => ["xp"];

    public string HelpInformation => "Imports and converts a REXPaint image to a SadConsole surface.";

    public object? Load(string file)
    {
        // Try loading uncompressed first.
        using Stream stream = File.OpenRead(file);
        Readers.REXPaintImage image = Readers.REXPaintImage.Load(stream);

        return new DocumentSurface((CellSurface)image.ToCellSurface()[0]);
    }

    public bool Save(object instance, string file, bool compress)
    {
        return false;
    }
}

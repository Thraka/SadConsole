using Hexa.NET.ImGui.SC.Windows;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.Editor.FileHandlers;

/// <summary>
/// File handler for loading PNG and BMP image files.
/// </summary>
internal class ImageFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => false;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, false);

    public string Title => "Image Files";

    public string[] ExtensionsLoading => ["png", "bmp"];

    public string[] ExtensionsSaving => [];

    public string HelpInformation => "Loads PNG and BMP image files for conversion to ASCII art.";

    public object? Load(string file)
    {
        using Stream stream = File.OpenRead(file);
        Texture2D texture = Texture2D.FromStream(SadConsole.Host.Global.GraphicsDevice, stream);
        return texture;
    }

    public bool Save(object instance, string file, bool compress)
    {
        return false;
    }
}

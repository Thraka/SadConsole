using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class AnimationFile : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, true);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, true);

    public string Title => "Animated Surface Object";

    public string[] ExtensionsLoading => ["animation"];

    public string[] ExtensionsSaving => ["animation"];

    public string HelpInformation => "Saves just the surface, without any other metadata, such as the document title.";

    public object? Load(string file)
    {
        // Try loading uncompressed first.
        if (!Serializer.TryLoad(file, false, out AnimatedScreenObject? surface))
        {
            if (!Serializer.TryLoad(file, true, out surface))
            {
                MessageWindow.Show($"Unable to load file.\r\n\r\nIs it the wrong type?", "Error");
                return null;
            }
        }

        return new DocumentAnimated(surface);
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is DocumentAnimated doc)
        {
            AnimatedScreenObject surface = doc._baseAnimation;

            try
            {
                Serializer.Save(surface, file, compress);

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

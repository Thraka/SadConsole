using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.FileHandlers;

internal class SimpleObjectsHandler : IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(false, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(false, false);

    public string Title => "Simple Objects";

    public string[] ExtensionsLoading => ["objs"];

    public string[] ExtensionsSaving => ["objs"];

    public string HelpInformation => "Imports and exports simple objects";

    public object? Load(string file)
    {
        if (Serializer.TryLoad<SimpleObjectDefinition[]>(file, false, out var simpleObjects))
        {
            return simpleObjects;
        }
        else
        {
            MessageWindow.Show($"Unable to load file.", "Error");
            return null;
        }
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is not SimpleObjectDefinition[])
        {
            MessageWindow.Show($"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}", "Error");
            return false;
        }

        try
        {
            SimpleObjectDefinition[] simpleObjects = (SimpleObjectDefinition[])instance;
            Serializer.Save(simpleObjects, file, false);
            return true;
        }
        catch (Exception e)
        {
            MessageWindow.Show($"Unable to save file.\r\n\r\n{e.Message}", "Error");
            return false;
        }
    }
}

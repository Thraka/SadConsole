using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;

namespace SadConsole.Editor.FileHandlers;

internal class ZonesHandler: IFileHandler
{
    public bool SupportsLoad => true;

    public bool SupportsSave => true;

    public FileDialogOptions DefaultSaveOptions { get; } = new(true, false);

    public FileDialogOptions DefaultLoadOptions { get; } = new(true, false);

    public string Title => "Zones";

    public string[] ExtensionsLoading => ["zones"];

    public string[] ExtensionsSaving => ["zones"];

    public string HelpInformation => "Imports and exports zones";

    public object? Load(string file)
    {
        ZoneSimplified[]? zones = null;

        if (Serializer.TryLoad<ZoneSerialized[]>(file, false, out var serializedZones))
        {
            zones = serializedZones.Select(z => (ZoneSimplified)z).ToArray();
            return zones;
        }
        else
        {
            SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to load file.", "Error");
            return null;
        }
    }

    public bool Save(object instance, string file, bool compress)
    {
        file = ((IFileHandler)this).GetFileWithValidExtensionForSave(file);

        if (instance is not ZoneSimplified[])
        {
            SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to save file.\r\n\r\nWrong type sent to handler:\r\n  {instance.GetType().Name}", "Error");
            return false;
        }

        try
        {
            ZoneSimplified[] zones = (ZoneSimplified[])instance;
            ZoneSerialized[] serializedZones = zones.Select(z => new ZoneSerialized
            {
                Name = z.Name,
                ZoneArea = Array.Empty<Point>(),
                Appearance = z.Appearance,
                Settings = z.Settings
            }).ToArray();

            Serializer.Save(serializedZones, file, false);
            return true;
        }
        catch (Exception e)
        {
            SadConsole.ImGuiSystem.MessageWindow.Show(Core.ImGuiComponent.ImGuiRenderer, $"Unable to save file.\r\n\r\n{e.Message}", "Error");
            return false;
        }
    }
}

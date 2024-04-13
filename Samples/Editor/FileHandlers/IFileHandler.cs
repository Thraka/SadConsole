namespace SadConsole.Editor.FileHandlers;

public interface IFileHandler
{
    bool SupportsLoad { get; }
    bool SupportsSave { get; }
    string FriendlyName { get; }
    string[] ExtensionsLoading { get; }
    string[] ExtensionsSaving { get; }
    object Load(string file);
    bool Save(object instance, string file);
    string HelpInformation { get; }
}

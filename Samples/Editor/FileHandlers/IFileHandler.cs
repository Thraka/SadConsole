using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.FileHandlers;

public interface IFileHandler: ITitle
{
    bool SupportsLoad { get; }
    bool SupportsSave { get; }
    FileDialogOptions DefaultSaveOptions { get; }
    FileDialogOptions DefaultLoadOptions { get; }
    string[] ExtensionsLoading { get; }
    string[] ExtensionsSaving { get; }
    object? Load(string file);
    bool Save(object instance, string file, bool compress);
    string HelpInformation { get; }
}

public record struct FileDialogOptions(bool OnlyCompressed, bool ShowCompressionToggle);

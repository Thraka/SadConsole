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

    /// <summary>
    /// Creates a serialized representation of the document without saving to file.
    /// Used by container documents (like Scene) to embed child documents.
    /// </summary>
    /// <param name="instance">The document instance to serialize.</param>
    /// <returns>The serialized object, or null if serialization failed.</returns>
    object? GetSerializedObject(object instance) => null;

    /// <summary>
    /// Creates a document from a serialized object.
    /// Used by container documents (like Scene) to reconstruct embedded child documents.
    /// </summary>
    /// <param name="serializedObject">The serialized object to reconstruct from.</param>
    /// <returns>The reconstructed document, or null if reconstruction failed.</returns>
    object? CreateFromSerialized(object serializedObject) => null;

    string GetFileWithValidExtensionForSave(string file)
    {
        if (Path.HasExtension(file))
        {
            foreach (string ext in ExtensionsSaving)
            {
                if (Path.GetExtension(file).Equals(ext, StringComparison.InvariantCultureIgnoreCase))
                    return file;
            }
        }

        return Path.ChangeExtension(file, ExtensionsSaving[0]);
    }
}

public record struct FileDialogOptions(bool OnlyCompressed, bool ShowCompressionToggle);

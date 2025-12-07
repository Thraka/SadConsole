namespace SadConsole.Editor.Serialization;

/// <summary>
/// Serialized representation of a scene document, including all embedded child documents.
/// </summary>
public class SceneDocumentSerialized
{
    /// <summary>
    /// The title of the scene.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The serialized child documents with their positions and type information.
    /// </summary>
    public SceneChildSerialized[]? Children { get; set; }

    /// <summary>
    /// Document options for the scene.
    /// </summary>
    public DocumentOptions? Options { get; set; }
}

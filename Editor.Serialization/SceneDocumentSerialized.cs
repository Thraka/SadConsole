using SadRogue.Primitives;

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
    /// Gets or sets the size of the scene in pixels.
    /// </summary>
    public Point ScenePixelSize { get; set; }

    /// <summary>
    /// The serialized child documents with their positions and type information.
    /// </summary>
    public SceneChildSerialized[]? Children { get; set; }

    /// <summary>
    /// Document options for the scene.
    /// </summary>
    public DocumentOptions? Options { get; set; }

    /// <summary>
    /// Metadata dictionary for the scene document.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    public IEnumerable<SceneChildSerialized> FindChild(string title, string documentType)
    {
        foreach (var child in Children ?? Array.Empty<SceneChildSerialized>())
        {
            if (child.DocumentType.Equals(documentType, StringComparison.InvariantCultureIgnoreCase))
            {
                if (child.Label.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;

                //else if (child.SerializedDocument is ITitle titleDoc && titleDoc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                //    yield return child;

                else if (child.SerializedDocument is SurfaceDocumentSerialized surfaceDoc && surfaceDoc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;

                else if (child.SerializedDocument is LayeredSurfaceDocumentSerialized layeredDoc && layeredDoc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;

                else if (child.SerializedDocument is AnimatedDocumentSerialized animDoc && animDoc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;

                else if (child.SerializedDocument is SceneDocumentSerialized sceneDoc && sceneDoc.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;
            }
        }
    }
}

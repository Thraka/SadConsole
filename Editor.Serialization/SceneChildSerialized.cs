using SadRogue.Primitives;

namespace SadConsole.Editor.Serialization;

/// <summary>
/// Represents a serialized child document within a scene, including its position and type information.
/// </summary>
public class SceneChildSerialized
{
    /// <summary>
    /// The type of the child document (e.g., "Surface", "LayeredSurface", "Animated").
    /// Used to determine which handler to use for deserialization.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// The position of the child document within the scene.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Whether the child uses pixel positioning instead of cell positioning.
    /// </summary>
    public bool UsePixelPositioning { get; set; }

    /// <summary>
    /// The font size used for cell-based positioning when UsePixelPositioning is false.
    /// This property is only used for DocumentScene children to determine the cell size for positioning.
    /// </summary>
    public Point SceneFontSize { get; set; }

    /// <summary>
    /// Optional display label for the child in the scene editor.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The serialized document data. The actual type depends on DocumentType.
    /// </summary>
    public object SerializedDocument { get; set; }

    /// <summary>
    /// The viewport rectangle defining which portion of the document to render.
    /// If null, the entire document surface is rendered.
    /// </summary>
    public Rectangle? Viewport { get; set; }
}

namespace SadConsole.Editor.Addins;

/// <summary>
/// The main entry point for an editor addin. Implement this interface in your
/// addin assembly and mark the assembly with <see cref="EditorAddinAttribute"/>.
/// The editor discovers and instantiates this type at startup.
/// </summary>
public interface IEditorAddin
{
    /// <summary>Display name shown in debug output and future addin manager.</summary>
    string Name { get; }

    /// <summary>Semantic version string (e.g. "1.0.0").</summary>
    string Version { get; }

    /// <summary>Author or organization name.</summary>
    string Author { get; }

    /// <summary>
    /// Called once after the addin assembly is loaded. Core.State and
    /// Core.ImGuiComponent are fully initialized when this is called.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Returns document builders this addin provides.
    /// Each builder appears in the New Document dialog. May return empty.
    /// </summary>
    IEnumerable<Documents.IBuilder> GetDocumentBuilders();

    /// <summary>
    /// Returns ImGui panels/objects this addin provides.
    /// Each panel is added to the ImGuiRenderer.UIObjects list. May return empty.
    /// </summary>
    IEnumerable<ImGuiSystem.ImGuiObjectBase> GetGuiPanels();

    /// <summary>
    /// Returns menu items this addin contributes to the top menu bar.
    /// Follows the StatusItems pattern — additive, processed each frame. May return empty.
    /// </summary>
    IEnumerable<AddinMenuItem> GetMenuItems();
}

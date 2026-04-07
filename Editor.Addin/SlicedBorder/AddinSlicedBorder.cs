using SadConsole.Editor.Addin.SlicedBorder;
using SadConsole.Editor.Addins;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

[assembly: EditorAddin(typeof(SlicedBorderAddin), Order = 10)]

namespace SadConsole.Editor.Addin.SlicedBorder;

/// <summary>
/// Example addin demonstrating how to extend the SadConsole editor.
/// Copy the compiled DLL to the editor's addins/ subfolder to load it.
/// </summary>
public class SlicedBorderAddin : IEditorAddin
{
    public string Name => "Sliced Border Addin";
    public string Version => "1.0.0";
    public string Author => "SadConsole";

    public void Initialize()
    {
        System.Diagnostics.Debug.WriteLine("[SlicedBorderAddin] Initialized.");
    }

    public IEnumerable<IBuilder> GetDocumentBuilders() =>
        [new SlicedDocument.Builder()];

    public IEnumerable<ImGuiObjectBase> GetGuiPanels() =>
        [];

    public IEnumerable<AddinMenuItem> GetMenuItems() =>
    [
        new AddinMenuItem("Addins", "Example: Hello World 2", () =>
        {
            System.Diagnostics.Debug.WriteLine("[SlicedBorderAddin] Hello from the example addin menu item!");
        })
    ];
}

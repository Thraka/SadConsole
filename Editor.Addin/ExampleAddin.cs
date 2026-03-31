using SadConsole.Editor.Addins;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

[assembly: EditorAddin(typeof(SadConsole.Editor.Addin.ExampleAddin))]

namespace SadConsole.Editor.Addin;

/// <summary>
/// Example addin demonstrating how to extend the SadConsole editor.
/// Copy the compiled DLL to the editor's addins/ subfolder to load it.
/// </summary>
public class ExampleAddin : IEditorAddin
{
    public string Name => "Example Addin";
    public string Version => "1.0.0";
    public string Author => "SadConsole";

    public void Initialize()
    {
        System.Diagnostics.Debug.WriteLine("[ExampleAddin] Initialized.");
    }

    public IEnumerable<IBuilder> GetDocumentBuilders() =>
        [];

    public IEnumerable<ImGuiObjectBase> GetGuiPanels() =>
        [];

    public IEnumerable<AddinMenuItem> GetMenuItems() =>
    [
        new AddinMenuItem("Addins", "Example: Hello World", () =>
        {
            System.Diagnostics.Debug.WriteLine("[ExampleAddin] Hello from the example addin menu item!");
        })
    ];
}

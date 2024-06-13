using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using SadConsole.Editor.Model;

namespace ImGuiNET;

public class DocumentTypeListControl
{
    /// <summary>
    /// Draws the list box.
    /// </summary>
    public static bool DrawListBox(string label, int heightInItems, ref int selectedIndex, [NotNullWhen(true)] ref Document? selectedDocument)
    {
        // If item chosen, or an item is selected but there's no actual document instance yet
        if (ImGui.ListBox(label, ref selectedIndex, DocumentTypeNames.Names, DocumentTypeNames.Names.Length, heightInItems) || selectedIndex != -1 && selectedDocument == null)
        {
            selectedDocument = selectedIndex switch
            {
                0 => new SurfaceDocument(),
                2 => new AnimationDocument(),
                _ => new SurfaceDocument(),
            };

            return true;
        }

        return false;
    }

    public static bool DrawComboBox(string label, int heightInItems, ref int selectedIndex, [NotNullWhen(true)] ref Document? selectedDocument)
    {
        // If item chosen, or an item is selected but there's no actual document instance yet
        if (ImGui.Combo(label, ref selectedIndex, DocumentTypeNames.Names, DocumentTypeNames.Names.Length, heightInItems) || selectedIndex != -1 && selectedDocument == null)
        {
            selectedDocument = selectedIndex switch
            {
                0 => new SurfaceDocument(),
                2 => new AnimationDocument(),
                _ => new SurfaceDocument(),
            };

            return true;
        }

        return false;
    }
}

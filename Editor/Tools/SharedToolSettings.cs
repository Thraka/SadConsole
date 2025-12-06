using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal static class SharedToolSettings
{
    public static ColoredGlyph Tip { get; set; }

    static SharedToolSettings()
    {
        Tip = new ColoredGlyph() { Glyph = 1 };
    }

    public static void ImGuiDrawZones(Document document)
    {

    }

    public static bool ImGuiDrawObjects(Document document, [NotNullWhen(true)] out SimpleObjectDefinition? obj)
    {
        IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)document;

        ImGui.SetNextItemWidth(-1);

        if (ImGui.BeginListBox("##pencil_simplegameobjects"u8))
        {
            SimpleObjectHelpers.DrawSelectables("pencil_simplegameobjects", docSimpleObjects.SimpleObjects, document.EditingSurfaceFont);

            ImGui.EndListBox();
        }

        bool isItemSelected = docSimpleObjects.SimpleObjects.IsItemSelected();

        if (isItemSelected)
        {
            ImGui.SeparatorText("Selected Object"u8);

            ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                document.EditingSurfaceFont,
                docSimpleObjects.SimpleObjects.SelectedItem!.Visual);
            ImGui.SameLine();
            ImGui.Text(docSimpleObjects.SimpleObjects.SelectedItem!.ToString());

            obj = docSimpleObjects.SimpleObjects.SelectedItem;

            return true;
        }

        obj = null;
        return false;
    }
}

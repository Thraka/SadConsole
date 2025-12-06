using System.Numerics;
using System.Reflection.Metadata;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Editor.Serialization;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
using SadConsole.UI.Controls;
using Document = SadConsole.Editor.Documents.Document;

namespace SadConsole.Editor.Tools;

internal class Zones : ITool
{
    public string Title => "\uf040 Zones";

    private bool _showZones = true;

    public string Description =>
        """
        ABC
        """;

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);

        ImGui.Checkbox("Show Zones", ref _showZones);

        ImGui.SeparatorText("Defined Zones"u8);

        ImGuiList<ZoneSerialized> zones = ((IDocumentZones)document).Zones;

        ImGui.SetNextItemWidth(-1);
        ImGui.ListBox("##zoneslist", ref zones.SelectedItemIndex, zones.Names, zones.Count, 6);

        if (zones.SelectedItem is not null)
        {
            ImGui.SeparatorText("Zone Properties"u8);
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##zone_name", ref zones.SelectedItem.Name, 256);

            ImGui.Text("Debug Apperance"u8);
            SettingsTable.BeginTable("##debugglyph");
            ImGuiTypes.ColoredGlyphReference glyphRef = zones.SelectedItem.Appearance;
            SettingsTable.DrawCommonSettings(true, true, false, true, true,
                ref glyphRef,
                null,
                null,
                document.EditingSurfaceFont,
                ImGuiCore.Renderer);
            zones.SelectedItem.Appearance = glyphRef.ToColoredGlyph();
            SettingsTable.EndTable();
            ImGui.Text("Settings..."u8);
            
        }
        
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        if (_showZones)
        {
            // Second layer, like the Tools.Empty tool does
            // but for zones. The ((IDocumentZones)document).Zones that have
            // ZoneSerialized.ZoneArea positions in the view of the document.EditingSurface.
            // The ZoneSerialized.Appearance is the debug glyph used to draw the zone.
            // Draw the zone areas with the Appearance glyph in a semi-transparent way.
            // The other layer (like the Tools.Empty tool uses) is just the size of the surface
            // viewport, while the backing surface is the full size of the document.
        }


        // isActive is when then mouse is pressed some how
        if (!isActive) return;
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}

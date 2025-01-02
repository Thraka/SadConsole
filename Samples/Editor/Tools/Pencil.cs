using System.Numerics;
using System.Reflection.Metadata;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
using SadConsole.UI.Controls;
using Document = SadConsole.Editor.Documents.Document;

namespace SadConsole.Editor.Tools;

internal class Pencil : ITool
{
    private readonly ImGuiList<string> _modes = new(0, "Draw", "Objects");
    private Windows.GlyphEditor _glyphEditor;

    public string Title => "\uf040 Pencil";

    public string Description =>
        """
        Draw glyphs on the surface.

        Use the left-mouse button to draw.

        The right-mouse button changes the current pencil tip to the foreground, background, and glyph, that is under the cursor.
        - Hold shift when clicking to only copy the colors.
        - Hold ctrl when clicking to only copy the glyph.
        """;

    public void BuildSettingsPanel(Document document)
    {
        bool supportsObjects = document is IDocumentSimpleObjects;

        if (supportsObjects)
        {
            ImGui.Separator();
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Mode");
            ImGui.SameLine();
            ImGui.Combo("##toolmode", ref _modes.SelectedItemIndex, _modes.Names, _modes.Count);
        }

        if (_modes.SelectedItemIndex == 0 || !supportsObjects)
        {
            ImGuiSC.BeginGroupPanel("Settings");

            Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
            Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
            int glyph = SharedToolSettings.Tip.Glyph;
            ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);

            if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
            {

                SettingsTable.DrawCommonSettings(true, true, true, true, true,
                    ref foreground, document.EditingSurface.Surface.DefaultForeground.ToVector4(),
                    ref background, document.EditingSurface.Surface.DefaultBackground.ToVector4(),
                    ref mirror,
                    ref glyph, document.EditingSurfaceFont, ImGuiCore.Renderer);

                SettingsTable.EndTable();
            }

            SharedToolSettings.Tip.Foreground = foreground.ToColor();
            SharedToolSettings.Tip.Background = background.ToColor();
            SharedToolSettings.Tip.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
            SharedToolSettings.Tip.Glyph = glyph;

            ImGuiSC.EndGroupPanel();
        }
        else
        {
            IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)document;

            ImGui.SetNextItemWidth(-1);

            if (ImGui.BeginListBox("##pencil_simplegameobjects"))
            {
                SimpleObjectHelpers.DrawSelectables("pencil_simplegameobjects", docSimpleObjects.SimpleObjects, document.EditingSurfaceFont);

                ImGui.EndListBox();
            }

            bool isItemSelected = docSimpleObjects.SimpleObjects.IsItemSelected();

            if (isItemSelected)
            {
                ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                    document.EditingSurfaceFont,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Visual);
                ImGui.SameLine();
                ImGui.Text(docSimpleObjects.SimpleObjects.SelectedItem!.ToString());
                SharedToolSettings.Tip = (ColoredGlyph)docSimpleObjects.SimpleObjects.SelectedItem.Visual.Clone();
            }

            if (isItemSelected)
            {
                ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                    document.EditingSurfaceFont,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Visual);

                ImGui.SameLine();
            }
        }
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.EditingSurface.Surface[hoveredCellPosition].Clear();
            SharedToolSettings.Tip.CopyAppearanceTo(document.EditingSurface.Surface[hoveredCellPosition]);
            document.EditingSurface.IsDirty = true;
        }
        else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
        {
            ColoredGlyphBase sourceCell = document.EditingSurface.Surface[hoveredCellPosition];

            if (ImGuiP.IsKeyDown(ImGuiKey.ModShift))
            {
                SharedToolSettings.Tip.Foreground = sourceCell.Foreground;
                SharedToolSettings.Tip.Background = sourceCell.Background;
            }
            else if (ImGuiP.IsKeyDown(ImGuiKey.ModCtrl))
            {
                SharedToolSettings.Tip.Glyph = sourceCell.Glyph;
            }
            else
                document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
        }
    }


    // private void GlyphEditor_Closed(object? sender, EventArgs e)
    // {
    //     if (_glyphEditor.DialogResult)
    //     {
    //         ColoredGlyph glyph = _glyphEditor.Glyph.ToColoredGlyph();
    //         if (_isAdding)
    //         {
    //             IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)Core.State.Documents.SelectedItem!;
    //
    //             docSimpleObjects.SimpleObjects.Objects.Add(
    //                 new SimpleObjectDefinition() { Visual = _glyphEditor.Glyph.ToColoredGlyph(), Name = _glyphEditor.Name! });
    //
    //             _isAdding = false;
    //         }
    //         else if (_isEditing)
    //         {
    //             IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)Core.State.Documents.SelectedItem!;
    //
    //             docSimpleObjects.SimpleObjects.SelectedItem!.Visual = _glyphEditor.Glyph.ToColoredGlyph();
    //             docSimpleObjects.SimpleObjects.SelectedItem!.Name = _glyphEditor.Name!;
    //
    //             SharedToolSettings.Tip = docSimpleObjects.SimpleObjects.SelectedItem.Visual;
    //
    //             _isEditing = false;
    //         }
    //     }
    //
    //     _glyphEditor = null;
    // }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}

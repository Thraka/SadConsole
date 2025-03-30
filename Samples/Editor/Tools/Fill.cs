using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Fill : ITool
{
    private readonly ImGuiList<string> _modes = new(0, "Draw", "Objects");
    private Windows.GlyphEditor? _glyphEditor;

    private bool _isAdding = false;
    private bool _isEditing = false;

    public string Title => "\ueb2a Fill";

    public string Description => """
        Fills an area of the surface.

        Use the left-mouse button to fill.

        The right-mouse button changes the current fill tip to the foreground, background, and glyph, that is under the cursor.
        """;

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);

        bool supportsObjects = document is IDocumentSimpleObjects;

        if (supportsObjects)
        {
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

            ImGui.BeginDisabled(!isItemSelected);
            if (ImGui.Button("Change Item"))
            {
                _glyphEditor = new(docSimpleObjects.SimpleObjects.SelectedItem!.Visual,
                                   document.EditingSurface.Surface.DefaultForeground,
                                   document.EditingSurface.Surface.DefaultBackground,
                                   document.EditingSurfaceFont, docSimpleObjects.SimpleObjects.SelectedItem!.Name);
                _glyphEditor.Closed += GlyphEditor_Closed;
                _isEditing = true;
                _glyphEditor.Open();
            }
            ImGui.EndDisabled();
            if (ImGui.Button("Add New Object"))
            {
                _glyphEditor = new(new ColoredGlyph(),
                                            document.EditingSurface.Surface.DefaultForeground,
                                            document.EditingSurface.Surface.DefaultBackground,
                                            document.EditingSurfaceFont, "Name");
                _glyphEditor.Closed += GlyphEditor_Closed;
                _isAdding = true;
                _glyphEditor.Open();
            }

            if (isItemSelected)
            {
                ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                    document.EditingSurfaceFont,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Visual);

                ImGui.SameLine();
            }

            if (ImGui.BeginCombo("##simplegameobjectscombo", docSimpleObjects.SimpleObjects.SelectedItem?.Name))
            {
                SimpleObjectHelpers.DrawSelectables("simplegameobjectscombo", docSimpleObjects.SimpleObjects, document.EditingSurfaceFont);

                ImGui.EndCombo();
            }
        }
    }

    private void GlyphEditor_Closed(object? sender, EventArgs e)
    {
        if (_glyphEditor!.DialogResult)
        {
            ColoredGlyph glyph = _glyphEditor.Glyph.ToColoredGlyph();
            if (_isAdding)
            {
                IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)Core.State.Documents.SelectedItem!;

                docSimpleObjects.SimpleObjects.Objects.Add(
                    new SimpleObjectDefinition() { Visual = _glyphEditor.Glyph.ToColoredGlyph(), Name = _glyphEditor.Name! });

                _isAdding = false;
            }
            else if (_isEditing)
            {
                IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)Core.State.Documents.SelectedItem!;

                docSimpleObjects.SimpleObjects.SelectedItem!.Visual = _glyphEditor.Glyph.ToColoredGlyph();
                docSimpleObjects.SimpleObjects.SelectedItem!.Name = _glyphEditor.Name!;

                SharedToolSettings.Tip = docSimpleObjects.SimpleObjects.SelectedItem.Visual;

                _isEditing = false;
            }
        }

        _glyphEditor = null;
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            ColoredGlyph cellToMatch = new();
            ColoredGlyphBase currentFillCell = SharedToolSettings.Tip;

            document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(cellToMatch);

            bool isTargetCell(ColoredGlyphBase c)
            {
                if (c.Glyph == 0 && cellToMatch.Glyph == 0)
                    return c.Background == cellToMatch.Background;

                return c.Foreground == cellToMatch.Foreground &&
                       c.Background == cellToMatch.Background &&
                       c.Glyph == cellToMatch.Glyph &&
                       c.Mirror == cellToMatch.Mirror;
            }

            void fillCell(ColoredGlyphBase c)
            {
                currentFillCell.CopyAppearanceTo(c);
                //console.TextSurface.SetEffect(c, _currentFillCell.Effect);
            }

            List<ColoredGlyphBase> cells = new(document.EditingSurface.Surface);

            Algorithms.NodeConnections<ColoredGlyphBase> getConnectedCells(ColoredGlyphBase c)
            {
                Algorithms.NodeConnections<ColoredGlyphBase> connections = new Algorithms.NodeConnections<ColoredGlyphBase>();

                var position = Point.FromIndex(cells.IndexOf(c), document.EditingSurface.Surface.Width);

                connections.West = document.EditingSurface.Surface.IsValidCell(position.X - 1, position.Y) ? document.EditingSurface.Surface[position.X - 1, position.Y] : null;
                connections.East = document.EditingSurface.Surface.IsValidCell(position.X + 1, position.Y) ? document.EditingSurface.Surface[position.X + 1, position.Y] : null;
                connections.North = document.EditingSurface.Surface.IsValidCell(position.X, position.Y - 1) ? document.EditingSurface.Surface[position.X, position.Y - 1] : null;
                connections.South = document.EditingSurface.Surface.IsValidCell(position.X, position.Y + 1) ? document.EditingSurface.Surface[position.X, position.Y + 1] : null;

                return connections;
            }

            if (!isTargetCell(currentFillCell))
                Algorithms.FloodFill(document.EditingSurface.Surface[hoveredCellPosition], isTargetCell, fillCell, getConnectedCells);

            document.EditingSurface.Surface.IsDirty = true;
        }
        else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
        {
            document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            document.EditingSurface.IsDirty = true;
        }
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}

using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Fill : ITool
{
    private readonly ImGuiList<string> _modes = new(0, "Draw", "Objects");

    public string Title => "\ueb2a Fill";

    public string Description => """
        Fills an area of the surface.

        Use the left-mouse button to fill.

        The right-mouse button changes the current fill tip to the foreground, background, and glyph, that is under the cursor.
        """;

    public void BuildSettingsPanel(Document document)
    {
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
            ImGuiSystem.Types.Mirror mirror = ImGuiSystem.Types.MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);

            if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
            {

                SettingsTable.DrawCommonSettings(true, true, true, true, true,
                    ref foreground, document.EditingSurface.Surface.DefaultForeground.ToVector4(),
                    ref background, document.EditingSurface.Surface.DefaultBackground.ToVector4(),
                    ref mirror,
                    ref glyph, document.EditingSurfaceFont, Core.ImGuiComponent.ImGuiRenderer);

                SettingsTable.EndTable();
            }

            SharedToolSettings.Tip.Foreground = foreground.ToColor();
            SharedToolSettings.Tip.Background = background.ToColor();
            SharedToolSettings.Tip.Mirror = ImGuiSystem.Types.MirrorConverter.ToSadConsoleMirror(mirror);
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
                ImGuiSC.FontGlyph.Draw(Core.ImGuiComponent.ImGuiRenderer, "gameobject_definition",
                    document.EditingSurfaceFont,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Visual);
                ImGui.SameLine();
                ImGui.Text(docSimpleObjects.SimpleObjects.SelectedItem!.ToString());
                SharedToolSettings.Tip = (ColoredGlyph)docSimpleObjects.SimpleObjects.SelectedItem.Visual.Clone();
            }

            ImGui.BeginDisabled(!isItemSelected);
            if (ImGui.Button("Change Item"))
            {
                Windows.GlyphEditorWindow.Show(Core.ImGuiComponent.ImGuiRenderer,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Visual,
                    document.EditingSurface.Surface.DefaultForeground,
                    document.EditingSurface.Surface.DefaultBackground,
                    document.EditingSurfaceFont,
                    (glyph, name) =>
                    {
                        IDocumentSimpleObjects doc = (IDocumentSimpleObjects)Core.State.SelectedDocument!;
                        doc.SimpleObjects.SelectedItem!.Visual = glyph.ToColoredGlyph();
                        doc.SimpleObjects.SelectedItem!.Name = name!;
                        SharedToolSettings.Tip = doc.SimpleObjects.SelectedItem.Visual;
                    },
                    null,
                    docSimpleObjects.SimpleObjects.SelectedItem!.Name);
            }
            ImGui.EndDisabled();
            if (ImGui.Button("Add New Object"))
            {
                Windows.GlyphEditorWindow.Show(Core.ImGuiComponent.ImGuiRenderer,
                    new ColoredGlyph(),
                    document.EditingSurface.Surface.DefaultForeground,
                    document.EditingSurface.Surface.DefaultBackground,
                    document.EditingSurfaceFont,
                    (glyph, name) =>
                    {
                        IDocumentSimpleObjects doc = (IDocumentSimpleObjects)Core.State.SelectedDocument!;
                        doc.SimpleObjects.Objects.Add(
                            new SimpleObjectDefinition() { Visual = glyph.ToColoredGlyph(), Name = name! });
                    },
                    null,
                    "Name");
            }

            if (isItemSelected)
            {
                ImGuiSC.FontGlyph.Draw(Core.ImGuiComponent.ImGuiRenderer, "gameobject_definition",
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

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
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
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
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

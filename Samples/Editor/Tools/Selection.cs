using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool
{
    private Point _firstPoint;
    private Point _secondPoint;
    private ShapeParameters _selectionBoxShape;
    private CellSurface _selectionSurface;
    private States _state = States.None;

    public string Title => "\uefa4 Selection\\Blueprints";

    public string Description =>
        """
        Selects an area of the document for cloning.

        To cancel the selection press the ESC key or right-click on the selection area.
        """;

    public Selection()
    {
        _selectionBoxShape = ShapeParameters.CreateStyledBoxFilled(
            ICellSurface.ConnectedLineThick,
            new ColoredGlyph(Color.White, Color.Black.SetAlpha(100)),
            new ColoredGlyph(Color.White, Color.Black.SetAlpha(50)));
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);

        ScreenSurface surface = document.EditingSurface;
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {

        if (_state == States.SelectionDone && ImGui.BeginPopupContextItem("selection_rightmenu"u8) || ImGuiP.IsPopupOpen("selection_rightmenu"u8))
        {
            if (ImGui.Selectable("Move"u8))
            {
                CopySurface(document);
                ClearSurface(document);
                _state = States.Pasting;

                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Clone"u8))
            {
                CopySurface(document);
                _state = States.Pasting;

                ImGui.CloseCurrentPopup();
            }
            if (ImGui.Selectable("Clone + Stamp"u8))
            {
                CopySurface(document);
                _state = States.PastingMultiple;

                ImGui.CloseCurrentPopup();
            }

            ImGui.Separator();
            if (ImGui.Selectable("Create Document"u8))
            {
                CopyToDocument(document);
                ImGui.CloseCurrentPopup();
            }

            ImGui.Separator();
            if (ImGui.Selectable("Erase"u8))
            {
                ClearSurface(document);
                document.VisualLayerToolLower.Surface.Clear();
                ClearState();

                ImGui.CloseCurrentPopup();
            }

            if (ImGui.Selectable("Cancel"u8))
            {
                CancelPaste(document, false);
                
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
            return;
        }

        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        // Cancelled but left mouse finally released, exit cancelled
        if (_state == States.Cancel && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            _state = States.None;

        // Cancelled
        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape)))
        {
            CancelPaste(document);
        }

        if (_state == States.Cancel)
            return;

        else if (_state is States.Pasting or States.PastingMultiple)
        {
            if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            {
                Point pos = hoveredCellPosition - new Point(_selectionSurface.Width / 2, _selectionSurface.Height / 2);
                _selectionSurface.Copy(document.EditingSurface.Surface, pos.X, pos.Y);

                if (_state == States.Pasting)
                {
                    CancelPaste(document, false);
                }
            }

            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape))
            {
                CancelPaste(document, false);
            }

            // Draw the floating surface
            else
            {
                document.VisualLayerToolLower.Surface.Clear();
                document.VisualLayerToolUpper.Surface.Clear();

                Point pos = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition - new Point(_selectionSurface.Width / 2, _selectionSurface.Height / 2);
                _selectionSurface.Copy(document.VisualLayerToolLower.Surface, pos.X, pos.Y);
                document.VisualLayerToolUpper.Surface.DrawBox(new Rectangle(pos.X, pos.Y, _selectionSurface.Width, _selectionSurface.Height),
                                                                            _selectionBoxShape);
                document.VisualLayerToolUpper.Surface.Print(0, 0, pos.ToString());
            }

            return;
        }
        else if (_state is States.SelectionDone && ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            ClearState();

        // Preview
        if (_state is States.MakingSelection or States.None && ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
        {
            if (_state == States.None)
            {
                _state = States.MakingSelection;

                _firstPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
            }

            _secondPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;

            document.VisualLayerToolLower.Surface.Clear();
            document.VisualLayerToolLower.Surface.DrawBox(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                                        new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                                                                        _selectionBoxShape);
        }

        // Commit selection
        else if (_state == States.MakingSelection && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
        {
            _state = States.SelectionDone;
            ImGui.OpenPopup("selection_rightmenu"u8);
        }
    }

    private void CopyToDocument(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);

        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);

        DocumentSurface doc = new DocumentSurface(_selectionSurface);
        doc.EditingSurfaceFont = document.EditingSurfaceFont;
        doc.EditingSurfaceFontSize = document.EditingSurfaceFontSize;
        doc.EditorFontSize = document.EditorFontSize;
        doc.Title = Document.GenerateName("Surface");

        Core.State.Documents.Objects.Add(doc);
    }

    private void CancelPaste(Document document, bool setCancel = true)
    {
        document.VisualLayerToolLower.Surface.Clear();
        document.VisualLayerToolUpper.Surface.Clear();
        ClearState();

        if (setCancel)
            _state = States.Cancel;
    }

    private void CopySurface(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        _selectionSurface = new(selectionArea.Width, selectionArea.Height);

        document.EditingSurface.Surface.Copy(selectionArea, _selectionSurface, 0, 0);
    }

    private void ClearSurface(Document document)
    {
        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

        Rectangle selectionArea = new(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                       new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

        document.EditingSurface.Surface.Clear(selectionArea);
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) =>
        CancelPaste(document, false);

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public void ClearState()
    {
        _firstPoint = Point.None;
        _secondPoint = Point.None;
        _state = States.None;
    }

    public override string ToString() =>
        Title;

    private enum States
    {
        None,
        MakingSelection,
        SelectionDone,
        Pasting,
        PastingMultiple,
        Cancel
    }
}

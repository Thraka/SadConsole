using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.ImGuiSystem;
using SadConsole.Editor.Model;
using SadRogue.Primitives;
using System.Numerics;

namespace SadConsole.Editor.Tools;

internal class Selection : ITool
{
    private bool _isFirstPointSelected = false;
    private Rectangle _boxArea;
    private Point _firstPoint;
    private bool _isCancelled;
    private bool _isPasting;
    CellSurface? _blueprint;
    private string[] _blueprintNames = [];
    private CellSurface[] _blueprintValues = [];
    private int _blueprintSelection = -1;
    private ToolMode _toolMode;

    private ShapeSettings.Settings _shapeSettings;

    public string Name => "Selection";

    public string Description => """
        ...
        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    private void DrawBlueprint()
    {
        if (_blueprint == null) return;

        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerLower.Surface.Clear();
        _blueprint.Copy(document.VisualToolLayerLower.Surface, _boxArea.X, _boxArea.Y);

        DrawBlueprintBox();
    }

    private void DrawBlueprintBox()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerUpper.Surface.Clear();
        document.VisualToolLayerUpper.Surface.DrawBox(_boxArea, ShapeParameters.CreateStyledBoxFilled(borderStyle: ICellSurface.ConnectedLineThin,
                                                                                borderColors: new ColoredGlyph(Color.White, Color.Black),
                                                                                fillStyle: new ColoredGlyph(Color.White.SetAlpha(150), Color.Transparent, 176)));
    }

    private void ToolLayersUpperClear()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerUpper.Clear();
    }

    private void ToolLayersLowerClear()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerLower.Clear();
    }

    private void ToolLayersClear()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerLower.Clear();
        document.VisualToolLayerUpper.Clear();
    }

    private void GetBlueprintFromSelection()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        _blueprint = new CellSurface(_boxArea.Width, _boxArea.Height);
        document.VisualDocument.Surface.Copy(_boxArea.Translate(document.VisualDocument.Surface.ViewPosition), _blueprint, 0, 0);
    }

    public void SelectionReset()
    {
        _toolMode = ToolMode.Selecting;
        _firstPoint = Point.None;
        _boxArea = Rectangle.Empty;
        _isFirstPointSelected = false;
        _blueprint = null;
        ToolLayersClear();
    }

    public void SelectionMove()
    {
        SelectionCopy();
        SelectionErase();
        DrawBlueprint();
    }

    public void SelectionCopy()
    {
        GetBlueprintFromSelection();
    }

    public void SelectionErase()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualDocument.Surface.Clear(_boxArea.Translate(document.VisualDocument.Surface.ViewPosition));
    }

    public void SelectionStore(string name)
    {
        _blueprintNames = [.. _blueprintNames, name];
        _blueprintValues = [.. _blueprintValues, _blueprint];
    }

    public void SelectionBluesprintStamp()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        Point location = _boxArea.Position + document.VisualDocument.Surface.ViewPosition;
        _blueprint.Copy(document.VisualDocument.Surface, location.X, location.Y);
    }

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        /*
         * --- Selection ---
         * BtnReset
         * BtnCopy
         * BtnStore
         * 
         * --- Blueprints ---
         * List Item
         * List Item
         * List Item
         * List Item
         * 
         * Btn
         * 
        */

        // Reset -> Selecting
        // Selecting
        // Selected
        //   Move  -> Erase -> Blueprint Selected
        //   Copy  -> Blueprint Selected
        //   Erase -> Reset
        //   Store -> Popup -> Selected
        // Blueprint Selected
        //   Stamp
        // Choose Blueprint -> Blueprint Selected
        // 
        Vector2 spacing = ImGui.GetStyle().ItemSpacing;

        ImGuiWidgets.BeginGroupPanel("Operations");

        ImGui.BeginDisabled(_toolMode == ToolMode.Selecting);
        if (ImGui.Button("Reset Selection"))
        {
            OnDeselected();
        }
        ImGui.EndDisabled();
        ImGui.Separator();

        ImGui.BeginDisabled(_toolMode != ToolMode.SelectedArea);
        if (ImGui.Button("Move"))
        {
            _toolMode = ToolMode.BlueprintMoveOnce;

            SelectionMove();
        }
        ImGui.EndDisabled();
        ImGui.SameLine();

        ImGui.BeginDisabled(_toolMode != ToolMode.SelectedArea);
        if (ImGui.Button("Copy"))
        {
            _toolMode = ToolMode.BlueprintMove;

            SelectionCopy();
        }
        ImGui.EndDisabled();
        ImGui.SameLine();

        ImGui.BeginDisabled(_toolMode != ToolMode.SelectedArea);
        if (ImGui.Button("Erase"))
        {
            SelectionErase();
            SelectionReset();
        }
        ImGui.EndDisabled();
        ImGui.Separator();

        ImGui.BeginDisabled(_toolMode != ToolMode.SelectedArea);
        if (ImGui.Button("Store"))
        {
            ImGui.OpenPopup("Blueprint Name");
        }
        ImGui.EndDisabled();
        ImGui.SameLine();

        ImGui.BeginDisabled(_toolMode != ToolMode.SelectedArea);
        if (ImGui.Button("Create New Document"))
        {
            Document document = ImGuiCore.State.GetOpenDocument();
            SurfaceDocument newDocument = new()
            {
                Width = _boxArea.Width,
                Height = _boxArea.Height
            };
            newDocument.Create();
            
            document.VisualDocument.Surface.Copy(_boxArea.Translate(document.VisualDocument.Surface.ViewPosition), newDocument.VisualDocument.Surface, 0, 0);
            ImGuiCore.State.OpenDocuments = [.. ImGuiCore.State.OpenDocuments, newDocument];
        }
        ImGui.EndDisabled();

        if (_blueprint != null)
        {
            string blueprintName = string.Empty;
            bool validateOutput(string name) =>
                string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || _blueprintNames.Contains(name);

            if (Windows.RenameWindow.Show("Blueprint Name", "Name", ref blueprintName, out bool accepted, validateOutput))
            {
                if (accepted)
                {
                    SelectionStore(blueprintName);
                }
            }
        }
        ImGuiWidgets.EndGroupPanel();

        ImGui.Text("Stored Blueprints");

        //Vector2 area = ImGui.GetItemRectSize();
        //ImGui.SetNextItemWidth(area.Y - ImGui.GetCursorPosY() - spacing.Y - area.Y);

        ImGui.SetNextItemWidth(ImGui.GetContentRegionMax().X - spacing.X);
        if (ImGui.ListBox("##listblueprints", ref _blueprintSelection, _blueprintNames, _blueprintNames.Length, 5))
        {
            _toolMode = ToolMode.BlueprintMove;
            
            _blueprint = _blueprintValues[_blueprintSelection];
            _boxArea = _blueprint.Area;

            DrawBlueprint();
            _blueprintSelection = -1;
        }

    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        // Cancelled but left mouse finally released, exit cancelled
        if (_isCancelled && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            _isCancelled = false;

        // Cancelled
        if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape)))
        {
            SelectionReset();
            _isCancelled = true;
        }

        if (_isCancelled)
            return;

        // Box selection logic
        if (_toolMode == ToolMode.Selecting)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;
                }

                Point secondPoint = hoveredCellPosition - document.VisualDocument.Surface.ViewPosition;

                _boxArea = new(new Point(Math.Min(_firstPoint.X, secondPoint.X), Math.Min(_firstPoint.Y, secondPoint.Y)),
                               new Point(Math.Max(_firstPoint.X, secondPoint.X), Math.Max(_firstPoint.Y, secondPoint.Y)));

                ToolLayersLowerClear();
                DrawBlueprintBox();
            }
            else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_boxArea != Rectangle.Empty)
                {
                    GetBlueprintFromSelection();
                    _isFirstPointSelected = false;
                    _toolMode = ToolMode.SelectedArea;

                }
                else
                {
                    SelectionReset();
                }
            }
        }

        // Reset by mouse or keyboard
        else if (_toolMode == ToolMode.SelectedArea)
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                SelectionReset();
            }
            else if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                SelectionReset();
            }
        }
        

        // Movement mode
        else if (_toolMode == ToolMode.BlueprintMove || _toolMode == ToolMode.BlueprintMoveOnce)
        {
            _boxArea = _boxArea.WithPosition(hoveredCellPosition - document.VisualDocument.Surface.ViewPosition);
            DrawBlueprint();

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                SelectionBluesprintStamp();

                if (_toolMode == ToolMode.BlueprintMoveOnce)
                    SelectionReset();
            }
            else if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                SelectionReset();
            }
        }
    }

    public void OnSelected()
    {

    }

    public void OnDeselected()
    {
        if (_toolMode == ToolMode.BlueprintMove)
            SelectionBluesprintStamp();

        SelectionReset();

        _isCancelled = false;
        _blueprint = null;
        _toolMode = ToolMode.Selecting;
    }

    public void DocumentViewChanged(Document document)
    {
        OnDeselected();
    }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }

    public enum ToolMode
    {
        Selecting,
        SelectedArea,
        BlueprintMove,
        BlueprintMoveOnce,
    }
}
